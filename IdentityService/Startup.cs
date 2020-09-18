using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Test;
using IdentityServerHost.Quickstart.UI;
using IdentityServerTesting.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServerTesting
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        string connectionString;
        string migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            connectionString = configuration.GetConnectionString("sqlDB");
            
        }        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddDbContext<ApplicationDbContext>(builder =>
            {
                builder.UseSqlServer(connectionString, sqlOptions =>
                { 
                    sqlOptions.MigrationsAssembly(migrationsAssembly); 
                });
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            
            services.AddIdentityServer()
                /*.AddInMemoryClients(Clients.Get())
                .AddInMemoryIdentityResources(Resources.GetIdentityResources())
                .AddInMemoryApiResources(Resources.GetApiResources())
                .AddInMemoryApiScopes(Resources.GetApiScopes())
                .AddTestUsers(Users.Get())*/
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                    {
                        builder.UseSqlServer(connectionString, sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(migrationsAssembly);
                        });
                    };
                })
                .AddAspNetIdentity<IdentityUser>()
                .AddOperationalStore(options =>
                { 
                    options.ConfigureDbContext = builder =>
                        {
                            builder.UseSqlServer(connectionString, sqlOptions =>
                            {
                                sqlOptions.MigrationsAssembly(migrationsAssembly);
                            });
                        };
                })
                .AddDeveloperSigningCredential();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            InitializeDbTestData(app);

            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private static void InitializeDbTestData(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                if (!context.Clients.Any())
                {
                    foreach (var client in Clients.Get())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Resources.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var scope in Resources.GetApiScopes())
                    {
                        context.ApiScopes.Add(scope.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Resources.GetApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                if (!roleManager.Roles.Any())
                {
                    var adminRole = new IdentityRole("admin");
                    roleManager.CreateAsync(adminRole).Wait();

                    roleManager.AddClaimAsync(adminRole, new Claim("permission", "projects.view")).Wait();
                    roleManager.AddClaimAsync(adminRole, new Claim("permission", "projects.create")).Wait();
                    roleManager.AddClaimAsync(adminRole, new Claim("permission", "projects.update")).Wait();

                    var superUserRole = new IdentityRole("superuser");
                    roleManager.CreateAsync(superUserRole).Wait();

                    roleManager.AddClaimAsync(superUserRole, new Claim("permission", "projects.view")).Wait();
                    roleManager.AddClaimAsync(superUserRole, new Claim("permission", "projects.create")).Wait();

                    var userRole = new IdentityRole("user");
                    roleManager.CreateAsync(userRole).Wait();

                    roleManager.AddClaimAsync(userRole, new Claim("permission", "projects.view")).Wait();
                }
                var listOfRoles = roleManager.Roles.ToList();
                if (!userManager.Users.Any())
                {
                    foreach (var testUser in TestUsers.Users)
                    {
                        var identityUser = new IdentityUser(testUser.Username)
                        {
                            Id = testUser.SubjectId

                        };
                        userManager.CreateAsync(identityUser, testUser.Password).Wait();
                        userManager.AddClaimsAsync(identityUser, testUser.Claims.ToList()).Wait();
                        foreach (var claim in testUser.Claims.ToList())
                        {
                            if (claim.Type == "role")
                            {
                                foreach (var role in listOfRoles)
                                {
                                    if (claim.Value.Contains(role.Name))
                                    {
                                        userManager.AddToRoleAsync(identityUser, role.Name).Wait();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
