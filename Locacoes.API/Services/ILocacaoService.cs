using Locacoes.API.Models;

namespace Locacoes.API.Services
{
    public interface ILocacaoService
    {
        Task<(Locacao? locacao, string? error)> CriarLocacaoAsync(int idCliente, int idFilme);
        Task<(bool success, string? error)> DevolverFilmeAsync(int idLocacao);
        Task<IEnumerable<Locacao>> ListarLocacoesAtivasAsync();
        Task<(bool success, string? error)> DeletarLocacaoAsync(int idLocacao);
        Task<IEnumerable<Locacao>> ListarTodasLocacoesAsync();
    }
}