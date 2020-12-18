using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MovieHub.Areas.Identity;
using MovieHub.Data;

[assembly: HostingStartup(typeof(IdentityHostingStartup))]
namespace MovieHub.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            
            builder.ConfigureServices((context, services) => {
               
                services.AddDefaultIdentity<MHUser>().AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>();
            });
        }
    }
    public static class ApplicationDbInitializer
    {
        public static async Task SeedUsers(UserManager<MHUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (userManager.FindByEmailAsync("admin@test.com").Result==null)
            {
                MHUser user = new MHUser
                {
                    UserName = "admin@test.com",
                    Email = "admin@test.com"
                };

                var result = userManager.CreateAsync(user, "admin123").Result;


                var roleResult = roleManager.CreateAsync(new IdentityRole("Admin")).Result;
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");

                }
            }       
        }   
    }
}