using ElectroPiTask.Domain.Entities;
using ElectroPiTask.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPiTask.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

            try
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var pendingMigrations = context.Database.GetPendingMigrations();

                if (pendingMigrations.Any())
                {
                    try
                    {
                        await context.Database.MigrateAsync();
                    }
                    catch (Exception ex)
                    {
                        // Log migration errors
                        Console.WriteLine($"Migration error: {ex.Message}");
                        throw;
                    }
                }

                await SeedAdminUserAsync(context);

                
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private static async Task SeedAdminUserAsync(ApplicationDbContext context)
        {
            // Check if admin exists
            if (context.Users.Any(u => u.Role == UserRole.Admin))
            {
                return;
            }

            // Create default admin user
            var admin = new User
            {
                FirstName = "Default",
                LastName = "User",
                Email = "admin@elctropi.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin,
            };

            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();

        }
    }
}
