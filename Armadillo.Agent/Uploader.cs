using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Armadillo.Shared;
using Armadillo.Siebel;

namespace Armadillo.Agent
{
    public class Uploader
    {
        private ISubcaseDataProdiver dataProdiver_;
        private DocumentClient documentClient_;

        private readonly string DatabaseId = "SubcaseMonitor";
        private readonly string SubcasesCollection = "Subcases";

        public Uploader(ISubcaseDataProdiver dataProdiver, DocumentClient documentClient)
        {
            dataProdiver_ = dataProdiver;
            documentClient_ = documentClient;
           
            InitializeAsync().Wait();
        }

        public async Task InitializeAsync()
        {
            await documentClient_.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseId });
            await documentClient_.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(DatabaseId),
                new DocumentCollection { Id = SubcasesCollection });
        }

        public async Task UpdateAsync()
        {
            var products = dataProdiver_.GetProducts();
            foreach(var product in products)
            {
                Console.WriteLine($"Register {product}");
                var subcases = await dataProdiver_.GetSubcasesAsync(product);
                foreach(var subcase in subcases)
                {
                    Console.WriteLine($"Register {subcase.Id} {subcase.Title}");
                    try
                    {
                        await documentClient_.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, SubcasesCollection, subcase.Id));
                        Console.WriteLine($"Found {subcase.Id}");
                    }
                    catch (DocumentClientException ex)
                    {
                        if (ex.StatusCode == HttpStatusCode.NotFound)
                        {
                            await documentClient_.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, SubcasesCollection), subcase);
                            Console.WriteLine($"Created {subcase.Id}");
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