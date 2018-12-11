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
        private ISubcaseDataProdiver dataProdiver_;

        public SubcasesController(ISubcaseDataProdiver dataProdiver)
        {
            dataProdiver_ = dataProdiver;
        }

        [HttpGet("products")]
        public IEnumerable<string> Products()
        {   
            return dataProdiver_.GetProducts();
        }

        [HttpGet("subcases")]
        public async Task<Product> Subcases(string product)
        {
            Console.WriteLine("Loading subcases for {0}", product);
            var subcases = await dataProdiver_.GetSubcasesAsync(product);
            return new Product()
            {
                Name = product,
                Subcases = subcases.ToArray()
            };
        }
    }
}
