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
                "Product One",
                "Product Two"
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
                var statuses = new[]
                {
                    "Waiting Support Response",
                    "Fix Provided",
                    "Investigating",
                    "Update from Support",
                    "Assigned"
                };
                return Enumerable.Range(1, rng.Next(10, 15)).Select(index => new Subcase
                {
                    Id = String.Format("{0}-1", rng.Next(405000, 405999)),
                    Title = "Lorem ipsum dolor sit amet, consectetur adipiscing elit " + rng.Next(100),
                    Level = "" + rng.Next(1, 4),
                    Customer = "Customer " + rng.Next(1, 5),
                    Owner = "Owner " + rng.Next(1, 7),
                    Status = statuses[rng.Next(0, statuses.Length)]
                });
            });
        }
    }
}
