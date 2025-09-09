using API.Authorization;
using API.Extensions;
using API.Middleware;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(new AuthorizeFilter());
});


builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddScoped<ProjectRoleHandler>();
builder.Services.AddScoped<IAuthorizationHandler>(sp => sp.GetRequiredService<ProjectRoleHandler>());
builder.Services.AddLogging();


builder.Services.AddAuthorization(options =>
{
    
    options.AddPolicy("CanCreateProjects", policy =>
        policy.RequireRole("Admin", "ProjectManager"));

    
    options.AddPolicy("ProjectOwnerOrManager", policy =>
        policy.Requirements.Add(new ProjectRoleRequirement("Owner", "ProjectManager")));

    options.AddPolicy("ProjectDeveloper", policy =>
        policy.Requirements.Add(new ProjectRoleRequirement("Developer")));

    options.AddPolicy("ProjectBusinessUser", policy =>
        policy.Requirements.Add(new ProjectRoleRequirement("BusinessUser")));

    
    options.AddPolicy("ProjectContributor", policy =>
        policy.Requirements.Add(new ProjectRoleRequirement("Owner", "ProjectManager", "Developer")));

    options.AddPolicy("ProjectAnyRole", policy =>
        policy.Requirements.Add(new ProjectRoleRequirement("Owner", "ProjectManager", "Developer", "BusinessUser")));
});



var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await context.Database.MigrateAsync();
    await Seed.SeedData(context, userManager, roleManager);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}

app.Run();