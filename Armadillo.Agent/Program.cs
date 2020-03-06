using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
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

            Console.Read();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder => { 
                builder.AddDebug();
                builder.AddConsole(); 
            });

            var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddUserSecrets("28e6f711-a4c4-4cef-9e37-50ebfee35f91");
            var configuration = builder.Build();

            var clientHandler = new HttpClientHandler();
            if (configuration["Username"] == null || configuration["Password"] == null)
            {
                // Configure ReportServerClient powered by HttpClientHandler with NTLM authentication
                var credentials = new CredentialCache { { new Uri(ReportServerDataProvider.ReportServerUrl), "NTLM", CredentialCache.DefaultNetworkCredentials } };
                clientHandler.Credentials = credentials;
            }
            else
            {
                clientHandler.Credentials = new NetworkCredential(configuration["Username"], configuration["Password"]);
            }

            services
                .AddHttpClient<IReportServerClient, ReportServerClient>()
                .ConfigurePrimaryHttpMessageHandler(() => clientHandler);

            services.AddTransient<Application>();
        }   
    }
}
