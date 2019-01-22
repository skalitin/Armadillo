using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Armadillo.Shared;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Armadillo.Siebel
{
    public class CosmosDataProvider : ISubcaseDataProdiver
    {
        private DocumentClient documentClient_;
        private readonly string DatabaseName = "SubcaseMonitor";
        private readonly string CollectionName = "Subcases";

        public CosmosDataProvider(DocumentClient documentClient)
        {
            documentClient_ = documentClient;
        }

        public IEnumerable<string> GetProducts()
        {
            return new[]
            {
                "Product One",
                "Product Two"
            };
        }
        public string GetReportLink(string product)
        {
            var template = @"http://tfsreports.prod.quest.corp/ReportServer?/Siebel/SPB/SLA+Siebel+(SPb)&rs:Command=Render&Location=EMEA-RU-St.%20Petersburg&rs:Format=HTML4.0&rc:LinkTarget=_top&rc:Javascript=false&rc:Toolbar=false";
            return QueryHelpers.AddQueryString(template, "Products", product);
        }

        public Task<IEnumerable<Subcase>> GetSubcasesAsync(string product)
        {
            return Task<IEnumerable<Subcase>>.Run(() => {
                var query = documentClient_.CreateDocumentQuery<Subcase>(
                    UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), new FeedOptions { MaxItemCount = -1 });
                return query as IEnumerable<Subcase>;
            });
        }
    }
}
