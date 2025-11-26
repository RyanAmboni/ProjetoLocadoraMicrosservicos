import React, { useState, useEffect, useMemo } from 'react';
import { ClientesAPI, FilmesAPI, LocacoesAPI } from '../api';

const mapIdToName = (id, list) => {
    const item = list.find(item => item.id === id);
    return item ? item.nome || item.titulo : `ID ${id} (Não encontrado)`;
};

const GerenciarLocacao = () => {
    const [clientes, setClientes] = useState([]);
    const [filmes, setFilmes] = useState([]);
    const [allLocacoes, setAllLocacoes] = useState([]); 
    const [selectedCliente, setSelectedCliente] = useState('');
    const [selectedFilme, setSelectedFilme] = useState('');
    const [mensagem, setMensagem] = useState('');
    const [loading, setLoading] = useState(true);
    const [filterLocacao, setFilterLocacao] = useState('');

    const fetchData = async () => {
        setLoading(true);
        try {
            const [resClientes, resFilmes, resAllLocacoes] = await Promise.all([
                ClientesAPI.listar(),
                FilmesAPI.listar(),
                LocacoesAPI.listarTodas() 
            ]);
            setClientes(resClientes.data);
            setFilmes(resFilmes.data); 
            setAllLocacoes(resAllLocacoes.data);
        } catch (error) {
            setMensagem("Erro ao carregar dados. Verifique se os Back-Ends estão online.");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchData();
    }, []);

    const clientesDisponiveis = useMemo(() => clientes, [clientes]);
    const filmesDisponiveis = useMemo(() => filmes.filter(f => f.quantidadeDisponivel > 0), [filmes]);
    
    const locacoesAtivas = useMemo(() => allLocacoes.filter(l => l.dataDevolucao === null), [allLocacoes]);
    const locacoesFinalizadas = useMemo(() => {
        const finalized = allLocacoes.filter(l => l.dataDevolucao !== null);
        if (!filterLocacao) return finalized;
        
        return finalized.filter(l => 
            mapIdToName(l.idCliente, clientes).toLowerCase().includes(filterLocacao.toLowerCase()) ||
            mapIdToName(l.idFilme, filmes).toLowerCase().includes(filterLocacao.toLowerCase())
        );
    }, [allLocacoes, filterLocacao, clientes, filmes]);


    const handleCriarLocacao = async (e) => {
        e.preventDefault();
        setMensagem('Criando locação...');

        if (!selectedCliente || !selectedFilme) {
            setMensagem('Erro: Selecione um cliente e um filme.');
            return;
        }

        try {
            await LocacoesAPI.criar(parseInt(selectedCliente), parseInt(selectedFilme));
            setMensagem('Locação criada com sucesso! Estoque decrementado.');
            setSelectedCliente('');
            setSelectedFilme('');
            fetchData();
        } catch (error) {
            const status = error.response?.status;
            const msg = error.response?.data?.message || error.message;
            
            if (status === 503) {
                 setMensagem(`Erro (503): Falha de comunicação com um microsserviço. Detalhe: ${msg}`);
            } else if (status === 400 || status === 404) {
                 setMensagem(`Erro (${status}): ${msg}`);
            } else {
                 setMensagem(`Erro desconhecido na locação: ${msg}`);
            }
        }
    };

    const handleDevolverFilme = async (idLocacao) => {
        setMensagem(`Devolvendo locação ${idLocacao}...`);
        try {
            await LocacoesAPI.devolver(idLocacao);
            setMensagem(`Devolução concluída! Estoque incrementado.`);
            fetchData();
        } catch (error) {
            const msg = error.response?.data?.message || error.message;
            setMensagem(`Falha na devolução: ${msg}`);
        }
    };

    if (loading) return <h3>Carregando dados...</h3>;

    return (
        <div>
            <h2>Criar Nova Locação</h2>
            <form onSubmit={handleCriarLocacao} style={{ display: 'flex', gap: '10px', marginBottom: '30px' }}>
                <select 
                    value={selectedCliente} 
                    onChange={e => setSelectedCliente(e.target.value)}
                    required
                    style={{ padding: '10px', border: '1px solid #ccc' }}
                >
                    <option value="">-- Selecionar Cliente --</option>
                    {clientesDisponiveis.map(c => <option key={c.id} value={c.id}>{c.nome} ({c.cpf})</option>)}
                </select>
                <select 
                    value={selectedFilme} 
                    onChange={e => setSelectedFilme(e.target.value)}
                    required
                    style={{ padding: '10px', border: '1px solid #ccc' }}
                >
                    <option value="">-- Filme Disponível (Disp: {filmesDisponiveis.length}) --</option>
                    {filmesDisponiveis.map(f => (
                        <option key={f.id} value={f.id}>
                            {f.titulo} (Disp: {f.quantidadeDisponivel})
                        </option>
                    ))}
                </select>
                <button type="submit" className="btn-primary" disabled={!selectedCliente || !selectedFilme}>
                    CRIAR LOCAÇÃO
                </button>
            </form>
            <p style={{ color: mensagem.includes('Erro') || mensagem.includes('Falha') ? '#7b241c' : '#004d40', fontWeight: 'bold' }}>{mensagem}</p>

            <h2>Locações Ativas ({locacoesAtivas.length})</h2>
            <table style={{ width: '100%', marginBottom: '40px' }}>
                <thead>
                    <tr>
                        <th>ID Locação</th>
                        <th>Cliente</th>
                        <th>Filme</th>
                        <th>Data Locação</th>
                        <th>Ações</th>
                    </tr>
                </thead>
                <tbody>
                    {locacoesAtivas.map(l => (
                        <tr key={l.id}>
                            <td>{l.id}</td>
                            <td>{mapIdToName(l.idCliente, clientes)}</td>
                            <td>{mapIdToName(l.idFilme, filmes)}</td>
                            <td>{new Date(l.dataLocacao).toLocaleDateString()}</td>
                            <td>
                                <button 
                                    onClick={() => handleDevolverFilme(l.id)} 
                                    className="btn-action"
                                >
                                    Devolver Filme
                                </button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
            
            <h2>Histórico de Devoluções ({locacoesFinalizadas.length})</h2>
            <input 
                type="text" 
                placeholder="Filtrar histórico por Nome/Filme" 
                value={filterLocacao} 
                onChange={e => setFilterLocacao(e.target.value)} 
                style={{ padding: '8px', marginBottom: '15px', width: '300px' }}
            />
            <table style={{ width: '100%' }}>
                <thead>
                    <tr>
                        <th>ID Locação</th>
                        <th>Cliente</th>
                        <th>Filme</th>
                        <th>Locação</th>
                        <th>Devolução</th>
                    </tr>
                </thead>
                <tbody>
                    {locacoesFinalizadas.map(l => (
                        <tr key={l.id}>
                            <td>{l.id}</td>
                            <td>{mapIdToName(l.idCliente, clientes)}</td>
                            <td>{mapIdToName(l.idFilme, filmes)}</td>
                            <td>{new Date(l.dataLocacao).toLocaleDateString()}</td>
                            <td style={{ color: '#004d40', fontWeight: 'bold' }}>
                                {new Date(l.dataDevolucao).toLocaleDateString()}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default GerenciarLocacao;