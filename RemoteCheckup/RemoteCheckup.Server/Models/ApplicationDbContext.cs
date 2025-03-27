
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace RemoteCheckup.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<DatabaseTarget> DatabaseTargets { get; set; } = default!;
    }

    public static class ApplicationDbInitializer
    {
        public static void SeedDefaultUser(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ApplicationDbContext>>()))
            {
                if (!context.Users.Any())
                {
                    var username = "default-admin";
                    var user = new ApplicationUser {
                        UserName = username,
                        NormalizedUserName = username.ToUpperInvariant(),
                    };

                    var hasher = new PasswordHasher<ApplicationUser>();
                    var password = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                        .Substring(0, 18).Replace("/", "_").Replace("+", "-");
                    var phash = hasher.HashPassword(user, password);
                    user.PasswordHash = phash;

                    File.WriteAllLines("!default-user", [
                        "This is the login info for the default user:",
                        "Username: " + username,
                        "Password: " + password,
                    ]);

                    context.Users.Add(user);
                    context.SaveChanges();
                }

                if (!context.DatabaseTargets.Any())
                {
                    context.DatabaseTargets.Add(new () {
                        Name = "default-mysql",
                        DatabaseType = DatabaseType.MySQL,
                        ConnectionString = "server=localhost;username=root;password={{SECRET}};",
                        ConnectionSecret = ""
                    });
                    context.SaveChanges();
                }
            }
        }
    }
}
