
namespace Locacoes.API.Services
{
    public class ClientesService
    {
        private readonly HttpClient _httpClient;

        public ClientesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> VerificarExistencia(int idCliente)
        {
            var response = await _httpClient.GetAsync($"/api/clientes/{idCliente}");

            return response.IsSuccessStatusCode;
        }
    }
}