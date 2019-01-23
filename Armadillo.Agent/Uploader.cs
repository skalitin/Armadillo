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
        private readonly string CollectionName = "Subcases";

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
            var products = dataProdiver_.GetProducts();
            foreach(var product in products)
            {
                logger_.LogInformation($"Register {product}");
                var subcases = await dataProdiver_.GetSubcasesAsync(product);
                foreach(var subcase in subcases)
                {
                    logger_.LogInformation($"Register {subcase.Id} {subcase.Title}");
                    try
                    {
                        await documentClient_.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseName, CollectionName, subcase.Id));
                        logger_.LogInformation($"Found {subcase.Id}");
                    }
                    catch (DocumentClientException ex)
                    {
                        if (ex.StatusCode == HttpStatusCode.NotFound)
                        {
                            await documentClient_.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), subcase);
                            logger_.LogInformation($"Created {subcase.Id}");
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
       }
    }
}