using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FlowerHouse.Models;
using FlowerHouse.Models.Entity;
using FlowerHouse.BgService;

namespace FlowerHouse
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddIdentity<HouseUser, IdentityRole>(options =>
            {
                options.Password.RequireLowercase = false;
                options.User.AllowedUserNameCharacters = null;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })

            .AddEntityFrameworkStores<FlowerHouseContext>().AddDefaultTokenProviders();

            services.AddDbContext<FlowerHouseContext>(options => options.UseMySql("Data Source=localhost;Database=flowerhouse;User Id=root;Password=juno57;port=3306;SslMode=None;"));

            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                   "ManageHouse",
                   authBuilder =>
                   {
                       authBuilder.RequireClaim("ManageHouse", "Allowed");
                   });
            });

            services.AddHostedService<OrderTimeOutService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    app.UseHsts();
            //}


            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                        name: "areas",
                        template: "{area:exists}/{controller=Users}/{action=Index}/{id?}"
                  );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            SampleData.InitializeBlogDatabaseAsync(app.ApplicationServices).Wait();
        }
    }
}
