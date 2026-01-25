using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ScholarFlow.Domain.Entities;

namespace ScholarFlow.Infrastructure.Persistence;

/// <summary>
/// Database seeder for initial data
/// </summary>
public static class DbSeeder
{
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        string[] roles = { "Admin", "Teacher", "Student" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }
}
