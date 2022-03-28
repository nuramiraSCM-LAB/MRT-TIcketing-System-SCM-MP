using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.Areas.Identity.Data;
using Project.Data;

[assembly: HostingStartup(typeof(Project.Areas.Identity.IdentityHostingStartup))]
namespace Project.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<ProjectContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("ProjectContextConnection")));

                services.AddDefaultIdentity<ApplicationUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                }
                
                )
                    .AddEntityFrameworkStores<ProjectContext>();
            });
        }
    }
}