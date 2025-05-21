using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

namespace NeuroApp.Api
{
    public class SensioApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string? _token;

        public SensioApiService(IConfiguration configuration)
        {
            _token = configuration.GetValue<string>("ApiSettings:SensioApiKey");
            _baseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl") ?? 
                throw new ArgumentNullException(nameof(configuration), "Base URL não configurado ou vazio.");

            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false
            };

            _httpClient = new HttpClient(handler);
            if (!string.IsNullOrEmpty(_token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            }
            else
            {
                throw new ArgumentNullException(nameof(configuration), "Token de API não configurado ou vazio.");
            }
        }

        public async Task<string> GetDataAsync(string endpoint)
        {
            try
            {
                var fullUri = $"{_baseUrl}{endpoint}";

                HttpResponseMessage response = await _httpClient.GetAsync(fullUri).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    string? contentType = response.Content.Headers.ContentType?.MediaType;
                    
                    if (contentType != null && contentType.Contains("application/json"))
                    {
                        return content;
                    }
                    else
                    {
                        throw new Exception($"Resposta da API não é JSON. Conteúdo recebido: {content[..500]}");
                    }
                }
                else
                {
                    throw new Exception($"Erro: {response.StatusCode}, {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao acessar a API: {ex.Message}");
            }
        }
    }
}