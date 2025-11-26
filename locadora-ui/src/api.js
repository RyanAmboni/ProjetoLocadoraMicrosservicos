import axios from 'axios';

const BASE_URL_CLIENTES = 'https://localhost:5101/api';
const BASE_URL_FILMES = 'https://localhost:5001/api';
const BASE_URL_LOCACOES = 'https://localhost:5201/api';

export const ClientesAPI = {
    listar: () => axios.get(`${BASE_URL_CLIENTES}/clientes`),
    cadastrar: (cliente) => axios.post(`${BASE_URL_CLIENTES}/clientes`, cliente),
    atualizar: (id, cliente) => axios.put(`${BASE_URL_CLIENTES}/clientes/${id}`, cliente),
    deletar: (id) => axios.delete(`${BASE_URL_CLIENTES}/clientes/${id}`),
};

export const FilmesAPI = {
    listar: () => axios.get(`${BASE_URL_FILMES}/filmes`),
    cadastrar: (filme) => axios.post(`${BASE_URL_FILMES}/filmes`, filme),
    atualizar: (id, filme) => axios.put(`${BASE_URL_FILMES}/filmes/${id}`, filme),
    deletar: (id) => axios.delete(`${BASE_URL_FILMES}/filmes/${id}`),
};

export const LocacoesAPI = {
    criar: (idCliente, idFilme) => axios.post(`${BASE_URL_LOCACOES}/locacoes`, { idCliente, idFilme }),
    listarAtivas: () => axios.get(`${BASE_URL_LOCACOES}/locacoes`),
    devolver: (idLocacao) => axios.put(`${BASE_URL_LOCACOES}/locacoes/${idLocacao}/devolucao`),
    listarTodas: () => axios.get(`${BASE_URL_LOCACOES}/locacoes/all`), 
};