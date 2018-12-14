using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Armadillo.Shared;

namespace Armadillo.Siebel
{
    class CachedSubcases
    {
        public CachedSubcases(IEnumerable<Subcase> subcases)
        {
            Subcases = subcases;
            RequestTime = DateTime.Now;
        }

        public IEnumerable<Subcase> Subcases { get; private set; }
        public DateTime RequestTime {get; private set;}
    }

    public class DataProdiverCache : ISubcaseDataProdiver
    {
        ISubcaseDataProdiver dataProdiver_;
        TimeSpan cacheTime_;
        Dictionary<string, CachedSubcases> cache_;

        public DataProdiverCache(ISubcaseDataProdiver dataProvider, TimeSpan cacheTime)
        {
            dataProdiver_ = dataProvider;
            cacheTime_ = cacheTime;
            cache_ = new Dictionary<string, CachedSubcases>();
        }

        public IEnumerable<string> GetProducts()
        {
            return dataProdiver_.GetProducts();
        }

        public Task<IEnumerable<Subcase>> GetSubcasesAsync(string product)
        {
            return dataProdiver_.GetSubcasesAsync(product);
        }
    }
}
