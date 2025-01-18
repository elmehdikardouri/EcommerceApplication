using EcomApplication.Data;
using EcomApplication.Interfaces;
using EcomApplication.Services; // Namespace des services ajoutés
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace EcommerceApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices((context, services) =>
                    {
                        // Connexion à la base de données
                        services.AddDbContext<EcommerceDbContext>(options =>
                            options.UseSqlServer(
                                context.Configuration.GetConnectionString("DefaultConnection")));

                        // Ajout des services personnalisés
                       

                        // Ajout des services MVC
                        services.AddControllersWithViews();

                        // Configuration de l'authentification par cookies
                        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                            .AddCookie(options =>
                            {
                                options.LoginPath = "/Account/Login";
                                options.AccessDeniedPath = "/Account/AccessDenied";
                                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                                options.SlidingExpiration = true;
                            });

                        services.AddScoped<IProductService, ProductService>();
                        services.AddScoped<ICartService, CartService>();
                        services.AddScoped<IOrderService, OrderService>();

                        // Configuration des sessions
                        services.AddDistributedMemoryCache(); // Nécessaire pour les sessions
                        services.AddSession(options =>
                        {
                            options.IdleTimeout = TimeSpan.FromMinutes(30);
                            options.Cookie.HttpOnly = true;
                            options.Cookie.IsEssential = true;
                        });
                    });

                    webBuilder.Configure((context, app) =>
                    {
                        var env = context.HostingEnvironment;

                        if (env.IsDevelopment())
                        {
                            app.UseDeveloperExceptionPage();
                        }
                        else
                        {
                            app.UseExceptionHandler("/Home/Error");
                            app.UseHsts();
                        }

                        app.UseHttpsRedirection();
                        app.UseStaticFiles();

                        // Ajout du middleware pour les sessions
                        app.UseSession();

                        app.UseRouting();
                        app.UseAuthentication();
                        app.UseAuthorization();

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllerRoute(
                                name: "default",
                                pattern: "{controller=Account}/{action=Login}/{id?}");
                        });
                    });
                });
    }
}
