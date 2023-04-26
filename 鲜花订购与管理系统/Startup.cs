using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FlowerHouse.Models;
using FlowerHouse.Models.Entity;
using FlowerHouse.BgService;
using Microsoft.Extensions.Hosting;

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

            //services.AddDbContextPool<FlowerHouseContext>(options => options.UseMySql("Data Source=localhost;Database=flowerhouse;User Id=root;Password=sin3306;port=3306;SslMode=None;",
            //    new MySqlServerVersion(new Version(8, 0, 21)),
            //    mysqlOptions =>
            //        mysqlOptions.CharSetBehavior(Pomelo.EntityFrameworkCore.MySql.Infrastructure.CharSetBehavior.NeverAppend))
            //    .EnableSensitiveDataLogging()
            //    .EnableDetailedErrors()
            //);

            services.AddDbContextPool<FlowerHouseContext>(options => {
                options.UseSqlite(Configuration.GetConnectionString("sqlite"));
                });


            

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

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }


            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSession();

            SampleData.InitializeBlogDatabaseAsync(app.ApplicationServices).Wait();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //            name: "areas",
            //            template: "{area:exists}/{controller=Users}/{action=Index}/{id?}"
            //      );

            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});

        }
    }
}
