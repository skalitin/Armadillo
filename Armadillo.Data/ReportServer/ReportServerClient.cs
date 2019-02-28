using System.Net.Http;
using System.Threading.Tasks;

namespace Armadillo.Data
{
    public class ReportServerClient : IReportServerClient
    {
        private readonly HttpClient _httpClient;

        public ReportServerClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetReportAsync(string uri)
        {
            return await _httpClient.GetStringAsync(uri);
        }
    }
}
