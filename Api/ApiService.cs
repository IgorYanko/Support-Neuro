using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace NeuroApp.Api
{
    public class SensioApiService
    {
        private readonly HttpClient _httpClient;

        public SensioApiService(IConfiguration configuration)
        {
            var apiSettings = configuration.GetSection("AppSettings");
            string token = apiSettings.GetValue<string>("SensioApiKey");
            string baseUrl = apiSettings.GetValue<string>("BaseUrl");

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("baseUrl")
            };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<string> GetDataAsync(string endpoint)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    return data;
                }
                else
                {
                    throw new Exception($"Erro: {response.StatusCode}, {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter dados: {ex.Message}");
                throw;
            }
        }
    }
}