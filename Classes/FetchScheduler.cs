using System;
using System.Threading.Tasks;
using NCrontab;

namespace RemoteFetch.Classes
{
    class FetchScheduler
    {
        public string datetimeFormat = "g";

        public DateTime GetNextOccurence(FetchUnit[] fetchUnits, DateTime dateTimeNow)
        {
            DateTime closestSchedule = dateTimeNow;

            foreach (FetchUnit fetchUnit in fetchUnits)
            {
                fetchUnit.NextScheduledFetch = CrontabSchedule.Parse(fetchUnit.Schedule).GetNextOccurrence(dateTimeNow);

                if ( closestSchedule == dateTimeNow)
                {
                    closestSchedule = fetchUnit.NextScheduledFetch;
                }

                if (DateTime.Compare(closestSchedule, fetchUnit.NextScheduledFetch) > 0)
                {
                    closestSchedule = fetchUnit.NextScheduledFetch;
                }
            }
            return closestSchedule;
        }

        public async Task ExecuteScheduled(FetchUnit[] fetchUnits, DateTime dateTimeNow)
        {
            foreach (FetchUnit fetchUnit in fetchUnits)
            {
                if (dateTimeNow.ToString(datetimeFormat) == fetchUnit.NextScheduledFetch.ToString(datetimeFormat))
                {
                    Console.WriteLine($"Starting execution of the {fetchUnit.UnitName}.");
                    FetchExecutor fetchExecutor = new FetchExecutor();
                    await fetchExecutor.ExecuteTask(fetchUnit, DateTime.Now);
                }
            }
        }

        public int CalculateTimeout(DateTime nextOccurence)
        {
            DateTime now = DateTime.Now;
            double timeout = ((nextOccurence - now).TotalMilliseconds);

            return Convert.ToInt32(Math.Ceiling(timeout));
        }
    }
}
