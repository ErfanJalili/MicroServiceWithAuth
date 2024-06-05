using IdentityServer.Data;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace IdentityServer
{
    public class Startup
    {
        private IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            //services.AddIdentityServer()
            //    .AddInMemoryClients(Config.Clients)
            //    .AddInMemoryApiScopes(Config.ApiScopes)
            //    .AddInMemoryIdentityResources(Config.IdentityResources)
            //    .AddTestUsers(TestUsers.Users)
            //    .AddDeveloperSigningCredential();


            var connectionString = configuration.GetConnectionString("DefaultConnection");
            // Register DbContexts
            services.AddDbContext<MyPersistedGrantDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddDbContext<MyConfigurationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentityServer()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(typeof(Startup).Assembly.FullName));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(typeof(Startup).Assembly.FullName));
                    options.EnableTokenCleanup = true;
                })
                .AddDeveloperSigningCredential();

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<MyPersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<MyConfigurationDbContext>();
                context.Database.Migrate();

                if (!context.Clients.Any())
                {
                    foreach (var client in Config.Clients.ToList())
                    {
                        var clientEntity = new IdentityServer4.EntityFramework.Entities.Client
                        {
                            ClientId = client.ClientId,
                            ClientName = client.ClientName,
                            RequirePkce = client.RequirePkce,
                            AllowRememberConsent = client.AllowRememberConsent,
                            RedirectUris = client.RedirectUris.Select(uri => new ClientRedirectUri { RedirectUri = uri }).ToList(),
                            PostLogoutRedirectUris = client.PostLogoutRedirectUris.Select(uri => new ClientPostLogoutRedirectUri { PostLogoutRedirectUri = uri }).ToList(),
                            ClientSecrets = client.ClientSecrets.Select(secret => new ClientSecret
                            {
                                Value = secret.Value.Sha256(), // Hash the secret value for storage
                                Type = secret.Type,
                                Description = secret.Description,
                                Expiration = secret.Expiration,
                            }).ToList(),
                            AllowedScopes = client.AllowedScopes.Select(scope => new ClientScope { Scope = scope }).ToList()
                        };

                        // Add the client entity to the database context
                        context.Clients.Add(clientEntity);
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.IdentityResources.ToList())
                    {
                        var resourceEntity = new IdentityServer4.EntityFramework.Entities.IdentityResource
                        {
                            Name = resource.Name,
                            DisplayName = resource.DisplayName,
                            Description = resource.Description,
                            Required = resource.Required,
                            Emphasize = resource.Emphasize,
                            ShowInDiscoveryDocument = resource.ShowInDiscoveryDocument,
                            //Properties = resource.Properties?.ToDictionary(kv => kv.Key, kv => kv.Value),
                            UserClaims = resource.UserClaims?.Select(claim => new IdentityResourceClaim { Type = claim }).ToList()
                        };

                        // Add the identity resource entity to the database context
                        context.IdentityResources.Add(resourceEntity);
                    }
                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var resource in Config.ApiScopes.ToList())
                    {
                        var scopeEntity = new IdentityServer4.EntityFramework.Entities.ApiScope
                        {
                            Name = resource.Name,
                            DisplayName = resource.DisplayName,
                            Description = resource.Description,
                            Required = resource.Required,
                            Emphasize = resource.Emphasize,
                            ShowInDiscoveryDocument = resource.ShowInDiscoveryDocument,
                            //Properties = resource.Properties?.ToDictionary(kv => kv.Key, kv => kv.Value),
                            UserClaims = resource.UserClaims?.Select(claim => new ApiScopeClaim { Type = claim }).ToList()
                        };

                        // Add the API scope entity to the database context
                        context.ApiScopes.Add(scopeEntity);
                    }
                    context.SaveChanges();
                }
            }
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            InitializeDatabase(app);

            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapDefaultControllerRoute();
                });
            });
        }
    }
}
