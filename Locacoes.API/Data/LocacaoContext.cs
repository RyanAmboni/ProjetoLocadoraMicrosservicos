
using Locacoes.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Locacoes.API.Data
{
    public class LocacaoContext : DbContext
    {
        public DbSet<Locacao> Locacoes { get; set; }

        public LocacaoContext(DbContextOptions<LocacaoContext> options) : base(options)
        {
        }
    }
}