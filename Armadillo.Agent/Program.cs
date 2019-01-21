using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Armadillo.Shared;

namespace Armadillo.Agent
{
    public class Subcase2
    {
        public string id { get; set; }
        public string title { get; set; }
    }

    class Program
    {
        public static IConfigurationRoot Configuration { get; set; }
        private static string EndpointUri = "";
        private static string PrimaryKey = "";

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddUserSecrets("28e6f711-a4c4-4cef-9e37-50ebfee35f91");
            Configuration = builder.Build();
            EndpointUri = Configuration["CosmosDB:EndpointUri"];
            PrimaryKey = Configuration["CosmosDB:PrimaryKey"];

            // Console.WriteLine("Uri {0}, key {1}", EndpointUri, PrimaryKey);
            // return ;

            try
            {
                var p = new Program();
                p.Spike().Wait();
            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }
        }

        private async Task Spike()
        {
            var client = new DocumentClient(new Uri(EndpointUri), PrimaryKey);
            await client.CreateDatabaseIfNotExistsAsync(new Database { Id = "SubcaseMonitor" });
            Console.WriteLine("Done 1");

            await client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri("SubcaseMonitor"),
                new DocumentCollection { Id = "Products" });
            Console.WriteLine("Done 2");

            var databaseName = "SubcaseMonitor";
            var collectionName = "Products";
            var subcase = new Subcase2()
            {
                id = "123456-1",
                title = "aaa"
            };

            try
            {
                await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, subcase.id));
                Console.WriteLine("Found {0}", subcase.id);
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), subcase);
                    Console.WriteLine("Created subcase {0}", subcase.id);
                }
                else
                {
                    throw;
                }
            }

        }
    }
}
