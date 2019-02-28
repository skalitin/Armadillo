using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Logging;
using Moq;
using Armadillo.Shared;
using Armadillo.Data;


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
        public void AllProductNamesAreUnique()
        {
            var reportNames = _dataProvider.GetProducts();
            CollectionAssert.AllItemsAreUnique(reportNames.ToArray());
        }

        [Test]
        public async Task ParsingReportPage()
        {
            var report = GetReport("Report");
            _mockReportClient
                .Setup(o => o.GetReportAsync(It.IsAny<string>()))
                .ReturnsAsync(report);

            var subcases = (await _dataProvider.GetSubcasesAsync("MyProduct")).ToArray();
            Assert.AreEqual(2, subcases.Length);

            var subcase = subcases.First();
            Assert.AreEqual("4385211-1", subcase.Id);
            Assert.AreEqual("John Doe", subcase.Owner);
            Assert.AreEqual("Sample Customer", subcase.Customer);
            Assert.AreEqual("Sample Title", subcase.Title);
            Assert.AreEqual("Waiting Support Response", subcase.Status);
            Assert.AreEqual("2", subcase.Level);
        }

        [Test]
        public async Task ParsingEmptyReportPage()
        {
            var report = GetReport("EmptyReport");
            _mockReportClient
                .Setup(o => o.GetReportAsync(It.IsAny<string>()))
                .ReturnsAsync(report);

            var subcases = (await _dataProvider.GetSubcasesAsync("MyProduct")).ToArray();
            Assert.AreEqual(0, subcases.Length);
        }

        private string GetReport(string name)
        {
            return File.ReadAllText($"Resources\\{name}.html");
        }
    }
}
