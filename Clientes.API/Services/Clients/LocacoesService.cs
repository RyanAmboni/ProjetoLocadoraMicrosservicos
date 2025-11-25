using System.Text.Json;
using System.Net.Http;

namespace Clientes.API.Services.Clients
{
    public class LocacoesService
    {
        private readonly HttpClient _httpClient;

        public LocacoesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> ClienteTemLocacaoAtiva(int idCliente)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/locacoes/cliente-ativo/{idCliente}");

                if (!response.IsSuccessStatusCode)
                {
                    return true;
                }

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<bool>(content);
            }
            catch (HttpRequestException)
            {
                return true;
            }
        }
    }
}