using Locacoes.API.Models;
using Locacoes.API.Services;
using Locacoes.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Locacoes.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocacoesController : ControllerBase
    {
        private readonly ILocacaoService _service;
        private readonly ILocacaoRepository _repository;

        public LocacoesController(ILocacaoService service, ILocacaoRepository repository)
        {
            _service = service;
            _repository = repository;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Locacao), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.ServiceUnavailable)]
        public async Task<ActionResult<Locacao>> CriarLocacao(Locacao locacaoInput)
        {
            var (locacao, error) = await _service.CriarLocacaoAsync(locacaoInput.IdCliente, locacaoInput.IdFilme);

            if (locacao != null)
            {
                return CreatedAtAction(nameof(BuscarLocacao), new { id = locacao.Id }, locacao);
            }

            if (error!.Contains("400")) return BadRequest(new { message = error });
            if (error!.Contains("404")) return NotFound(new { message = error });
            if (error!.Contains("503")) return StatusCode(503, new { message = error });

            return StatusCode(500, new { message = error });
        }

        [HttpPut("{id}/devolucao")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.ServiceUnavailable)]
        public async Task<IActionResult> DevolverFilme(int id)
        {
            var (success, error) = await _service.DevolverFilmeAsync(id);

            if (success)
            {
                return NoContent();
            }

            if (error!.Contains("404")) return NotFound(new { message = error });
            if (error!.Contains("503")) return StatusCode(503, new { message = error });

            return StatusCode(500, new { message = error });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarLocacao(int id)
        {
            var (success, error) = await _service.DeletarLocacaoAsync(id);

            if (success)
            {
                return NoContent();
            }

            if (error!.Contains("404")) return NotFound(new { message = error });
            if (error!.Contains("400")) return BadRequest(new { message = error });

            return StatusCode(500, new { message = error });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Locacao>>> ListarLocacoesAtivas()
        {
            return Ok(await _service.ListarLocacoesAtivasAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Locacao>> BuscarLocacao(int id)
        {
            var locacao = await _repository.GetByIdAsync(id);

            if (locacao == null)
            {
                return NotFound();
            }
            return Ok(locacao);
        }

        [HttpGet("filme-ativo/{idFilme}")]
        public async Task<IActionResult> VerificarLocacaoAtivaPorFilme(int idFilme)
        {
            var hasActive = await _repository.HasActiveRentalsByFilmIdAsync(idFilme);
            return Ok(hasActive);
        }

        [HttpGet("cliente-ativo/{idCliente}")]
        public async Task<IActionResult> VerificarLocacaoAtivaPorCliente(int idCliente)
        {
            var hasActive = await _repository.HasActiveRentalsByClientIdAsync(idCliente);
            return Ok(hasActive);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Locacao>>> ListarTodasLocacoes()
        {
            return Ok(await _service.ListarTodasLocacoesAsync());
        }
    }
}