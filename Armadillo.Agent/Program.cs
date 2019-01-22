using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Armadillo.Shared;
using Armadillo.Siebel;

namespace Armadillo.Agent
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddUserSecrets("28e6f711-a4c4-4cef-9e37-50ebfee35f91");
            var configuration = builder.Build();
            var endpointUri = configuration["CosmosDB:EndpointUri"];
            var primaryKey = configuration["CosmosDB:PrimaryKey"];

            Console.WriteLine("Database endpoint {0}, key {1}", endpointUri, primaryKey);
            var documentClient = new DocumentClient(new Uri(endpointUri), primaryKey);

            var dataProvider  = new RandomDataProvider();

            try
            {
                // var uploader = new Uploader(dataProvider, documentClient);
                // uploader.UpdateAsync().Wait();

                var cosmosDataProvider = new CosmosDataProvider(documentClient);
                var subcases = cosmosDataProvider.GetSubcasesAsync("ignored").Result;
                foreach(var subcase in subcases)
                {
                    Console.WriteLine($"Subcase {subcase.Id} {subcase.Title}");
                }
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
        }
    }
}
