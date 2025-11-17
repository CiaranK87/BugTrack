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

var builder = WebApplication.CreateBuilder(args);

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

    Console.WriteLine("Attempting to connect to BugTrack database...");

    // Apply migrations to the database
    try
    {
        await context.Database.MigrateAsync();
        Console.WriteLine("Database migration completed successfully");
    }
    catch (Exception migrateEx)
    {
        Console.WriteLine($"Migration failed: {migrateEx.Message}");
        Console.WriteLine("Please ensure the 'BugTrack' database exists in your Neon console.");
    }
    
    // Only seed data in development environment
    if (app.Environment.IsDevelopment())
    {
        await Seed.SeedData(context, userManager, roleManager);
    }
    else
    {
        // In production, create roles if they don't exist
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        if (!await roleManager.RoleExistsAsync("ProjectManager"))
            await roleManager.CreateAsync(new IdentityRole("ProjectManager"));
        if (!await roleManager.RoleExistsAsync("Developer"))
            await roleManager.CreateAsync(new IdentityRole("Developer"));
        if (!await roleManager.RoleExistsAsync("User"))
            await roleManager.CreateAsync(new IdentityRole("User"));
        
    }
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during startup");
}

app.Run();