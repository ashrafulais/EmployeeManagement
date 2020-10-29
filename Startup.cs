using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement
{
    public class Startup
    {
        private IConfiguration config;

        public Startup(IConfiguration configuration)
        {
            config = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        // Configure the services required for our application
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<AppDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("EmployeeDbConnection"))
            );

            services.AddIdentity<IdentityUser, IdentityRole>(options => 
            {
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = true;
                options.Password.RequiredUniqueChars = 3;
            })
            .AddEntityFrameworkStores<AppDbContext>(); //use EfCore to get user & role info

            /*services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 4;
            });*/

            services.AddScoped<IEmployeeRepository, SqlEmployeeRepository>();

            //we want the user to be authenticated
            services.AddRazorPages()
                .AddMvcOptions(options =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                                    .RequireAuthenticatedUser()
                                    .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddXmlSerializerFormatters();

            //we want the user to be authenticated
            /*services.AddMvc(options => 
            {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            })
            .AddXmlSerializerFormatters();*/
            //options.EnableEndpointRouting = false,
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //setup application's request processing pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error"); //error controller
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }
            //Serves the index.html / default.html files first
            app.UseStaticFiles();
            app.UseAuthentication();
            
            //> conventional routing
            /*app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });*/

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            //app.UseMvc();

        }
    }
}
