using Locacoes.API.Models;
using Locacoes.API.Repositories;
using System.Net;

namespace Locacoes.API.Services
{
    public class LocacaoService : ILocacaoService
    {
        private readonly ILocacaoRepository _repository;
        private readonly ClientesService _clientesService;
        private readonly FilmesService _filmesService;

        public LocacaoService(
            ILocacaoRepository repository,
            ClientesService clientesService,
            FilmesService filmesService)
        {
            _repository = repository;
            _clientesService = clientesService;
            _filmesService = filmesService;
        }

        public async Task<(Locacao? locacao, string? error)> CriarLocacaoAsync(int idCliente, int idFilme)
        {
            try
            {
                if (!await _clientesService.VerificarExistencia(idCliente))
                {
                    return (null, $"Cliente com ID {idCliente} não encontrado. (404)");
                }

                var decrementarResponse = await _filmesService.TentarDecrementarEstoque(idFilme);

                if (decrementarResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return (null, $"Filme com ID {idFilme} não encontrado. (404)");
                }
                if (decrementarResponse.StatusCode == HttpStatusCode.BadRequest)
                {
                    return (null, "Filme indisponível para locação. Estoque esgotado. (400)");
                }
                if (!decrementarResponse.IsSuccessStatusCode)
                {
                    return (null, "Erro desconhecido ao tentar reservar o estoque do filme. (500)");
                }

                var novaLocacao = new Locacao { IdCliente = idCliente, IdFilme = idFilme, DataLocacao = DateTime.Now };
                var locacaoCriada = await _repository.AddAsync(novaLocacao);

                return (locacaoCriada, null);
            }
            catch (HttpRequestException ex)
            {
                return (null, $"Falha na comunicação com outro microsserviço. (503). Detalhe: {ex.Message}");
            }
        }

        public async Task<(bool success, string? error)> DevolverFilmeAsync(int idLocacao)
        {
            var locacao = await _repository.GetActiveByIdAsync(idLocacao);

            if (locacao == null)
            {
                return (false, "Locação ativa não encontrada ou já devolvida. (404)");
            }

            try
            {
                var incrementarResponse = await _filmesService.TentarIncrementarEstoque(locacao.IdFilme);

                if (!incrementarResponse.IsSuccessStatusCode)
                {
                    var errorContent = await incrementarResponse.Content.ReadAsStringAsync();
                    return (false, $"Falha ao atualizar o estoque remoto. Detalhe: {errorContent} (500)");
                }

                locacao.DataDevolucao = DateTime.Now;
                await _repository.UpdateAsync(locacao);

                return (true, null);
            }
            catch (HttpRequestException ex)
            {
                return (false, $"Falha na comunicação com o microsserviço de Filmes. (503). Detalhe: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Locacao>> ListarLocacoesAtivasAsync()
        {
            return await _repository.ListActiveAsync();
        }

        public async Task<(bool success, string? error)> DeletarLocacaoAsync(int idLocacao)
        {
            var locacao = await _repository.GetByIdAsync(idLocacao);

            if (locacao == null)
            {
                return (false, "Locação não encontrada. (404)");
            }

            if (locacao.DataDevolucao == null)
            {
                return (false, "Não é possível deletar uma locação ativa. Realize a devolução antes. (400)");
            }

            await _repository.DeleteAsync(locacao);
            return (true, null);
        }
        public async Task<IEnumerable<Locacao>> ListarTodasLocacoesAsync()
        {
            return await _repository.ListAllOrderedAsync();
        }

    }


}