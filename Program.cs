using System;
using RemoteFetch.Classes;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace RemoteFetch
{
    class Program
    {
        public static string datetimeFormat = "g";

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        async static Task MainAsync(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
            FetchUnit[] fetchUnits = configuration.GetSection("FetchUnits").Get<FetchUnit[]>();

            #region Create context, run migrations and sync it with config file
            using (FetchUnitDbContext context = new FetchUnitDbContext())
            {
                context.Database.Migrate();
                var fetchUnitNames = context.FetchUnits.Select(x => x.UnitName);

                foreach (FetchUnit fetchUnit in fetchUnits)
                {
                    foreach (FetchItem fetchItem in fetchUnit.FetchItems)
                    {
                        fetchItem.Id = $"{fetchUnit.UnitName}-{fetchItem.ItemName}";
                    }

                    if (!fetchUnitNames.Contains(fetchUnit.UnitName))
                    {
                        Console.WriteLine($"The DB context doesn't have the {fetchUnit.UnitName} so it will be added.");
                        context.Add(fetchUnit);
                    }
                    await context.SaveChangesAsync();
                }
            }
            #endregion

            FetchScheduler fetchScheduler = new FetchScheduler();
            DateTime nextOccurence = fetchScheduler.GetNextOccurence(fetchUnits, DateTime.Now);
            bool firstIteration = true;   

            while(true)
            {
                if (!firstIteration)
                {
                    await fetchScheduler.ExecuteScheduled(fetchUnits, DateTime.Now);

                    using (FetchUnitDbContext context = new FetchUnitDbContext())
                    {
                        foreach (FetchUnit fetchUnit in fetchUnits)
                        {
                            context.Update(fetchUnit);
                        }
                        context.SaveChanges();
                    }

                    nextOccurence = fetchScheduler.GetNextOccurence(fetchUnits, DateTime.Now);
                    
                }
                
                int sleepInterval = fetchScheduler.CalculateTimeout(nextOccurence);

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