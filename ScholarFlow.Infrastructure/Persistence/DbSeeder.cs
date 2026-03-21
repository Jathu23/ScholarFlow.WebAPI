using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScholarFlow.Domain.Entities;

namespace ScholarFlow.Infrastructure.Persistence;

/// <summary>
/// Database seeder for initial data
/// </summary>
public static class DbSeeder
{
    public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        string[] roles = { "Admin", "Teacher", "Student" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }

        var adminEmail = configuration["SeedUsers:Admin:Email"] ?? "admin@scholarflow.local";
        var adminPassword = configuration["SeedUsers:Admin:Password"] ?? "Admin@12345";
        var studentEmail = configuration["SeedUsers:Student:Email"] ?? "student@scholarflow.local";
        var studentPassword = configuration["SeedUsers:Student:Password"] ?? "Student@12345";

        await EnsureUserInRoleAsync(userManager, adminEmail, adminPassword, "Admin");
        await EnsureUserInRoleAsync(userManager, studentEmail, studentPassword, "Student");
    }

    private static async Task EnsureUserInRoleAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string role)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to seed user '{email}': {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            var addRoleResult = await userManager.AddToRoleAsync(user, role);
            if (!addRoleResult.Succeeded)
            {
                var errors = string.Join(", ", addRoleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to add role '{role}' to '{email}': {errors}");
            }
        }
    }
}
