using System;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Armadillo.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Armadillo.Agent
{
    public class Application
    {
        IServiceProvider _serviceProvider;
        ILoggerFactory _loggerFactory;
        ILogger _logger;
        Timer _timer = new Timer(1000 * 60 * 5);
        
        public Application(ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger("Application");
        }

        public void StartMonitoring()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddUserSecrets("28e6f711-a4c4-4cef-9e37-50ebfee35f91");
                var configuration = builder.Build();

                var dataProviderName = configuration["SubcaseDataProvider"];
                _logger.LogInformation($"Data provider: {dataProviderName}");

                ISubcaseDataProdiver dataProvider = null;
                if(String.Equals("Random", dataProviderName, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("Using random data provider");
                    dataProvider = new RandomDataProvider();
                }
                else if(String.Equals("Report", dataProviderName, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("Using SSRS report data provider");
                    var reportServerClient = _serviceProvider.GetService<IReportServerClient>();
                    dataProvider = new ReportServerDataProvider(_loggerFactory.CreateLogger("ReportServerDataProvider"), reportServerClient);
                }
                else
                {
                    throw new ApplicationException($"Unsupported data provider: {dataProviderName}");
                }

                var endpointUri = configuration["CosmosDB:EndpointUri"];
                var primaryKey = configuration["CosmosDB:PrimaryKey"];

                _logger.LogInformation("Using database endpoint {0}", endpointUri);
                var documentClient = new DocumentClient(new Uri(endpointUri), primaryKey);

                var uploader = new Uploader(dataProvider, documentClient, _loggerFactory.CreateLogger("Uploader"));

                _timer.Elapsed += (sender, e) =>
                {
                    _logger.LogInformation("Update started...");

                    uploader.UpdateAsync().Wait();
                    _logger.LogInformation("Update completed");
                };

                _timer.AutoReset = true;
                _timer.Enabled = true;
                _timer.Start();

                _logger.LogInformation($"Periodic update started at every {_timer.Interval/60000} min.");

                // await TestReadSubcasesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        // For test/dev purposes only - read and print uploaded data
        private async Task TestReadSubcasesAsync(DocumentClient documentClient)
        {
            var cosmosDataProvider = new CosmosDataProvider(documentClient, _loggerFactory.CreateLogger("CosmosDataProvider"));
            var products = await cosmosDataProvider.GetProductsAsync();
            foreach(var product in products)
            {
                _logger.LogInformation($"Read product {product}");

                var subcases = await cosmosDataProvider.GetSubcasesAsync(product);
                foreach (var subcase in subcases)
                {
                    _logger.LogInformation($"Subcase {subcase.Id} {subcase.Title}");
                }
            }            
        }
    }
}
