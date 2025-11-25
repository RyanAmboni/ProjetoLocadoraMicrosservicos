using Filmes.API.Models;

namespace Filmes.API.Services
{
    public interface IFilmeService
    {
        Task<Filme?> CadastrarAsync(Filme filme);
        Task<Filme?> BuscarPorIdAsync(int id);
        Task<bool> DecrementarEstoqueAsync(int id);
        Task<bool> IncrementarEstoqueAsync(int id);
        Task<(bool success, string? error)> DeletarAsync(int id);
        Task<IEnumerable<Filme>> ListarTodosAsync();
        Task<Filme?> AtualizarAsync(int id, Filme filmeAtualizado);
    }
}