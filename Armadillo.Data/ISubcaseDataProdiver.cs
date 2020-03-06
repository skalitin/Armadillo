using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Armadillo.Shared;

namespace Armadillo.Data
{
    public interface ISubcaseDataProdiver
    {
        Task<IEnumerable<string>> GetProductsAsync();
        Task<IEnumerable<Subcase>> GetSubcasesAsync(string product);
        string GetReportLink(string product);
    }
}
