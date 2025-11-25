using Clientes.API.Models;

namespace Clientes.API.Repositories
{
    public interface IClienteRepository
    {
        Task<Cliente?> GetByIdAsync(int id);
        Task<IEnumerable<Cliente>> GetAllAsync();
        Task<Cliente> AddAsync(Cliente cliente);
        Task<bool> ExistsByCpfAsync(string cpf);
        Task DeleteAsync(Cliente cliente);
        Task UpdateAsync(Cliente cliente);
        Task<bool> ExistsByCpfAndIdNotAsync(string cpf, int id);
    }
}