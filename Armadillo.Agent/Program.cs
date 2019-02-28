using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Armadillo.Data;

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

            var credentials = new CredentialCache { { new Uri(ReportServerDataProvider.ReportServerUrl), "NTLM", CredentialCache.DefaultNetworkCredentials } };
            var clientHandler = new HttpClientHandler { Credentials = credentials };
            services
                .AddHttpClient<IReportServerClient, ReportServerClient>()
                .ConfigurePrimaryHttpMessageHandler(() => clientHandler);

            services.AddTransient<Application>();
        }   
    }
}
