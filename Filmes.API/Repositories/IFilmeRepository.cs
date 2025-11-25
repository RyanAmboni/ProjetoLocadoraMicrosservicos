using Filmes.API.Models;

namespace Filmes.API.Repositories
{
    public interface IFilmeRepository
    {
        Task<Filme?> GetByIdAsync(int id);
        Task<Filme> UpdateAsync(Filme filme);
        Task<Filme> AddAsync(Filme filme);
        Task DeleteAsync(Filme filme);
        Task<IEnumerable<Filme>> GetAllAsync(); 
    }
}