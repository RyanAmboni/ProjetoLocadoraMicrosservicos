using Clientes.API.Models;
using Clientes.API.Repositories;
using Clientes.API.Services.Clients;

namespace Clientes.API.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _repository;
        private readonly LocacoesService _locacoesService;

        public ClienteService(IClienteRepository repository, LocacoesService locacoesService)
        {
            _repository = repository;
            _locacoesService = locacoesService;
        }

        public async Task<Cliente?> BuscarPorIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Cliente>> ListarTodosAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Cliente?> CadastrarAsync(Cliente cliente)
        {
            if (await _repository.ExistsByCpfAsync(cliente.CPF))
            {
                return null;
            }

            return await _repository.AddAsync(cliente);
        }

        public async Task<(bool success, string? error)> DeletarAsync(int id)
        {
            var cliente = await _repository.GetByIdAsync(id);
            if (cliente == null)
            {
                return (false, "Cliente não encontrado. (404)");
            }

            if (await _locacoesService.ClienteTemLocacaoAtiva(id))
            {
                return (false, "Não é possível excluir: O cliente possui locações ativas. (400)");
            }

            await _repository.DeleteAsync(cliente);
            return (true, null);
        }

        public async Task<Cliente?> AtualizarAsync(int id, Cliente clienteAtualizado)
        {
            var clienteExistente = await _repository.GetByIdAsync(id);

            if (clienteExistente == null)
            {
                return null;
            }

            if (await _repository.ExistsByCpfAndIdNotAsync(clienteAtualizado.CPF, id))
            {
                throw new InvalidOperationException("CPF já cadastrado para outro cliente.");
            }

            clienteExistente.Nome = clienteAtualizado.Nome;
            clienteExistente.Email = clienteAtualizado.Email;
            clienteExistente.CPF = clienteAtualizado.CPF; 

            await _repository.UpdateAsync(clienteExistente);
            return clienteExistente;
        }
    }
}