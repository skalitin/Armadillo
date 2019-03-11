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

namespace Armadillo.Data
{
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
            var url = GetReportLink(product);
            var page = await GetReportAsync(url);
            try
            {
                var nodes = XDocument.Parse(page).Root?.Descendants().Elements()
                    .Where(e => e.Name.LocalName == "Details").ToArray();
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
                const string message = "Cannot parse HTML report, incorrect format.";
                _logger.LogError(exception, message);
                throw new ApplicationException(message);
            }
        }

        public string GetReportLink(string product)
        {
            var template = ReportServerUrl + @"/ReportServer?/Siebel/SPB/SLA+Siebel+(SPb)&rs:Command=Render&Location=EMEA-RU-St.%20Petersburg&rs:Format=xml&rc:LinkTarget=_top&rc:Javascript=false&rc:Toolbar=false";
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
                "IT Search",
                "LiteSpeed for SQL Server",
                "Migration Mgr for AD",
                "On Demand Migrations",
                "On Demand Recovery"
            };
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
