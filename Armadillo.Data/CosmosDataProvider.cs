using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Armadillo.Shared;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;

namespace Armadillo.Data
{
    public class CosmosDataProvider : ISubcaseDataProdiver
    {
        ILogger _logger;
        private DocumentClient _documentClient;
        private readonly string DatabaseName = "SubcaseMonitor";
        private readonly string CollectionName = "Products";

        public CosmosDataProvider(DocumentClient documentClient, ILogger logger)
        {
            _documentClient = documentClient;
            _logger = logger;
        }

        public IEnumerable<string> GetProducts()
        {
            _logger.LogInformation($"Get products");

            var products = GetProductsAsync().Result;
            return products.Select(each => each.Name);
        }

        public string GetReportLink(string product)
        {
            _logger.LogInformation($"Get report link");

            var products = GetProductsAsync().Result;
            return products.First(each => each.Name == product).ReportLink;
        }

        public async Task<IEnumerable<Subcase>> GetSubcasesAsync(string productName)
        {
            _logger.LogInformation($"Loading subcases for {productName}");

            var products = (await GetProductsAsync()).ToArray();
            _logger.LogInformation($"Loaded products: {products.Count()}");

            var product = products.First(each => each.Name == productName);
            _logger.LogInformation($"Found product: {product.Name}");

            return product.Subcases;
        }

        public Task<IEnumerable<Product>> GetProductsAsync()
        {
            _logger.LogInformation($"Loading products");

            return Task<IEnumerable<Product>>.Run(() => {
                var query = _documentClient.CreateDocumentQuery<Product>(
                    UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), new FeedOptions { MaxItemCount = -1 });
                return query as IEnumerable<Product>;
            });
        }
    }
}
