using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Armadillo.Shared;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Xml;
using System.Globalization;
using HtmlAgilityPack;

namespace Armadillo.Data
{
    enum ReportFormat
    {
        XML,
        HTML
    }

    public class ReportServerDataProvider : ISubcaseDataProdiver
    {
        private readonly ILogger _logger;
        private readonly IReportServerClient _reportServerClient;
        public static string ReportServerUrl = @"http://tfsreports.prod.quest.corp";

        public ReportServerDataProvider(ILogger logger, IReportServerClient reportServerClient)
        {
            _logger = logger;
            _reportServerClient = reportServerClient;
        }

        public async Task<IEnumerable<Subcase>> GetSubcasesAsync(string product)
        {
            var url = GetReportLink(product, ReportFormat.XML);
            var page = await GetReportAsync(url);
            try
            {
                var nodes = XDocument.Parse(page).Root?.Descendants().Elements().Where(e => e.Name.LocalName == "Details").ToArray();
                _logger.LogDebug("Rows: {Count}", nodes?.Length ?? 0);

                return nodes?.Select(e => new Subcase
                {
                    Id = e.Attribute("COL_NUM")?.Value,
                    Title = e.Attribute("COLTITLE")?.Value,
                    Level = e.Attribute("COL_PR")?.Value.Substring("Level ".Length),
                    Owner = e.Attribute("COLOWNER")?.Value,
                    Status = e.Attribute("X_RD_STATUS")?.Value,
                    Customer = e.Attribute("CUSTOMER")?.Value,
                    Created = ParseDateTime(e.Attribute("COLCREATED")?.Value),
                    LastUpdate = ParseDateTime(e.Attribute("STATECHANGE")?.Value),
                    Loaded = DateTime.UtcNow
                });
            }
            catch (XmlException exception)
            {
                const string message = "Cannot parse XML report, incorrect format.";
                _logger.LogError(exception, message);
                throw new ApplicationException(message);
            }
        }

        public string GetReportLink(string product)
        {
            return GetReportLink(product, ReportFormat.HTML);
        }

        private string GetReportLink(string product, ReportFormat format)
        {
            //var template = ReportServerUrl + 
            //    @"/ReportServer?/Siebel/SPB/SLA+Siebel+(SPb)&rs:Command=Render&Location=EMEA-RU-St.%20Petersburg&rs:Format=" + 
            //    (format == ReportFormat.HTML ? "HTML4.0" : "XML") + @"&rc:LinkTarget=_top&rc:Javascript=false";

            var template = ReportServerUrl +
                @"/ReportServer?/Siebel/SPB/SLA+Siebel+(SPb)&rs:Command=Render&Location=AMER-CA-NS-Halifax&rs:Format=" +
                (format == ReportFormat.HTML ? "HTML4.0" : "XML") + @"&rc:LinkTarget=_top&rc:Javascript=false";

            return String.IsNullOrEmpty(product) ? template : QueryHelpers.AddQueryString(template, "Products", product);
        }

        public async Task<IEnumerable<string>> GetProductsAsync()
        {
            var url = GetReportLink(product: null, ReportFormat.HTML);
            var page = await GetReportAsync(url);
            try
            {
                var htmlDoc = new HtmlDocument();	
                htmlDoc.LoadHtml(page);	

                var htmlBody = htmlDoc.DocumentNode;	
                var wrapperNode = htmlBody.SelectSingleNode("//div[@id='ReportViewerControl_ctl04_ctl05_divDropDown']");
                
                if(wrapperNode == null) {	
                    var message = "Cannot parse HTML report, incorrect format.";	
                    _logger.LogError(message);	
                    throw new ApplicationException(message);	
                }

                var nodes = wrapperNode.SelectNodes("table/tr/td/span/label");
                
                // First two elements are "(Select All)" and " All"
                var products = nodes.Select(each => each.InnerText.Replace("&nbsp;", " ")).Skip(2);
                
                _logger.LogDebug("Parsed products: {products}", products);
                return products;

            }
            catch (XmlException exception)
            {
                const string message = "Cannot parse HTML report, incorrect format.";
                _logger.LogError(exception, message);
                throw new ApplicationException(message);
            }
        }

        private async Task<string> GetReportAsync(string url)
        {
            _logger.LogDebug("Loading report {url}", url);

            var uri = new Uri(url);
            return await _reportServerClient.GetReportAsync(url);
        }
        
        private static DateTime ParseDateTime(string value)
        {
            DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result);
            return result;
        }
    }
}
