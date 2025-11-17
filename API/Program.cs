using API.Authorization;
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
if (builder.Environment.IsDevelopment())
{
    DotNetEnv.Env.Load();
}

// Add services to the container.
builder.Services.AddControllers();


builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddScoped<IAuthorizationHandler, ProjectRoleHandler>();
builder.Services.AddScoped<IAuthorizationHandler, TicketAuthorizationHandler>();
builder.Services.AddLogging();


builder.Services.AddAuthorization(options =>
{
    // Global role policies
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireClaim("globalrole", "Admin"));

    options.AddPolicy("RequireProjectManagerRole", policy =>
        policy.RequireClaim("globalrole", "Admin", "ProjectManager"));

    options.AddPolicy("CanCreateProjects", policy =>
        policy.RequireClaim("globalrole", "Admin", "ProjectManager"));

    options.AddPolicy("CanManageGlobalRoles", policy =>
        policy.RequireClaim("globalrole", "Admin"));

    // Project role policies
    options.AddPolicy("ProjectOwnerOrManager", policy =>
        policy.Requirements.Add(new ProjectRoleRequirement("Owner", "ProjectManager")));

    options.AddPolicy("ProjectDeveloper", policy =>
        policy.Requirements.Add(new ProjectRoleRequirement("Developer")));

    options.AddPolicy("ProjectUser", policy =>
        policy.Requirements.Add(new ProjectRoleRequirement("User")));

    options.AddPolicy("ProjectContributor", policy =>
        policy.Requirements.Add(new ProjectRoleRequirement("Owner", "ProjectManager", "Developer", "User")));

    options.AddPolicy("ProjectAnyRole", policy =>
        policy.Requirements.Add(new ProjectRoleRequirement("Owner", "ProjectManager", "Developer", "User")));

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

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<TicketCommentHub>("/hubs/comments");

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
    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    if (!await roleManager.RoleExistsAsync("ProjectManager"))
        await roleManager.CreateAsync(new IdentityRole("ProjectManager"));
    if (!await roleManager.RoleExistsAsync("Developer"))
        await roleManager.CreateAsync(new IdentityRole("Developer"));
    if (!await roleManager.RoleExistsAsync("User"))
        await roleManager.CreateAsync(new IdentityRole("User"));
    
    Log.Information("Application roles created/verified successfully");
}
catch (Exception ex)
{
    Log.Fatal(ex, "An error occurred during startup");
}
finally
{
    Log.CloseAndFlush();
}

app.Run();