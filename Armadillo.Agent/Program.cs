using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Armadillo.Shared;
using Armadillo.Siebel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;

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
            Task.Run(() => application.Run()).Wait();
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
