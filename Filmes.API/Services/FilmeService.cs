using Filmes.API.Models;
using Filmes.API.Repositories;
using Filmes.API.Services.Clients;

namespace Filmes.API.Services
{
    public class FilmeService : IFilmeService
    {
        private readonly IFilmeRepository _repository;
        private readonly LocacoesService _locacoesService;

        public FilmeService(IFilmeRepository repository, LocacoesService locacoesService)
        {
            _repository = repository;
            _locacoesService = locacoesService;
        }

        public async Task<Filme?> CadastrarAsync(Filme filme)
        {
            filme.QuantidadeDisponivel = filme.QuantidadeTotal;
            return await _repository.AddAsync(filme);
        }

        public async Task<Filme?> BuscarPorIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Filme>> ListarTodosAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Filme?> AtualizarAsync(int id, Filme filmeAtualizado)
        {
            var filmeExistente = await _repository.GetByIdAsync(id);

            if (filmeExistente == null)
            {
                return null;
            }

            var diff = filmeAtualizado.QuantidadeTotal - filmeExistente.QuantidadeTotal;

            filmeExistente.Titulo = filmeAtualizado.Titulo;
            filmeExistente.Genero = filmeAtualizado.Genero;
            filmeExistente.QuantidadeTotal = filmeAtualizado.QuantidadeTotal;

            filmeExistente.QuantidadeDisponivel += diff;

            if (filmeExistente.QuantidadeDisponivel < 0)
            {
                filmeExistente.QuantidadeDisponivel = 0;
            }

            await _repository.UpdateAsync(filmeExistente);
            return filmeExistente;
        }

        public async Task<bool> DecrementarEstoqueAsync(int id)
        {
            var filme = await _repository.GetByIdAsync(id);

            if (filme == null) return false;

            if (filme.QuantidadeDisponivel <= 0)
            {
                throw new InvalidOperationException("Estoque esgotado.");
            }

            filme.QuantidadeDisponivel--;
            await _repository.UpdateAsync(filme);
            return true;
        }

        public async Task<bool> IncrementarEstoqueAsync(int id)
        {
            var filme = await _repository.GetByIdAsync(id);

            if (filme == null) return false;

            if (filme.QuantidadeDisponivel >= filme.QuantidadeTotal)
            {
                throw new InvalidOperationException("Estoque já está cheio.");
            }

            filme.QuantidadeDisponivel++;
            await _repository.UpdateAsync(filme);
            return true;
        }

        public async Task<(bool success, string? error)> DeletarAsync(int id)
        {
            var filme = await _repository.GetByIdAsync(id);
            if (filme == null)
            {
                return (false, "Filme não encontrado. (404)");
            }

            if (await _locacoesService.FilmeTemLocacaoAtiva(id))
            {
                return (false, "Não é possível excluir: O filme está atualmente locado. (400)");
            }

            await _repository.DeleteAsync(filme);
            return (true, null);
        }
    }
}