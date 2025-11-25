using Clientes.API.Models;
using Clientes.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Clientes.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _service;

        public ClientesController(IClienteService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<Cliente>> CadastrarCliente(Cliente cliente)
        {
            var novoCliente = await _service.CadastrarAsync(cliente);

            if (novoCliente == null)
            {
                return Conflict(new { message = "Já existe um cliente cadastrado com este CPF." });
            }

            return CreatedAtAction(nameof(BuscarPorId), new { id = novoCliente.Id }, novoCliente);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> ListarTodos()
        {
            return Ok(await _service.ListarTodosAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> BuscarPorId(int id)
        {
            var cliente = await _service.BuscarPorIdAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return Ok(cliente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarCliente(int id, Cliente cliente)
        {
            try
            {
                var clienteAtualizado = await _service.AtualizarAsync(id, cliente);

                if (clienteAtualizado == null)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message }); 
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarCliente(int id)
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