using System;
using RemoteFetch.Classes;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace RemoteFetch
{
    class Program
    {
        public static string datetimeFormat = "g";

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            var configuration = LoadConfiguration();
            services.AddSingleton(configuration);
            services.AddDbContext<FetchUnitDbContext>(options => options.UseMySQL(configuration.GetConnectionString("DefaultConnection")));
            services.AddSingleton<FetchScheduler>();
            services.AddSingleton<FetchExecutor>();
            services.AddTransient<RemoteFetch>();

            return services;
        }

        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            return builder.Build();
        }

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        async static Task MainAsync(string[] args)
        {
            var services = ConfigureServices();

            var serviceProvider = services.BuildServiceProvider();

            // calls the Run method in App, which is replacing Main
            await serviceProvider.GetService<RemoteFetch>().Run();
        }
    }
}