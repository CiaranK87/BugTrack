using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using API.Authorization;
using Infrastructure.Security;
using API.Extensions;
using API.Middleware;
using API.Hubs;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Load environment variables from .env file
DotNetEnv.Env.Load();

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });


builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddScoped<IAuthorizationHandler, ProjectRoleHandler>();
builder.Services.AddScoped<IAuthorizationHandler, TicketAuthorizationHandler>();
builder.Services.AddLogging();
builder.Services.AddHttpClient();

builder.Services.Configure<API.Services.DemoSettings>(builder.Configuration.GetSection("DemoSettings"));
builder.Services.AddHostedService<API.Services.DemoReseedService>();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 500,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
});


builder.Services.AddAuthorization(options =>
{
    // Global role policies
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireClaim("globalrole", Roles.Global.Admin));

    options.AddPolicy("RequireProjectManagerRole", policy =>
        policy.RequireClaim("globalrole", Roles.Global.Admin, Roles.Global.ProjectManager));

    options.AddPolicy("CanCreateProjects", policy =>
        policy.RequireClaim("globalrole", Roles.Global.Admin, Roles.Global.ProjectManager));

    options.AddPolicy("CanManageGlobalRoles", policy =>
        policy.RequireClaim("globalrole", Roles.Global.Admin));

    // Project role policies
    options.AddPolicy("ProjectOwnerOrManager", policy =>
        policy.Requirements.Add(new ProjectRoleRequirement(Roles.Project.Owner, Roles.Project.ProjectManager)));

    options.AddPolicy("ProjectDeveloper", policy =>
        policy.Requirements.Add(new ProjectRoleRequirement(Roles.Project.Developer)));

    options.AddPolicy("ProjectUser", policy =>
        policy.Requirements.Add(new ProjectRoleRequirement(Roles.Project.User)));

    options.AddPolicy("ProjectContributor", policy =>
        policy.Requirements.Add(new ProjectRoleRequirement(Roles.Project.Owner, Roles.Project.ProjectManager, Roles.Project.Developer, Roles.Project.User, Roles.Project.Guest)));

    options.AddPolicy("ProjectAnyRole", policy =>
        policy.Requirements.Add(new ProjectRoleRequirement(Roles.Project.Owner, Roles.Project.ProjectManager, Roles.Project.Developer, Roles.Project.User, Roles.Project.Guest)));

    options.AddPolicy("CanUploadAttachments", policy =>
        policy.RequireClaim("globalrole", Roles.Global.User, Roles.Global.Developer, Roles.Global.ProjectManager, Roles.Global.Admin));

    options.AddPolicy("ProjectOwnerOnly", policy =>
        policy.Requirements.Add(new IsOwnerRequirement()));

    // Ticket operation policies
    options.AddPolicy("CanReadTicket", policy =>
        policy.Requirements.Add(new TicketOperationRequirement(TicketOperation.Read)));

    options.AddPolicy("CanCreateTicket", policy =>
        policy.Requirements.Add(new TicketOperationRequirement(TicketOperation.Create)));

    options.AddPolicy("CanEditTicket", policy =>
        policy.Requirements.Add(new TicketOperationRequirement(TicketOperation.Edit)));

    options.AddPolicy("CanDeleteTicket", policy =>
        policy.Requirements.Add(new TicketOperationRequirement(TicketOperation.Delete)));
});



var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

// Add security headers
app.UseSecurityHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseRateLimiter();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<TicketCommentHub>("/hubs/comments");
app.MapHub<NotificationHub>("/hubs/notifications");

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    Log.Information("Attempting to connect to BugTrack database...");

    // Apply migrations to the database
    try
    {
        await context.Database.MigrateAsync();
        Log.Information("Database migration completed successfully");
    }
    catch (Exception migrateEx)
    {
        Log.Error(migrateEx, "Migration failed: {ErrorMessage}", migrateEx.Message);
        Log.Information("Please ensure the 'BugTrack' database exists in your Neon console.");
    }
    
    // Create roles if they don't exist (needed for both dev and production)
    if (!await roleManager.RoleExistsAsync(Roles.Global.Admin))
        await roleManager.CreateAsync(new IdentityRole(Roles.Global.Admin));
    if (!await roleManager.RoleExistsAsync(Roles.Global.ProjectManager))
        await roleManager.CreateAsync(new IdentityRole(Roles.Global.ProjectManager));
    if (!await roleManager.RoleExistsAsync(Roles.Global.Developer))
        await roleManager.CreateAsync(new IdentityRole(Roles.Global.Developer));
    if (!await roleManager.RoleExistsAsync(Roles.Global.User))
        await roleManager.CreateAsync(new IdentityRole(Roles.Global.User));
    if (!await roleManager.RoleExistsAsync(Roles.Global.Guest))
        await roleManager.CreateAsync(new IdentityRole(Roles.Global.Guest));

    Log.Information("Application roles created/verified successfully");

    var seederLogger = services.GetRequiredService<ILogger<Program>>();
    await DemoSeeder.SeedDemoUserAsync(userManager, seederLogger);
}
catch (Exception ex)
{
    Log.Fatal(ex, "An error occurred during startup");
}

app.Run();
Log.CloseAndFlush();