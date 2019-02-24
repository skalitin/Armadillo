using System;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using HtmlAgilityPack;
using Armadillo.Shared;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Armadillo.Data
{
    public class ReportServerDataProvider : ISubcaseDataProdiver
    {
        private readonly ILogger logger_;
        private readonly IHttpClientFactory htmlClientFactory_;

        public ReportServerDataProvider(ILogger logger, IHttpClientFactory factory)
        {
            logger_ = logger;
            htmlClientFactory_ = factory;
        }

        public async Task<IEnumerable<Subcase>> GetSubcasesAsync(string product)
        {
            var url = GetReportLink(product);
            var page = await GetPageAsync(url);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(page);

            var htmlBody = htmlDoc.DocumentNode;
            var tableNode = htmlBody.SelectSingleNode("//table[@class='a209']");
            var rowNodes = tableNode.SelectNodes("tr");
                        
            logger_.LogDebug("Rows: {Count}", rowNodes.Count);

            var subcases = new List<Subcase>();
            int i = 0;
            foreach(var node in rowNodes)
            {
                i++;
                if(i <= 2) continue;
                var cells = node.SelectNodes("td");
                var subcase = new Subcase()
                {
                    Id = cells[1].InnerText,
                    Title = cells[2].InnerText,
                    Level = cells[3].InnerText.Substring("Level ".Length),
                    Owner = cells[4].InnerText,
                    Status = cells[8].InnerText,
                    Customer = cells[9].InnerText,
                    Loaded = DateTime.UtcNow
                };
                
                subcases.Add(subcase);
            }

            return subcases;
        }
        public string GetReportLink(string product)
        {
            var template = @"http://tfsreports.prod.quest.corp/ReportServer?/Siebel/SPB/SLA+Siebel+(SPb)&rs:Command=Render&Location=EMEA-RU-St.%20Petersburg&rs:Format=HTML4.0&rc:LinkTarget=_top&rc:Javascript=false&rc:Toolbar=false";
            return QueryHelpers.AddQueryString(template, "Products", product);
        }

        public IEnumerable<string> GetProducts()
        {
            return new[]
            {
                "Recovery Manager for AD",
                "Recovery Manager for Exchange",
                "OnDemand Migration for Email",
                "InTrust",
                "Migration Mgr for AD",
                "On Demand Migrations",
                "On Demand Recovery"
            };
        }

        private async Task<string> GetPageAsync(string url)
        {
            logger_.LogDebug("Loading report {url}", url);

            var uri = new Uri(url);

            // using(var client = htmlClientFactory_.CreateClient())
            // {
            //     client.BaseAddress = uri;
            //     return await client.GetStringAsync(url);
            // }

            var credentialsCache = new CredentialCache { { uri, "NTLM", CredentialCache.DefaultNetworkCredentials } };
            var handler = new HttpClientHandler { Credentials = credentialsCache };
            
            using(var httpClient = new HttpClient(handler) { BaseAddress = uri  })
            {
                return await httpClient.GetStringAsync(url);
            }
        }

    }

}