using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Armadillo.Agent
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            var application = serviceProvider.GetService<Application>();
            application.StartMonitoring();

            Console.ReadKey();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder => { 
                builder.AddDebug();
                builder.AddConsole(); 
            });

            services.AddTransient<Application>();
        }   
    }
}
