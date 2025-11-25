using Clientes.API.Models;

namespace Clientes.API.Services
{
    public interface IClienteService
    {
        Task<Cliente?> BuscarPorIdAsync(int id);
        Task<IEnumerable<Cliente>> ListarTodosAsync();
        Task<Cliente?> CadastrarAsync(Cliente cliente);
        Task<(bool success, string? error)> DeletarAsync(int id);
        Task<Cliente?> AtualizarAsync(int id, Cliente clienteAtualizado);
    }
}