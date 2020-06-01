using Microsoft.EntityFrameworkCore;

namespace RemoteFetch.Classes
{
    class FetchUnitDbContext : DbContext
    {
        public DbSet<FetchUnit> FetchUnits { get; set; }

        public FetchUnitDbContext(DbContextOptions<FetchUnitDbContext> options)
            : base(options) { }
    }
}