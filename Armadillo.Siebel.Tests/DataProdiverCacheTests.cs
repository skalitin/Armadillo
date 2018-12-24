using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using Armadillo.Shared;

namespace Armadillo.Siebel.Tests
{
    class TestDataProvider : ISubcaseDataProdiver
    {
        public IEnumerable<string> GetProducts()
        {
            return new string[]{};
        }

        public Task<IEnumerable<Subcase>> GetSubcasesAsync(string product)
        {
            return Task<IEnumerable<Subcase>>.Run(() =>
            {
                return (IEnumerable<Subcase>)new List<Subcase>();
            });
        }
    }

    [TestClass]
    public class DataProdiverCacheTests
    {
        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
