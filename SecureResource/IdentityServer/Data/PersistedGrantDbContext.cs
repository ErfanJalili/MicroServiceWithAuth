using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Data
{
    public class MyPersistedGrantDbContext : IdentityServer4.EntityFramework.DbContexts.PersistedGrantDbContext
    {
        public MyPersistedGrantDbContext(DbContextOptions<IdentityServer4.EntityFramework.DbContexts.PersistedGrantDbContext> options, OperationalStoreOptions operationalStoreOptions)
            : base(options, operationalStoreOptions)
        {
        }
    }
}
