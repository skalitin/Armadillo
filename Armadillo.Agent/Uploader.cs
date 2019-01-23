using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Armadillo.Shared;
using Armadillo.Siebel;
using Microsoft.Extensions.Logging;

namespace Armadillo.Agent
{
    public class Uploader
    {
        private ISubcaseDataProdiver dataProdiver_;
        private DocumentClient documentClient_;
        private ILogger logger_;

        private readonly string DatabaseName = "SubcaseMonitor";
        
        private readonly string CollectionName = "Products";

        public Uploader(ISubcaseDataProdiver dataProdiver, DocumentClient documentClient, ILogger logger)
        {
            dataProdiver_ = dataProdiver;
            documentClient_ = documentClient;
            logger_ = logger;

            InitializeAsync().Wait();
        }

        public async Task InitializeAsync()
        {
            await documentClient_.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseName });
            
            await documentClient_.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(DatabaseName),
                new DocumentCollection { Id = CollectionName });
        }

        public async Task UpdateAsync()
        {
            var productNames = dataProdiver_.GetProducts();
            foreach(var productName in productNames)
            {
                try
                {
                    logger_.LogInformation($"Getting subcases for {productName}...");
                    var subcases = (await dataProdiver_.GetSubcasesAsync(productName)).ToArray();

                    logger_.LogInformation($"{subcases.Count()} subcases for {productName}...");
                    var product = new Product
                    {
                        Id = GetHash(productName),
                        Name = productName,
                        ReportLink = dataProdiver_.GetReportLink(productName),
                        Subcases = subcases.ToArray()
                    };
                    await RegisterProductAsync(product);
                    logger_.LogInformation($"Update complete for {productName}");
                }
                catch(Exception error)
                {
                    logger_.LogError(error, $"Error on updating subcases for {productName}");
                }
            }
        }

        private async Task RegisterProductAsync(Product product)
        {
            logger_.LogInformation($"Register {product.Name} and its subcases");
            try
            {
                var uri = UriFactory.CreateDocumentUri(DatabaseName, CollectionName, product.Id);
                await documentClient_.ReadDocumentAsync(uri);
                logger_.LogInformation($"Found product {product.Id} {product.Name}");
                
                await documentClient_.ReplaceDocumentAsync(uri, product);
                logger_.LogInformation($"Updated existing product {product.Id} {product.Name}");
            }
            catch (DocumentClientException error)
            {
                if (error.StatusCode == HttpStatusCode.NotFound)
                {
                    await documentClient_.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), product);
                    logger_.LogInformation($"Created new product {product.Id} {product.Name}");
                }
                else
                {
                    logger_.LogError(error, $"Error registering {product.Name}");
                    throw;
                }
            }

            logger_.LogInformation($"Registration completed for {product.Name}");
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