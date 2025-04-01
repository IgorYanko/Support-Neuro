using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
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
            var cacheKey = $"api_{endpoint}";
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                try
                {
                    var fullUri = $"{_baseUrl}{endpoint}";
                    LoggingService.LogDebug($"Iniciando requisição GET para: {fullUri}");

                    HttpResponseMessage response = await _httpClient.GetAsync(fullUri);

                    if (response.IsSuccessStatusCode)
                    {
                        string contentType = response.Content.Headers.ContentType?.MediaType;
                        
                        if (contentType != null && contentType.Contains("application/json"))
                        {
                            string data = await response.Content.ReadAsStringAsync();
                            LoggingService.LogDebug($"Resposta recebida com sucesso para: {fullUri}");
                            return data;
                        }
                        else
                        {
                            string htmlContent = await response.Content.ReadAsStringAsync();
                            LoggingService.LogWarning($"Resposta não é JSON para: {fullUri}. Conteúdo: {htmlContent.Substring(0, 500)}");

                            throw new Exception($"Resposta da API não é JSON. Conteúdo recebido: {htmlContent.Substring(0, 500)}");
                        }
                    }
                    else
                    {
                        LoggingService.LogError($"Erro na requisição para: {fullUri}. Status: {response.StatusCode}, Razão: {response.ReasonPhrase}");
                        throw new Exception($"Erro: {response.StatusCode}, {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    LoggingService.LogError($"Erro ao obter dados de: {endpoint}", ex);
                    throw;
                }
            }, TimeSpan.FromMinutes(5));
        }

        public async Task InvalidateCacheAsync(string endpoint)
        {
            var cacheKey = $"api_{endpoint}";
            await _cacheService.RemoveAsync(cacheKey);
        }
    }
} 