using System.Threading.Tasks;

namespace Armadillo.Data
{
    public interface IReportServerClient
    {
        Task<string> GetReportAsync(string uri);
    }
}
