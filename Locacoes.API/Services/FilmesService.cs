
using System.Net.Http; 

namespace Locacoes.API.Services
{
    public class FilmesService
    {
        private readonly HttpClient _httpClient;

        public FilmesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> TentarDecrementarEstoque(int idFilme)
        {
            return await _httpClient.PutAsync($"/api/filmes/decrementar-estoque/{idFilme}", null);
        }

        public async Task<HttpResponseMessage> TentarIncrementarEstoque(int idFilme)
        {
            return await _httpClient.PutAsync($"/api/filmes/incrementar-estoque/{idFilme}", null);
        }
    }
}