using IdentityModel;
using IdentityServer.DATA;
using IdentityServer.Models.Users;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using static IdentityServer4.Models.IdentityResources;
using System.Security.Claims;
using System;
namespace IdentityServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {


            //services.AddIdentityServer()
            //    .AddInMemoryClients(Config.Clients)
            //    .AddInMemoryApiScopes(Config.ApiScopes)
            //    .AddInMemoryIdentityResources(Config.IdentityResources)
            //    .AddTestUsers(TestUsers.Users)
            //    .AddDeveloperSigningCredential();

            // Using EF Core
            #region IdentityServer4Connection with migrationAssembly

            const string connectionString = @"Server=.;Database=IdentityCoreDbV1;User Id=sa;Password=erfan@0912; MultipleActiveResultSets=True;Connection Timeout=30;TrustServerCertificate=True";
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            #endregion

            #region ApplicationDbContext

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly)));
            #endregion

            #region AddIdentity
            services.AddIdentity<User, Role>(config => config.SignIn.RequireConfirmedEmail = true)
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            #endregion

            //#region mailKit

            //var mailKit = Configuration.GetSection("Email").Get<MailKitOptions>();
            //services.AddMailKit(config => config.UseMailKit(mailKit));

            //#endregion



            services.AddIdentityServer()
               .AddAspNetIdentity<User>()
             .AddDeveloperSigningCredential()

            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b =>
                         b.UseSqlServer(connectionString,
                                         sql => sql.MigrationsAssembly(migrationAssembly));

            })
            .AddOperationalStore(options =>
                options.ConfigureDbContext = b =>
                         b.UseSqlServer(connectionString,
                                         sql => sql.MigrationsAssembly(migrationAssembly)));
    
            services.AddControllersWithViews();

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

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var identityContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
                identityContext.Database.Migrate();

                if (!identityContext.Users.Any())
                {
                    identityContext.Add(new User
                    {

                        Email = "jalilierfan77@gmail.com",
                        UserName = "admin",
                        NormalizedEmail = "jalilierfan77@gmail.com".ToUpper(),
                        NormalizedUserName = "admin".ToUpper(),
                        PasswordHash = "AQAAAAEAACcQAAAAEHVKmDA16wpDiUSLEWCs3qMpGj8MD1yvF5ToMcbBxfJBwCwPZdygr/tZy6vVIIUDZQ==",
                        EmailConfirmed = true,
                        SecurityStamp = "XXXX"
                    });

                    identityContext.SaveChanges();
                }
                if (!identityContext.Roles.Any())
                {
                    identityContext.Add(new Role
                    {
                        Name = "admin",
                        NormalizedName = "ADMIN",
                    });
                    identityContext.SaveChanges();
                }
                if (!identityContext.UserRoles.Any())
                {
                    identityContext.UserRoles.Add(new IdentityUserRole<long>
                    {
                        RoleId = 1,
                        UserId =1
                    });
                    identityContext.SaveChanges();
                }
                if (!identityContext.UserClaims.Any())
                {
                    identityContext.UserClaims.AddRange(new List<IdentityUserClaim<long>>
                    {
                        new IdentityUserClaim<long>
                        {
                        UserId = 1,
                        ClaimType = JwtClaimTypes.GivenName,
                        ClaimValue = "Erfan",
                        },
                        new IdentityUserClaim<long>
                        {
                        UserId = 1,
                        ClaimType = JwtClaimTypes.Email,
                        ClaimValue = "jalilierfan77@gmail.com",
                        },
                         new IdentityUserClaim<long>
                        {
                        UserId = 1,
                        ClaimType = JwtClaimTypes.Role,
                        ClaimValue = "admin",
                        }
                    });
                    identityContext.SaveChanges();
                }


                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    //foreach (var client in Config.Clients.ToList())
                    //{
                    //    context.Clients.Add(client.ToEntity());
                    //}


                    context.Clients.Add(new IdentityServer4.EntityFramework.Entities.Client
                    {
                        ClientId = "movieClient",
                        AllowedGrantTypes = new List<IdentityServer4.EntityFramework.Entities.ClientGrantType>
                        {
                           new ClientGrantType
                           {
                               GrantType =  GrantType.ClientCredentials
                           },
                        },
                        ClientSecrets = new List<ClientSecret>
                        {
                            new ClientSecret
                            {
                                 Value = "secret",
                                 Type = IdentityServerConstants.SecretTypes.SharedSecret
                            }
                        },

                        AllowedScopes =  new List<ClientScope>
                        {
                            new ClientScope
                            {
                              Scope = "movieAPI"
                            }
                        }
                    });


                    context.Clients.Add(new IdentityServer4.EntityFramework.Entities.Client
                    {
                        ClientId = "movies_mvc_client",
                        ClientName = "Movies MVC Web App",
                        AllowedGrantTypes =  new List<ClientGrantType>
                        {
                           new ClientGrantType
                           {
                               GrantType =  GrantType.Hybrid
                           },
                        },
                        RequirePkce = false,
                        AllowRememberConsent = false,
                        RedirectUris = new List<ClientRedirectUri>
                       {
                           new ClientRedirectUri
                           {
                               RedirectUri = "https://localhost:44347/signin-oidc"
                           }
                       },
                        PostLogoutRedirectUris = new List<ClientPostLogoutRedirectUri>()
                       {
                           new ClientPostLogoutRedirectUri
                           {
                            PostLogoutRedirectUri = "https://localhost:44347/signout-callback-oidc"
                           }
                       },
                        ClientSecrets = new List<ClientSecret>
                        {
                            new ClientSecret
                            {
                                 Value = "secret"
                            }
                        },
                        
                        AllowedScopes =  new List<ClientScope>
                        {

                            new ClientScope
                            {
                              Scope = "movieAPI",
                              
                            },
                            new ClientScope
                            {
                              Scope = "roles"
                            },
                            new ClientScope
                            {
                              Scope =  IdentityServerConstants.StandardScopes.OpenId,

                            },
                            new ClientScope
                            {
                              Scope =  IdentityServerConstants.StandardScopes.Profile,
                            },
                            new ClientScope
                            {
                              Scope =  IdentityServerConstants.StandardScopes.Address,
                            },
                            new ClientScope
                            {
                              Scope =  IdentityServerConstants.StandardScopes.Email,
                            },
                        }
                    });

                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    //foreach (var resource in Config.IdentityResources)
                    //{
                    //    context.IdentityResources.Add(resource.ToEntity());
                    //}

                    var resorce = Config.IdentityResources.Select(t => new IdentityServer4.EntityFramework.Entities.IdentityResource
                    {
                        Name = t.Name,
                        DisplayName = t.DisplayName,
                        Required = t.Required,
                        UserClaims = t.UserClaims.Select(tt => new IdentityResourceClaim 
                        {
                            Type = tt
                        } ).ToList(),
                        

                    }).ToList();
                    
                    context.IdentityResources.Add(new IdentityServer4.EntityFramework.Entities.IdentityResource
                    {
                        Name = IdentityServerConstants.StandardScopes.OpenId,
                        DisplayName = "Your user identifier",
                        Required = true,
                        
                        UserClaims = new List<IdentityResourceClaim>
                    {
                        new IdentityResourceClaim
                        {
                        Type = JwtClaimTypes.Subject
                        }
                    }
                    });
                    context.IdentityResources.Add(new IdentityServer4.EntityFramework.Entities.IdentityResource
                    {
                        Name = IdentityServerConstants.StandardScopes.Profile,
                        DisplayName = "User profile",
                        Description = "Your user profile information (first name, last name, etc.)",
                        Required = true,
                        Emphasize = true,
                        //UserClaims = Constants.ScopeToClaimsMapping[IdentityServerConstants.StandardScopes.Profile].ToList()
                        UserClaims = GetProfileUserClaims()
                    }) ;
                    context.IdentityResources.Add(new IdentityServer4.EntityFramework.Entities.IdentityResource
                    {
                        Name = IdentityServerConstants.StandardScopes.Address,
                        DisplayName = "Your postal address",
                        Emphasize = true,
                        UserClaims = new List<IdentityResourceClaim>
                    {
                        new IdentityResourceClaim
                        {
                            Type = JwtClaimTypes.Address
                        }
                    }
                    });
                    context.IdentityResources.Add(new IdentityServer4.EntityFramework.Entities.IdentityResource
                    {
                        Name = IdentityServerConstants.StandardScopes.Email,
                        DisplayName = "Your email address",
                        Emphasize = true,
                        UserClaims = new List<IdentityResourceClaim>
                    {
                        new IdentityResourceClaim
                        {
                            Type = JwtClaimTypes.Email
                        },
                            new IdentityResourceClaim
                        {
                            Type = JwtClaimTypes.EmailVerified
                        }
                    }
                    });
                   
                    context.IdentityResources.Add(new IdentityServer4.EntityFramework.Entities.IdentityResource
                    {
                        Name = "Roles",
                        DisplayName = "Your Role",
                        Emphasize = true,
                        UserClaims = new List<IdentityResourceClaim>
                    {
                        new IdentityResourceClaim
                        {
                            Type = JwtClaimTypes.Role
                        }
                    }
                    });

                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    //foreach (var resource in Config.ApiScopes)
                    //{
                    //    context.ApiScopes.Add(resource.ToEntity());
                    //}
                    context.ApiScopes.Add(new IdentityServer4.EntityFramework.Entities.ApiScope
                    {
                        Name = "movieAPI",
                        DisplayName ="Movie API"
                    });
                    
                    context.SaveChanges();
                }
            }
        }

        private List<IdentityResourceClaim> GetProfileUserClaims()
        {
                       
                return new List<IdentityResourceClaim> 
                {
                    new IdentityResourceClaim
                    {
                        Type = JwtClaimTypes.Name,
                    },
                    new IdentityResourceClaim
                    {
                        Type =  JwtClaimTypes.FamilyName,
                    },new IdentityResourceClaim
                    {
                        Type =  JwtClaimTypes.GivenName,
                    },new IdentityResourceClaim
                    {
                        Type =      JwtClaimTypes.MiddleName,
                    },new IdentityResourceClaim
                    {
                        Type = JwtClaimTypes.NickName,
                    },new IdentityResourceClaim
                    {
                        Type =  JwtClaimTypes.PreferredUserName,
                    },new IdentityResourceClaim
                    {
                        Type =  JwtClaimTypes.Profile,
                    },new IdentityResourceClaim
                    {
                        Type =JwtClaimTypes.Picture,
                    },new IdentityResourceClaim
                    {
                        Type = JwtClaimTypes.WebSite,
                    },new IdentityResourceClaim
                    {
                        Type = JwtClaimTypes.Gender,
                    },new IdentityResourceClaim
                    {
                        Type =   JwtClaimTypes.BirthDate,
                    },new IdentityResourceClaim
                    {
                        Type = JwtClaimTypes.ZoneInfo,
                    },new IdentityResourceClaim
                    {
                        Type = JwtClaimTypes.Locale,
                    },new IdentityResourceClaim
                    {
                        Type =  JwtClaimTypes.UpdatedAt
                    }
                };

        }
    }
}
