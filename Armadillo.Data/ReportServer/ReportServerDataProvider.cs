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

        public async Task<IEnumerable<Subcase>> GetSubcasesAsync(string input)
        {
            ParseProduct(input, out (string location, string product) result);
            var url = GetReportLink(result.location, result.product, ReportFormat.XML);
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

        public string GetReportLink(string input)
        {
            ParseProduct(input, out (string location, string product) result);
            return GetReportLink(result.location, result.product, ReportFormat.HTML);
        }

        private string GetReportLink(string location, string product, ReportFormat format)
        {
            var url = ReportServerUrl +
                @"/ReportServer?/Siebel/SPB/SLA+Siebel+(SPb)&rs:Command=Render&rs:Format=" +
                (format == ReportFormat.HTML ? "HTML4.0" : "XML") + @"&rc:LinkTarget=_top&rc:Javascript=false";

            url = String.IsNullOrEmpty(product)  ? url : QueryHelpers.AddQueryString(url, "Products", product);
            url = String.IsNullOrEmpty(location) ? url : QueryHelpers.AddQueryString(url, "Location", location);

            return url;
        }

        private async Task<IEnumerable<string>> GetLocationsAsync()
        {
            var url = GetReportLink(location: null, product: null, ReportFormat.HTML);
            var page = await GetReportAsync(url);
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(page);
                var htmlBody = htmlDoc.DocumentNode;

                var locationsWrapperNode = htmlBody.SelectSingleNode("//div[@id='ReportViewerControl_ctl04_ctl03']");
                if (locationsWrapperNode == null)
                {
                    var message = "Cannot parse HTML report, incorrect format.";
                    _logger.LogError(message);
                    throw new ApplicationException(message);
                }

                // First element is  "(Select Value)"
                var locationNodes = locationsWrapperNode.SelectNodes("select/option");
                var locations = locationNodes.Select(each => each.InnerText.Replace("&nbsp;", " ")).Skip(1);
                _logger.LogDebug("Parsed locations: {locations}", locations);

                return locations;
            }
            catch (XmlException exception)
            {
                const string message = "Cannot parse HTML report, incorrect format.";
                _logger.LogError(exception, message);
                throw new ApplicationException(message);
            }
        }

        private async Task<IEnumerable<string>> GetProductsAsync(string location)
        {
            var url = GetReportLink(location, product: null, ReportFormat.HTML);
            var page = await GetReportAsync(url);
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(page);
                var htmlBody = htmlDoc.DocumentNode;

                var productsWrapperNode = htmlBody.SelectSingleNode("//div[@id='ReportViewerControl_ctl04_ctl05_divDropDown']");
                if (productsWrapperNode == null)
                {
                    var message = "Cannot parse HTML report, incorrect format.";
                    _logger.LogError(message);
                    throw new ApplicationException(message);
                }

                // First two elements are "(Select All)" and " All"
                var productNodes = productsWrapperNode.SelectNodes("table/tr/td/span/label");
                var products = productNodes.Select(each => each.InnerText.Replace("&nbsp;", " ")).Skip(2);
                _logger.LogDebug("Parsed {location} products: {products}", location, products);

                return products;
            }
            catch (XmlException exception)
            {
                const string message = "Cannot parse HTML report, incorrect format.";
                _logger.LogError(exception, message);
                throw new ApplicationException(message);
            }
        }

        public async Task<IEnumerable<string>> GetProductsAsync()
        {
            var products = new List<string>();
            var locations = await GetLocationsAsync();
            foreach(var location in locations)
            {
                foreach(var product in await GetProductsAsync(location))
                {
                    products.Add($"{location} | {product}");
                }
            }
            
            _logger.LogDebug("All products: {products}", products);
            return products;
        }

        private async Task<string> GetReportAsync(string url)
        {
            _logger.LogDebug("Loading report {url}", url);
            return await _reportServerClient.GetReportAsync(url);
        }
        
        private static DateTime ParseDateTime(string value)
        {
            DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result);
            return result;
        }

        private static void ParseProduct(string input, out (string location, string product) result)
        {
            var parts = input.Split('|');
            if (parts.Length == 2)
            {
                result.location = parts[0].Trim();
                result.product = parts[1].Trim();
            }
            else
            {
                result.location = result.product = null;
            }
        }
    }
}
