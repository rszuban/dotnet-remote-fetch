using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RemoteFetch.Classes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteFetch
{
    class RemoteFetch
    {
        private readonly IConfiguration _configuration;
        private readonly FetchUnitDbContext _context;
        private readonly FetchScheduler _fetchScheduler;

        public RemoteFetch(IConfiguration configuration, FetchUnitDbContext context, FetchScheduler fetchScheduler)
        {
            _configuration = configuration;
            _context = context;
            _fetchScheduler = fetchScheduler;
        }
        public async Task Run()
        {
            FetchUnit[] fetchUnits = _configuration.GetSection("FetchUnits").Get<FetchUnit[]>();

            #region Create context, run migrations and sync it with config file
            _context.Database.Migrate();
            var fetchUnitNames = _context.FetchUnits.Select(x => x.UnitName);

            foreach (FetchUnit fetchUnit in fetchUnits)
            {
                foreach (FetchItem fetchItem in fetchUnit.FetchItems)
                {
                    fetchItem.Id = $"{fetchUnit.UnitName}-{fetchItem.ItemName}";
                }

                if (!fetchUnitNames.Contains(fetchUnit.UnitName))
                {
                    Console.WriteLine($"The DB context doesn't have the {fetchUnit.UnitName} so it will be added.");
                    _context.Add(fetchUnit);
                }
                await _context.SaveChangesAsync();
            }
            #endregion

            DateTime nextOccurence = _fetchScheduler.GetNextOccurence(fetchUnits, DateTime.Now);
            bool firstIteration = true;

            while (true)
            {
                if (!firstIteration)
                {
                    await _fetchScheduler.ExecuteScheduled(fetchUnits, DateTime.Now);

                    foreach (FetchUnit fetchUnit in fetchUnits)
                    {
                        _context.Update(fetchUnit);
                    }
                    _context.SaveChanges();


                    nextOccurence = _fetchScheduler.GetNextOccurence(fetchUnits, DateTime.Now);

                }

                int sleepInterval = _fetchScheduler.CalculateTimeout(nextOccurence);

                if (sleepInterval > 0)
                {
                    Console.WriteLine($"Sleeping until {nextOccurence}.");
                    Task.Delay(sleepInterval).Wait();
                }
                else
                {
                    Console.WriteLine($"Unusual {sleepInterval}. This isn't supposed to happen.");
                }

                firstIteration = false;
            }
        }
    }
}
