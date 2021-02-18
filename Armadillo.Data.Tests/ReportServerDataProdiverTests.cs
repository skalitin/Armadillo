using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Moq;
using Armadillo.Shared;
using Armadillo.Data;
using System.Globalization;

namespace Armadillo.Data.Tests
{
    [TestFixture]
    public class ReportServerDataProviderTests
    {
        private ReportServerDataProvider _dataProvider;
        private static ILogger _logger;
        private Mock<IReportServerClient> _mockReportClient;

        [OneTimeSetUp]
        public static void ClassInitialize()
        {
            var loggerFactory = new LoggerFactory();
            _logger = loggerFactory.CreateLogger("Test");
        }

        [SetUp]
        public void SetUp()
        {
            _mockReportClient = new Mock<IReportServerClient>();
            _dataProvider = new ReportServerDataProvider(_logger, _mockReportClient.Object);
        }

        [Test]
        public async Task ParsingReport()
        {
            var report = GetReport("Report.xml");
            _mockReportClient
                .Setup(o => o.GetReportAsync(It.IsAny<string>()))
                .ReturnsAsync(report);

            var subcases = (await _dataProvider.GetSubcasesAsync("MyProduct")).ToArray();
            Assert.AreEqual(3, subcases.Length);

            var subcase = subcases.First();
            Assert.AreEqual("4385211-1", subcase.Id);
            Assert.AreEqual("John Doe", subcase.Owner);
            Assert.AreEqual("Sample Customer", subcase.Customer);
            Assert.AreEqual("Sample Title", subcase.Title);
            Assert.AreEqual("Waiting Support Response", subcase.Status);
            Assert.AreEqual("2", subcase.Level);
            Assert.AreEqual(DateTime.ParseExact("10.12.18 06:22", "dd.MM.yy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None), subcase.Created);
            Assert.AreEqual(DateTime.ParseExact("14.02.19 14:20", "dd.MM.yy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None), subcase.LastUpdate);
        }

        [Test]
        public async Task ParsingEmptyReport()
        {
            var report = GetReport("EmptyReport.xml");
            _mockReportClient
                .Setup(o => o.GetReportAsync(It.IsAny<string>()))
                .ReturnsAsync(report);

            var subcases = (await _dataProvider.GetSubcasesAsync("MyProduct")).ToArray();
            Assert.AreEqual(0, subcases.Length);
        }

        [Test]
        public async Task ParsingPageSourceToGetReportList()
        {
            var report = GetReport("PageSource.html");
            _mockReportClient
                .Setup(o => o.GetReportAsync(It.IsAny<string>()))
                .ReturnsAsync(report);

            var products = await _dataProvider.GetProductsAsync();
            Assert.AreEqual("AMER-CA-NS-Halifax | InTrust", products.First());
            CollectionAssert.Contains(products, "AMER-CA-NS-Halifax | Recovery Manager for AD");
        }

        private string GetReport(string name)
        {
            return File.ReadAllText($"Resources\\{name}");
        }
    }
}
