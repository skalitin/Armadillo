using System;
using System.Linq;
using System.Threading.Tasks;
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
                var subcases = await dataProdiver_.GetSubcasesAsync(productName);
                var product = new Product
                {
                    Id = productName.GetHashCode().ToString(),
                    Name = productName,
                    ReportLink = dataProdiver_.GetReportLink(productName),
                    Subcases = subcases.ToArray()
                };
                await RegisterProductAsync(product);
            }
       }

       private async Task RegisterProductAsync(Product product)
       {
            logger_.LogInformation($"Register product {product.Name}");
            try
            {
                await documentClient_.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseName, CollectionName, product.Id));
                logger_.LogInformation($"Found product {product.Id} {product.Name}");
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    await documentClient_.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), product);
                    logger_.LogInformation($"Created product {product.Id} {product.Name}");
                }
                else
                {
                    throw;
                }
            }
       }
    }
}