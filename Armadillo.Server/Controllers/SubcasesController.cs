using Armadillo.Shared;
using Armadillo.Siebel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Armadillo.Server.Controllers
{
    [Route("api")]
    public class SubcasesController : Controller
    {
        [HttpGet("products")]
        public IEnumerable<string> Products()
        {   
            return new[] {
                "Recovery Manager for AD",
                "ODME",
                "Migration Manager for AD"
            };
        }
        
        [HttpGet("subcases")]
        public Product Subcases(string productName)
        {
            var subcases = new ReportServerAccessor().GetSubcasesAsync().Result;

            // var rng = new Random();
            // var max = rng.Next(10, 15);
            // var subcases = Enumerable.Range(1, max).Select(index => new Subcase
            // {
            //     Id = String.Format("{0}-1", rng.Next(405000, 405999)),
            //     Title = "Lorem ipsum dolor sit amet, consectetur adipiscing elit " + rng.Next(100),
            //     Level = "" + rng.Next(1, 4),
            //     Customer = "Customer " + rng.Next(1, 5),
            //     Owner = "Owner " + rng.Next(1, 7),
            //     Status = "Status " + rng.Next(1, 3),
            // });
            
            productName = String.IsNullOrEmpty(productName) ? "RMAD" : productName;
            return new Product()
            {
                Name = productName,
                Subcases = subcases.ToArray()
            };
        }
    }
}
