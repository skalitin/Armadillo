using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Armadillo.Shared;
using Microsoft.Extensions.Logging;

namespace Armadillo.Data
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
        ISubcaseDataProdiver _dataProdiver;
        TimeSpan _refreshTimeout;
        Dictionary<string, CachedSubcases> _cache;
        ILogger _logger;

        public DataProdiverCache(ISubcaseDataProdiver dataProvider, ILogger logger, TimeSpan refreshTimeout)
        {
            _dataProdiver = dataProvider;
            _refreshTimeout = refreshTimeout;
            _logger = logger;
            _cache = new Dictionary<string, CachedSubcases>();
        }

        public IEnumerable<string> GetProducts()
        {
            return _dataProdiver.GetProducts();
        }

        public string GetReportLink(string product)
        {
            return _dataProdiver.GetReportLink(product);
        }

        public async Task<IEnumerable<Subcase>> GetSubcasesAsync(string product)
        {
            var now = DateTime.Now;
            CachedSubcases cachedSubcases = null;
            if(_cache.TryGetValue(product, out cachedSubcases))
            {
                _logger.LogDebug("Cached subcases found for {product}", product);
                if(now - cachedSubcases.RequestTime < _refreshTimeout)
                {
                    _logger.LogDebug("Return cached subcases for {product}", product);
                    return cachedSubcases.Subcases;
                }

                _logger.LogDebug("Cache expired for {product}", product);
            }

            _logger.LogDebug("Updating cache for {product}...", product);
            var subcases = await _dataProdiver.GetSubcasesAsync(product);

            _logger.LogDebug("Cache updated for {product}", product);
            _cache[product] = new CachedSubcases(subcases);

            return subcases;
        }
    }
}
