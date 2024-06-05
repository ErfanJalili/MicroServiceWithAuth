using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Data
{
    public class MyConfigurationDbContext : IdentityServer4.EntityFramework.DbContexts.ConfigurationDbContext
    {
        public MyConfigurationDbContext(DbContextOptions<IdentityServer4.EntityFramework.DbContexts.ConfigurationDbContext> options, ConfigurationStoreOptions storeOptions)
            : base(options, storeOptions)
        {
        }
    }
}
