using Filmes.API.Models;
using Filmes.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Filmes.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilmesController : ControllerBase
    {
        private readonly IFilmeService _service;

        public FilmesController(IFilmeService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<Filme>> CadastrarFilme(Filme filme)
        {
            var novoFilme = await _service.CadastrarAsync(filme);
            return CreatedAtAction(nameof(BuscarPorId), new { id = novoFilme.Id }, novoFilme);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Filme>>> ListarTodos()
        {
            return Ok(await _service.ListarTodosAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Filme>> BuscarPorId(int id)
        {
            var filme = await _service.BuscarPorIdAsync(id);

            if (filme == null)
            {
                return NotFound();
            }

            return Ok(filme);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarFilme(int id, Filme filme)
        {
            try
            {
                var filmeAtualizado = await _service.AtualizarAsync(id, filme);

                if (filmeAtualizado == null)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao processar atualização: " + ex.Message });
            }
        }

        [HttpPut("decrementar-estoque/{id}")]
        public async Task<IActionResult> DecrementarEstoque(int id)
        {
            try
            {
                if (await _service.DecrementarEstoqueAsync(id))
                {
                    return NoContent();
                }
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("incrementar-estoque/{id}")]
        public async Task<IActionResult> IncrementarEstoque(int id)
        {
            try
            {
                if (await _service.IncrementarEstoqueAsync(id))
                {
                    return NoContent();
                }
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarFilme(int id)
        {
            var (success, error) = await _service.DeletarAsync(id);

            if (success)
            {
                return NoContent();
            }

            if (error!.Contains("404")) return NotFound(new { message = error });
            if (error!.Contains("400")) return BadRequest(new { message = error });

            return StatusCode(500, new { message = error });
        }
    }
}