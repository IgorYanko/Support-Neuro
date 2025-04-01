using System.Threading.Tasks;

namespace NeuroApp.Interfaces
{
    public interface IApiService
    {
        Task<string> GetDataAsync(string endpoint);
    }
} 