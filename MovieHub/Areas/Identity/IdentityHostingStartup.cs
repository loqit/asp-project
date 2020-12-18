using System;
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
               
                services.AddDefaultIdentity<MHUser>(options =>
                    {
                        options.SignIn.RequireConfirmedEmail = true;
                    }).AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>();
            });
        }
    }
    public static class ApplicationDbInitializer
    {
        public static async Task SeedUsers(UserManager<MHUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            Console.WriteLine("1");
            if (userManager.FindByEmailAsync("admin@test.com").Result==null)
            {
                var user = new MHUser
                {
                    UserName = "admin@test.com",
                    Email = "admin@test.com"
                };
                Console.WriteLine("2");
                var result = userManager.CreateAsync(user, "admin123").Result;
                //var roleResult = roleManager.CreateAsync(new IdentityRole("Admin")).Result;
                
                if (result.Succeeded)
                {
                    Console.WriteLine("3");
                    await userManager.AddToRoleAsync(user, "Admin");

                }
            }       
        }   
    }
}