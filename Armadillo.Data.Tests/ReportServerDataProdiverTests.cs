using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using Armadillo.Shared;
using Armadillo.Data;
using System.Linq;

namespace Armadillo.Data.Tests
{
    [TestClass]
    public class ReportServerDataProviderTests
    {
        private ReportServerDataProvider dataProvider_;

        [TestInitialize]
        public void SetUp()
        {
            dataProvider_ = new ReportServerDataProvider(null, null);
        }

        [TestMethod]
        public void AllProductNamesAreUnique()
        {
            var reportNames = dataProvider_.GetProducts();
            CollectionAssert.AllItemsAreUnique(reportNames.ToArray());
        }
    }
}
