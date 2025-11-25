using Locacoes.API.Models;

namespace Locacoes.API.Repositories
{
    public interface ILocacaoRepository
    {
        Task<Locacao?> GetByIdAsync(int id);
        Task<Locacao?> GetActiveByIdAsync(int id);
        Task<Locacao> AddAsync(Locacao locacao);
        Task UpdateAsync(Locacao locacao);
        Task<IEnumerable<Locacao>> ListActiveAsync();
        Task DeleteAsync(Locacao locacao);
        Task<bool> HasActiveRentalsByFilmIdAsync(int idFilme);
        Task<bool> HasActiveRentalsByClientIdAsync(int idCliente);
        Task<IEnumerable<Locacao>> ListAllOrderedAsync();
    }
}