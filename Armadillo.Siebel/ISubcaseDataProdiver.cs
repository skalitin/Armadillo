using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Armadillo.Shared;

namespace Armadillo.Siebel
{
    public interface ISubcaseDataProdiver
    {
        Task<IEnumerable<Subcase>> GetSubcasesAsync(string product);
        IEnumerable<Subcase> GetSubcases(string product);
        IEnumerable<string> GetProducts();
    }
}
