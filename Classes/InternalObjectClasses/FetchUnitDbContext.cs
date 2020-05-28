using Microsoft.EntityFrameworkCore;

namespace RemoteFetch.Classes
{
    class FetchUnitDbContext : DbContext
    {
        public DbSet<FetchUnit> FetchUnits { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //I couldn't figure out passing the connection string from main function :<
            optionsBuilder.UseMySQL("SetMe");
        }
    }
}