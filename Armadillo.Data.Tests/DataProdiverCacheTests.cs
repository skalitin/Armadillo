using NUnit.Framework;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Armadillo.Shared;
using Armadillo.Data;
using Moq;

namespace Armadillo.Data.Tests
{
    [TestFixture]
    public class DataProdiverCacheTests
    {
        private Mock<ISubcaseDataProdiver> _mockDataProvider;
        private DataProdiverCache _dataProviderCache;

        [SetUp]
        public void SetUp()
        {
            _mockDataProvider = new Mock<ISubcaseDataProdiver>();
            _dataProviderCache = new DataProdiverCache(_mockDataProvider.Object, null, TimeSpan.FromSeconds(1));
        }

        // [Test]
        // public void ProductListIsNotCached()
        // {
        //     var products = new[]{ "product one", "product two" };
        //     _mockDataProvider
        //         .Setup(o => o.GetProducts())
        //         .Returns(products);
            
        //     var result = _dataProviderCache.GetProducts();
        //     Assert.AreEqual(products, result);
        // }
    }
}
