using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Armadillo.Shared;

namespace Armadillo.Siebel
{
    public interface ISubcaseDataProdiver
    {
        IEnumerable<string> GetProducts();
        string GetReportLink(string product);
        Task<IEnumerable<Subcase>> GetSubcasesAsync(string product);
    }
}
