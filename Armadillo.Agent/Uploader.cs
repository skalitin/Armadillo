using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Armadillo.Shared;
using Armadillo.Data;
using Microsoft.Extensions.Logging;

namespace Armadillo.Agent
{
    public class Uploader
    {
        private ISubcaseDataProdiver _dataProdiver;
        private DocumentClient _documentClient;
        private ILogger _logger;

        private readonly string DatabaseName = "SubcaseMonitor";
        
        private readonly string CollectionName = "Products";

        public Uploader(ISubcaseDataProdiver dataProdiver, DocumentClient documentClient, ILogger logger)
        {
            _dataProdiver = dataProdiver;
            _documentClient = documentClient;
            _logger = logger;

            InitializeAsync().Wait();
        }

        public async Task InitializeAsync()
        {
            await _documentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseName });
            
            await _documentClient.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(DatabaseName),
                new DocumentCollection { Id = CollectionName });
        }

        public async Task UpdateAsync()
        {
            var productNames = await _dataProdiver.GetProductsAsync();
            foreach(var productName in productNames)
            {
                try
                {
                    _logger.LogInformation($"Getting subcases for {productName}...");
                    var subcases = (await _dataProdiver.GetSubcasesAsync(productName)).ToArray();

                    _logger.LogInformation($"{subcases.Count()} subcases for {productName}...");
                    var product = new Product
                    {
                        Id = GetHash(productName),
                        Name = productName,
                        ReportLink = _dataProdiver.GetReportLink(productName),
                        Subcases = subcases.ToArray()
                    };
                    await RegisterProductAsync(product);
                    _logger.LogInformation($"Update complete for {productName}");
                }
                catch(Exception error)
                {
                    _logger.LogError(error, $"Error on updating subcases for {productName}");
                }
            }
        }

        private async Task RegisterProductAsync(Product product)
        {
            _logger.LogInformation($"Register {product.Name} and its subcases");
            try
            {
                var uri = UriFactory.CreateDocumentUri(DatabaseName, CollectionName, product.Id);
                await _documentClient.ReadDocumentAsync(uri);
                _logger.LogInformation($"Found product {product.Id} {product.Name}");
                
                await _documentClient.ReplaceDocumentAsync(uri, product);
                _logger.LogInformation($"Updated existing product {product.Id} {product.Name}");
            }
            catch (DocumentClientException error)
            {
                if (error.StatusCode == HttpStatusCode.NotFound)
                {
                    await _documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), product);
                    _logger.LogInformation($"Created new product {product.Id} {product.Name}");
                }
                else
                {
                    _logger.LogError(error, $"Error registering {product.Name}");
                    throw;
                }
            }

            _logger.LogInformation($"Registration completed for {product.Name}");
        }

        private string GetHash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(result).Replace("-", "");
            }
        }
    }
}