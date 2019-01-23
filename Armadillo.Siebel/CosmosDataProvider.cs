using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Armadillo.Shared;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;

namespace Armadillo.Siebel
{
    public class CosmosDataProvider : ISubcaseDataProdiver
    {
        ILogger logger_;
        private DocumentClient documentClient_;
        private readonly string DatabaseName = "SubcaseMonitor";
        private readonly string CollectionName = "Products";

        public CosmosDataProvider(DocumentClient documentClient, ILogger logger)
        {
            documentClient_ = documentClient;
            logger_ = logger;
        }

        public IEnumerable<string> GetProducts()
        {
            logger_.LogInformation($"Get products");

            var products = GetProductsAsync().Result;
            return products.Select(each => each.Name);
        }

        public string GetReportLink(string product)
        {
            logger_.LogInformation($"Get report link");

            var products = GetProductsAsync().Result;
            return products.First(each => each.Name == product).ReportLink;
        }

        public async Task<IEnumerable<Subcase>> GetSubcasesAsync(string productName)
        {
            logger_.LogInformation($"Loading subcases for {productName}");

            var products = (await GetProductsAsync()).ToArray();
            logger_.LogInformation($"Loaded products: {products.Count()}");

            var product = products.First(each => each.Name == productName);
            logger_.LogInformation($"Found product: {product.Name}");

            return product.Subcases;
        }

        public Task<IEnumerable<Product>> GetProductsAsync()
        {
            logger_.LogInformation($"Loading products");

            return Task<IEnumerable<Product>>.Run(() => {
                var query = documentClient_.CreateDocumentQuery<Product>(
                    UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), new FeedOptions { MaxItemCount = -1 });
                return query as IEnumerable<Product>;
            });
        }
    }
}
