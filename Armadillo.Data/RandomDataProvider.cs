using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Armadillo.Shared;
using Microsoft.AspNetCore.WebUtilities;

namespace Armadillo.Data
{

    public class RandomDataProvider : ISubcaseDataProdiver
    {        
        public Task<IEnumerable<string>> GetProductsAsync()
        {
            var rng = new Random();
            return Task<IEnumerable<string>>.Run(() => {
                var result = Enumerable.Range(1, rng.Next(2, 5)).Select(index => $"Product {index}").ToList();
                result.Add("Product X - No subcases");
                return (IEnumerable<string>)result;
            });
        }

        public Task<IEnumerable<Subcase>> GetSubcasesAsync(string product)
        {
            return Task<IEnumerable<Subcase>>.Run(() => {

                if(product.Contains("No subcases")) 
                {
                    return new List<Subcase>();
                }

                var rng = new Random();
                var max = rng.Next(10, 15);
                var statuses = new[]
                {
                    "New",
                    "Waiting Support Response",
                    "Fix Provided",
                    "Investigating",
                    "Update from Support",
                    "Assigned"
                };
                
                if(rng.Next(0, 10) > 8)
                {
                    throw new ApplicationException("Sample error - something happened...");
                }

                return Enumerable.Range(1, rng.Next(10, 15)).Select(index => new Subcase
                {
                    Id = String.Format("{0}-1", rng.Next(400000, 500000)),
                    Title = "Lorem ipsum dolor sit amet, consectetur adipiscing elit " + rng.Next(100),
                    Level = "" + rng.Next(1, 5),
                    Customer = "Customer " + rng.Next(1, 5),
                    Owner = "Owner " + rng.Next(1, 7),
                    Status = statuses[rng.Next(0, statuses.Length)],
                    Created = DateTime.UtcNow - TimeSpan.FromDays(rng.Next(1, 60)) - TimeSpan.FromMinutes(rng.Next(0, 30)),
                    LastUpdate = DateTime.UtcNow - TimeSpan.FromDays(rng.Next(0, 7)) - TimeSpan.FromMinutes(rng.Next(0, 30)),
                    Loaded = DateTime.UtcNow - TimeSpan.FromMinutes(rng.Next(0, 3))
                });
            });
        }

        public string GetReportLink(string product)
        {
            var template = @"https://www.google.com/search";
            return QueryHelpers.AddQueryString(template, "q", product);
        }
    }
}
