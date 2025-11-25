
namespace Locacoes.API.Models
{
    public class Locacao
    {
        public int Id { get; set; }

        public int IdCliente { get; set; }
        public int IdFilme { get; set; }

        public DateTime DataLocacao { get; set; } = DateTime.Now; 
        public DateTime? DataDevolucao { get; set; } 

        public bool EstaAtiva => DataDevolucao == null; 
    }
}