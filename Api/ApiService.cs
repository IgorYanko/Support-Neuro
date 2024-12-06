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
        private readonly string _baseUrl;
        private readonly string _token;

        public SensioApiService(IConfiguration configuration)
        {
            _token = configuration.GetValue<string>("ApiSettings:SensioApiKey");
            _baseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl");

            if (string.IsNullOrEmpty(_baseUrl))
            {
                throw new ArgumentNullException(nameof(_baseUrl), "Base URL não configurado ou está vazio.");
            }

            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false
            };

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }

        public async Task<string> GetDataAsync(string endpoint)
        {
            try
            {
                var fullUri = $"{_baseUrl}{endpoint}";

                HttpResponseMessage response = await _httpClient.GetAsync(fullUri);

                if (response.IsSuccessStatusCode)
                {
                    string contentType = response.Content.Headers.ContentType?.MediaType;
                    
                    if (contentType != null && contentType.Contains("application/json"))
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        return data;
                    }
                    else
                    {
                        string htmlContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Resposta não é JSON: {htmlContent}");

                        throw new Exception($"Resposta da API não é JSON. Conteúdo recebido: {htmlContent.Substring(0, 500)}");
                    }
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