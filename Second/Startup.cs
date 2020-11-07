using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using First.AuthorizationRequirements;
using First.Data;
using First.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace First
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<AppDbContext>(options => { options.UseInMemoryDatabase("Memory"); });

            services.AddIdentity<PlantsistEmployee, IdentityRole>(options => { })
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options => {
                options.Cookie.Name = "Grandpa.Cookie";
                options.LoginPath = "/Home/Login";
            });

            services.AddScoped<IUserClaimsPrincipalFactory<PlantsistEmployee>,
                PlantsistEmployeeClaimsPrincipalFactory>();


            services.AddAuthorization( options => {

                //options.AddPolicy("Claim.Level", policyBuilder =>
                //{
                //    policyBuilder.AddRequirements(new CustomRequireClaim("level"));
                //});
                options.AddPolicy("Claim.Level", policyBuilder => 
                {
                    policyBuilder.RequireCustomClaim("level");
                });
            });

            services.AddScoped<IAuthorizationHandler, CustomRequireClaimHanlder>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
