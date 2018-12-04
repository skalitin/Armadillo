using Armadillo.Shared;
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
        [HttpGet("[action]")]
        public IEnumerable<Subcase> Subcases()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new Subcase
            {
                Id = String.Format("{0}", index),
                Title = "Test title",
                // Level = rng.Next(5),
                Customer = "",
                Owner = "",
                Status = ""
            });
        }
    }
}
