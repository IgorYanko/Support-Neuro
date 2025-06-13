using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Configuration;
using NeuroApp.Interfaces;

namespace NeuroApp.Services
{
    public class SensioApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _token;
        private readonly ICacheService _cacheService;

        public SensioApiService(IConfiguration configuration, ICacheService cacheService)
        {
            _token = configuration.GetValue<string>("ApiSettings:SensioApiKey");
            _baseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl");
            _cacheService = cacheService;

            if (string.IsNullOrEmpty(_baseUrl))
            {
                LoggingService.LogError("Base URL não configurado ou está vazio.");
                throw new ArgumentNullException(nameof(_baseUrl), "Base URL não configurado ou está vazio.");
            }

            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false
            };

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            
            LoggingService.LogInformation($"SensioApiService inicializado com base URL: {_baseUrl}");
        }

        public async Task<string> GetDataAsync(string endpoint)
        {
            string cacheKey = $"api_cache_{endpoint}";

            var cachedData = await _cacheService.GetAsync<string>(cacheKey);
            if (cachedData != null)
            {
                return cachedData;
            }

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

                        await _cacheService.SetAsync(cacheKey, data, TimeSpan.FromMinutes(5));

                        return data;
                    }
                    else
                    {
                        string htmlContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Resposta não é JSON para: {fullUri}. Conteúdo: {htmlContent.Substring(0, Math.Min(500, htmlContent.Length))}");

                        throw new Exception($"Resposta da API não é JSON. Conteúdo recebido: {htmlContent.Substring(0, Math.Min(500, htmlContent.Length))}");
                    }
                }
                else
                {
                    MessageBox.Show($"Erro na requisição para: {fullUri}. Status: {response.StatusCode}, Razão: {response.ReasonPhrase}");
                    throw new Exception($"Erro: {response.StatusCode}, {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao obter dados de {endpoint}: {ex}");
                throw;
            }
        }

        public async Task InvalidateCacheAsync(string endpoint)
        {
            var cacheKey = $"api_{endpoint}";
            await _cacheService.RemoveAsync(cacheKey);
        }
    }
} 