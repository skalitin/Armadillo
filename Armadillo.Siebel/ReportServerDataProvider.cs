using System;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using HtmlAgilityPack;
using Armadillo.Shared;

namespace Armadillo.Siebel
{
    public class ReportServerDataProvider : ISubcaseDataProdiver
    {
        private string url_;

        public ReportServerDataProvider()
        {
            //  url = @"https://tfsreports.webapps.quest.com/ReportServer?/Siebel/SPB/SLA+Siebel+(SPb)&rs:Command=Render&Location=EMEA-RU-St.%20Petersburg&Products=Recovery%20Manager%20for%20AD&rs:Format=HTML4.0&rc:LinkTarget=_top&rc:Javascript=false&rc:Toolbar=false";
            url_ = @"http://tfsreports.prod.quest.corp/ReportServer?/Siebel/SPB/SLA+Siebel+(SPb)&rs:Command=Render&Location=EMEA-RU-St.%20Petersburg&Products=Recovery%20Manager%20for%20AD&rs:Format=HTML4.0&rc:LinkTarget=_top&rc:Javascript=false&rc:Toolbar=false";
        }

        public async Task<IEnumerable<Subcase>> GetSubcasesAsync(string product)
        {
            var page = "";
            try
            {
                page = await GetPageAsync(url_);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new Subcase[]{};
            }
                        
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(page);

            var htmlBody = htmlDoc.DocumentNode;
            var tableNode = htmlBody.SelectSingleNode("//table[@class='a209']");
            var rowNodes = tableNode.SelectNodes("tr");
                        
            Console.WriteLine("Rows: {0}", rowNodes.Count);
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
                    //Level = cells[3].InnerText,
                    Level = "3",
                    Owner = cells[4].InnerText,
                    Status = cells[7].InnerText,
                    Customer = cells[9].InnerText
                };
                
                subcases.Add(subcase);
            }

            return subcases;
        }

        public IEnumerable<Subcase> GetSubcases(string product)
        {
            return GetSubcasesAsync(product).Result;
        }

        public IEnumerable<string> GetProducts()
        {
            return new[]
            {
                "Recovery Manager for AD",
                "Recovery Manager for Exchange",
                "InTrust",
                "Migration Mgr for AD",
                "On Demand Migrations",
                "OnDemand Migration for Email",
                "All"
            };
        }

        private async Task<string> GetPageAsync(string url)
        {
            var uri = new Uri(url);
            var credentialsCache = new CredentialCache { { uri, "NTLM", CredentialCache.DefaultNetworkCredentials } };
            var handler = new HttpClientHandler { Credentials = credentialsCache };
            
            Console.WriteLine("Sending request...");
            using(var httpClient = new HttpClient(handler) { BaseAddress = uri  })
            {
                return await httpClient.GetStringAsync(url);
            }
        }

    }

}
