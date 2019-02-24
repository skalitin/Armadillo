using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using Armadillo.Shared;
using Armadillo.Data;


namespace Armadillo.Data.Tests
{
    [TestClass]
    public class ReportServerDataProviderTests
    {
        private ReportServerDataProvider dataProvider_;
        private static ILogger logger_;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var loggerFactory = new LoggerFactory();
            logger_ = loggerFactory.CreateLogger("Test");
        }

        [TestInitialize]
        public void SetUp()
        {
            dataProvider_ = new ReportServerDataProvider(logger_, null);
        }

        [TestMethod]
        public void AllProductNamesAreUnique()
        {
            var reportNames = dataProvider_.GetProducts();
            CollectionAssert.AllItemsAreUnique(reportNames.ToArray());
        }

        [TestMethod]
        public async Task ParseReportPage()
        {
            var result = await dataProvider_.GetSubcasesAsync("MyProduct");
            Assert.IsNotNull(result);
        }
    }
}
