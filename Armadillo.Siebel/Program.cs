using System;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Armadillo.Siebel
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = @"http://tfsreports.prod.quest.corp/ReportServer?/Siebel/SPB/SLA+Siebel+(SPb)&rs:Command=Render&Location=EMEA-RU-St.%20Petersburg&Products=Recovery%20Manager%20for%20AD&rs:Format=HTML4.0&rc:LinkTarget=_top&rc:Javascript=false&rc:Toolbar=false";
            // var url = @"https://tfsreports.webapps.quest.com/ReportServer?/Siebel/SPB/SLA+Siebel+(SPb)&rs:Command=Render&Location=EMEA-RU-St.%20Petersburg&Products=Recovery%20Manager%20for%20AD&rs:Format=HTML4.0&rc:LinkTarget=_top&rc:Javascript=false&rc:Toolbar=false";

            var page = "";
            try
            {
                page = GetPageAsync(url).Result;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }
            // Console.WriteLine(page);
            
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(page);
            var htmlBody = htmlDoc.DocumentNode;
            var test = htmlBody.SelectSingleNode("//table[@class='a209']");
            //var test = htmlBody.SelectSingleNode("//table");

            Console.WriteLine(test.OuterHtml);

            // HtmlWeb web = new HtmlWeb();
            // var htmlDoc = web.Load(html);
            // var node = htmlDoc.DocumentNode.SelectSingleNode("//head/title");
            // Console.WriteLine("Node Name: " + node.Name + "\n" + node.OuterHtml);
        }

        static async Task<string> GetPageAsync(string url)
        {
            var uri = new Uri(url);
            var credentialsCache = new CredentialCache { { uri, "NTLM", CredentialCache.DefaultNetworkCredentials } };
            var handler = new HttpClientHandler { Credentials = credentialsCache };
            using(var httpClient = new HttpClient(handler) { BaseAddress = uri  })
            {
                return await httpClient.GetStringAsync(url);
            }
        }
    }

}
