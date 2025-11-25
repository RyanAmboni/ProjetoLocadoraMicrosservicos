
namespace Filmes.API.Models
{
    public class Filme
    {
        public int Id { get; set; }
        public required string Titulo { get; set; }
        public required string Genero { get; set; }

        public int QuantidadeTotal { get; set; }
        public int QuantidadeDisponivel { get; set; }
    }
}