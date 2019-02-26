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

        [Test, TestCaseSource("SampleReport")]
        public async Task ParseReportPage(string report)
        {
            _mockReportClient
                .Setup(o => o.GetReportAsync(It.IsAny<string>()))
                .ReturnsAsync(report);

            var result = await _dataProvider.GetSubcasesAsync("MyProduct");
            Assert.IsNotNull(result);
        }

        public static string[] SampleReport => new[] { File.ReadAllText(@"Resources\SampleReport.html") };
    }
}
