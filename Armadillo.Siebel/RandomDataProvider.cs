using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Armadillo.Shared;

namespace Armadillo.Siebel
{
    public class RandomDataProvider : ISubcaseDataProdiver
    {
        public IEnumerable<string> GetProducts()
        {
            return new[]
            {
                "Recovery Manager for AD",
                "Recovery Manager for Exchange"
            };
        }

        public IEnumerable<Subcase> GetSubcases(string product)
        {
            return GetSubcasesAsync(product).Result;
        }

        public Task<IEnumerable<Subcase>> GetSubcasesAsync(string product)
        {
            return Task<IEnumerable<Subcase>>.Run(() => {
                var rng = new Random();
                var max = rng.Next(10, 15);
                return Enumerable.Range(1, max).Select(index => new Subcase
                {
                    Id = String.Format("{0}-1", rng.Next(405000, 405999)),
                    Title = "Lorem ipsum dolor sit amet, consectetur adipiscing elit " + rng.Next(100),
                    Level = "" + rng.Next(1, 4),
                    Customer = "Customer " + rng.Next(1, 5),
                    Owner = "Owner " + rng.Next(1, 7),
                    Status = "Status " + rng.Next(1, 3),
                });
            });
        }
    }
}
