using Filmes.API.Data;
using Filmes.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Filmes.API.Repositories
{
    public class FilmeRepository : IFilmeRepository
    {
        private readonly FilmeContext _context;

        public FilmeRepository(FilmeContext context)
        {
            _context = context;
        }

        public async Task<Filme?> GetByIdAsync(int id)
        {
            return await _context.Filmes.FindAsync(id);
        }

        public async Task<Filme> UpdateAsync(Filme filme)
        {
            _context.Filmes.Update(filme);
            await _context.SaveChangesAsync();
            return filme;
        }

        public async Task<Filme> AddAsync(Filme filme)
        {
            _context.Filmes.Add(filme);
            await _context.SaveChangesAsync();
            return filme;
        }

        public async Task DeleteAsync(Filme filme)
        {
            _context.Filmes.Remove(filme);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Filme>> GetAllAsync()
        {
            return await _context.Filmes.ToListAsync();
        }
    }
}