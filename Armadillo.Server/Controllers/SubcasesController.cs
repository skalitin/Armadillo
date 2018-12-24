using Armadillo.Shared;
using Armadillo.Siebel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Armadillo.Server.Controllers
{
    [Route("api")]
    public class SubcasesController : Controller
    {
        private ISubcaseDataProdiver dataProdiver_;
        private ILogger<SubcasesController> logger_;

        public SubcasesController(ISubcaseDataProdiver dataProdiver, ILogger<SubcasesController> logger)
        {
            dataProdiver_ = dataProdiver;
            logger_ = logger;
        }

        [HttpGet("products")]
        public IEnumerable<string> Products()
        {   
            logger_.LogInformation("Loading products");
            return dataProdiver_.GetProducts();
        }

        [HttpGet("subcases")]
        public async Task<Product> Subcases(string product)
        {
            logger_.LogInformation("Loading subcases for {0}", product);
            var subcases = await dataProdiver_.GetSubcasesAsync(product);
            return new Product()
            {
                Name = product,
                Subcases = subcases.ToArray()
            };
        }
    }
}
