using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Armadillo.Shared;

namespace Armadillo.Data.Tests
{
    class TestDataProvider : ISubcaseDataProdiver
    {
        public IEnumerable<string> GetProducts()
        {
            return new string[]{};
        }


        public string GetReportLink(string product)
        {
            return "";
        }

        public Task<IEnumerable<Subcase>> GetSubcasesAsync(string product)
        {
            return Task<IEnumerable<Subcase>>.Run(() =>
            {
                return (IEnumerable<Subcase>)new List<Subcase>();
            });
        }
    }

    [TestFixture]
    public class DataProdiverCacheTests
    {
        [Test]
        public void TestMethod1()
        {
            // spike
            Assert.AreEqual("test", "test");
        }
    }
}
