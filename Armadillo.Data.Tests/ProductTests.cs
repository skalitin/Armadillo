using Armadillo.Shared;
using NUnit.Framework;
using System;

namespace Armadillo.Data.Tests
{
    [TestFixture]
    public class ProductTests
    {
        [Test]
        public void CalculateLoadedTimeWithNullSubcases()
        {
            var product = new Product();
            Assert.AreEqual(DateTime.MinValue, product.Loaded);
            Assert.IsFalse(product.HasLoadedTime);
        }

        [Test]
        public void CalculateLoadedTimeWithoutSubcases()
        {
            var product = new Product();
            product.Subcases = new Subcase[] {};
            Assert.AreEqual(DateTime.MinValue, product.Loaded);
            Assert.IsFalse(product.HasLoadedTime);
        }

        [Test]
        public void CalculateLoadedTimeBasedOnFirstSubcase()
        {
            var product = new Product();
            product.Subcases = new Subcase[]
            {
                new Subcase() { Loaded = DateTime.Parse("10.10.2019 10:10")},
                new Subcase() { Loaded = DateTime.Parse("10.10.2019 10:15")}
            };

            Assert.AreEqual(DateTime.Parse("10.10.2019 10:10"), product.Loaded);
            Assert.IsTrue(product.HasLoadedTime);
        }
    }
}