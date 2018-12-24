using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Armadillo.Shared;
using Microsoft.Extensions.Logging;

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
        TimeSpan refreshTimeout_;
        Dictionary<string, CachedSubcases> cache_;
        ILogger logger_;

        public DataProdiverCache(ISubcaseDataProdiver dataProvider, ILogger logger, TimeSpan refreshTimeout)
        {
            dataProdiver_ = dataProvider;
            refreshTimeout_ = refreshTimeout;
            logger_ = logger;
            cache_ = new Dictionary<string, CachedSubcases>();
        }

        public IEnumerable<string> GetProducts()
        {
            return dataProdiver_.GetProducts();
        }

        public async Task<IEnumerable<Subcase>> GetSubcasesAsync(string product)
        {
            var now = DateTime.Now;
            CachedSubcases cachedSubcases = null;
            if(cache_.TryGetValue(product, out cachedSubcases))
            {
                logger_.LogDebug("Cached subcases found for {product}", product);
                if(now - cachedSubcases.RequestTime < refreshTimeout_)
                {
                    logger_.LogDebug("Return cached subcases for {product}", product);
                    return cachedSubcases.Subcases;
                }

                logger_.LogDebug("Cache expired for {product}", product);
            }

            logger_.LogDebug("Updating cache for {product}...", product);
            var subcases = await dataProdiver_.GetSubcasesAsync(product);

            logger_.LogDebug("Cache updated for {product}", product);
            cache_[product] = new CachedSubcases(subcases);

            return subcases;
        }
    }
}
