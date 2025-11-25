using Locacoes.API.Data;
using Locacoes.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Locacoes.API.Repositories
{
    public class LocacaoRepository : ILocacaoRepository
    {
        private readonly LocacaoContext _context;

        public LocacaoRepository(LocacaoContext context)
        {
            _context = context;
        }

        public async Task<Locacao?> GetByIdAsync(int id)
        {
            return await _context.Locacoes.FindAsync(id);
        }

        public async Task<Locacao?> GetActiveByIdAsync(int id)
        {
            return await _context.Locacoes
                                 .Where(l => l.Id == id && l.DataDevolucao == null)
                                 .FirstOrDefaultAsync();
        }

        public async Task<Locacao> AddAsync(Locacao locacao)
        {
            _context.Locacoes.Add(locacao);
            await _context.SaveChangesAsync();
            return locacao;
        }

        public async Task UpdateAsync(Locacao locacao)
        {
            _context.Locacoes.Update(locacao);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Locacao>> ListActiveAsync()
        {
            return await _context.Locacoes
                                 .Where(l => l.DataDevolucao == null)
                                 .ToListAsync();
        }

        public async Task DeleteAsync(Locacao locacao)
        {
            _context.Locacoes.Remove(locacao);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasActiveRentalsByFilmIdAsync(int idFilme)
        {
            return await _context.Locacoes.AnyAsync(l => l.IdFilme == idFilme && l.DataDevolucao == null);
        }

        public async Task<bool> HasActiveRentalsByClientIdAsync(int idCliente)
        {
            return await _context.Locacoes.AnyAsync(l => l.IdCliente == idCliente && l.DataDevolucao == null);
        }

        public async Task<IEnumerable<Locacao>> ListAllOrderedAsync()
        {
            return await _context.Locacoes
                                 .OrderByDescending(l => l.DataDevolucao) 
                                 .ThenByDescending(l => l.DataLocacao) 
                                 .ToListAsync();
        }
    }
}
