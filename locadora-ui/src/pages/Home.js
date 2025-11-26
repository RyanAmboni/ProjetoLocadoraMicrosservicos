import React, { useState, useEffect, useMemo } from 'react';
import { ClientesAPI, FilmesAPI } from '../api';

const Home = () => {
    const [clientes, setClientes] = useState([]);
    const [filmes, setFilmes] = useState([]);
    const [loading, setLoading] = useState(true);
    const [globalError, setGlobalError] = useState(null);
    const [editingCliente, setEditingCliente] = useState(null);
    const [filterCliente, setFilterCliente] = useState('');
    const [filterFilme, setFilterFilme] = useState('');
    
    const [editingFilme, setEditingFilme] = useState(null); 

    const fetchData = async () => {
        setLoading(true);
        setGlobalError(null);

        try {
            const [resClientes, resFilmes] = await Promise.all([
                ClientesAPI.listar(),
                FilmesAPI.listar()
            ]);
            setClientes(resClientes.data);
            setFilmes(resFilmes.data);
        } catch (error) {
            console.error("Erro ao buscar dados:", error);
            setGlobalError("Erro ao carregar dados. Verifique se os 3 Back-Ends estão rodando.");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchData();
    }, []);

    const filteredClientes = useMemo(() => {
        if (!filterCliente) return clientes;
        return clientes.filter(c => 
            c.nome.toLowerCase().includes(filterCliente.toLowerCase()) || 
            c.cpf.includes(filterCliente)
        );
    }, [clientes, filterCliente]);

    const filteredFilmes = useMemo(() => {
        if (!filterFilme) return filmes;
        return filmes.filter(f => 
            f.titulo.toLowerCase().includes(filterFilme.toLowerCase()) || 
            f.genero.toLowerCase().includes(filterFilme.toLowerCase())
        );
    }, [filmes, filterFilme]);

    const handleDeleteCliente = async (id) => {
        if (!window.confirm("Tem certeza que deseja deletar este cliente?")) return;
        try {
            await ClientesAPI.deletar(id);
            alert("Cliente deletado com sucesso!");
            fetchData();
        } catch (error) {
            const errorMessage = error.response?.data?.message || "Erro desconhecido ao deletar.";
            alert(`Falha ao deletar: ${errorMessage}`);
        }
    };
    
    const handleUpdateCliente = async (e) => {
        e.preventDefault();
        try {
            const updatedData = { ...editingCliente }; 
            await ClientesAPI.atualizar(editingCliente.id, updatedData);
            alert("Cliente atualizado com sucesso!");
            setEditingCliente(null);
            fetchData();
        } catch (error) {
            const errorMessage = error.response?.data?.message || "Erro desconhecido ao atualizar.";
            alert(`Falha ao atualizar: ${errorMessage}`);
        }
    };

    const handleDeleteFilme = async (id) => {
        if (!window.confirm("Tem certeza que deseja deletar este filme?")) return;
        try {
            await FilmesAPI.deletar(id);
            alert("Filme deletado com sucesso!");
            fetchData();
        } catch (error) {
            const errorMessage = error.response?.data?.message || "Erro desconhecido ao deletar.";
            alert(`Falha ao deletar: ${errorMessage}`);
        }
    };

    const handleUpdateFilme = async (e) => {
        e.preventDefault();
        try {
            const updatedData = { ...editingFilme, quantidadeTotal: parseInt(editingFilme.quantidadeTotal) }; 
            await FilmesAPI.atualizar(editingFilme.id, updatedData);
            alert("Filme atualizado com sucesso!");
            setEditingFilme(null);
            fetchData();
        } catch (error) {
            const errorMessage = error.response?.data?.message || "Erro desconhecido ao atualizar.";
            alert(`Falha ao atualizar: ${errorMessage}`);
        }
    };


    if (loading) return <h3>Carregando dados das APIs...</h3>;

    return (
        <div style={{ padding: '0 20px' }}>
            {globalError && (
                <div style={{ padding: '15px', marginBottom: '20px', backgroundColor: '#f9e6e6', border: '1px solid #7b241c', color: '#7b241c', fontWeight: 'bold' }}>
                    {globalError}
                </div>
            )}
            
{editingCliente && (
    <div style={{ border: '2px solid #a38b6d', padding: '20px', marginBottom: '20px', backgroundColor: '#fdfbf6' }}>
        <h3>Editando Cliente ID: {editingCliente.id}</h3>
        <form onSubmit={handleUpdateCliente} style={{ display: 'flex', flexDirection: 'column', gap: '10px', maxWidth: '400px' }}>
            <input type="text" placeholder="Nome" value={editingCliente.nome} onChange={e => setEditingCliente({...editingCliente, nome: e.target.value})} required/>
            <input type="email" placeholder="Email" value={editingCliente.email} onChange={e => setEditingCliente({...editingCliente, email: e.target.value})} required/>
                        
            <input type="text" placeholder="CPF" value={editingCliente.cpf} disabled style={{ background: '#eee'}} title="CPF não pode ser editado."/>
            
            <div style={{ display: 'flex', gap: '10px' }}>
                <button type="submit" className="btn-primary">Salvar</button>
                <button type="button" className="btn-action" onClick={() => setEditingCliente(null)}>Cancelar</button>
            </div>
        </form>
    </div>
)}
            
            {editingFilme && (
                <div style={{ border: '2px solid #a38b6d', padding: '20px', marginBottom: '20px', backgroundColor: '#fdfbf6' }}>
                    <h3>Editando Filme ID: {editingFilme.id}</h3>
                    <form onSubmit={handleUpdateFilme} style={{ display: 'flex', flexDirection: 'column', gap: '10px', maxWidth: '400px' }}>
                        <input type="text" placeholder="Título" value={editingFilme.titulo} onChange={e => setEditingFilme({...editingFilme, titulo: e.target.value})} required/>
                        <input type="text" placeholder="Gênero" value={editingFilme.genero} onChange={e => setEditingFilme({...editingFilme, genero: e.target.value})} required/>
                        <input type="number" placeholder="Total" value={editingFilme.quantidadeTotal} onChange={e => setEditingFilme({...editingFilme, quantidadeTotal: e.target.value})} required/>
                        <p>Disponível atual: {editingFilme.quantidadeDisponivel}</p>
                        <div style={{ display: 'flex', gap: '10px' }}>
                            <button type="submit" className="btn-primary">Salvar Alterações</button>
                            <button type="button" className="btn-action" onClick={() => setEditingFilme(null)}>Cancelar</button>
                        </div>
                    </form>
                </div>
            )}

            
            <h2>Filmes Cadastrados (Total: {filmes.length})</h2>
            <input 
                type="text" 
                placeholder="Filtrar por Título ou Gênero" 
                value={filterFilme} 
                onChange={e => setFilterFilme(e.target.value)} 
                style={{ padding: '8px', marginBottom: '15px', width: '300px' }}
            />
            <table style={{ width: '100%', marginBottom: '40px' }}>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Título</th>
                        <th>Gênero</th>
                        <th>Total</th>
                        <th>Disponível</th>
                        <th>Ações</th>
                    </tr>
                </thead>
                <tbody>
                    {filteredFilmes.map(f => (
                        <tr key={f.id}>
                            <td>{f.id}</td>
                            <td>{f.titulo}</td>
                            <td>{f.genero}</td>
                            <td>{f.quantidadeTotal}</td>
                            <td style={{ fontWeight: 'bold', color: f.quantidadeDisponivel > 0 ? '#004d40' : '#7b241c' }}>{f.quantidadeDisponivel}</td>
                             <td>
                                <button onClick={() => setEditingFilme(f)} className="btn-primary" style={{ marginRight: '10px' }}>
                                    Editar
                                </button>
                                <button onClick={() => handleDeleteFilme(f.id)} className="btn-danger">
                                    Excluir
                                </button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            <h2>Clientes Cadastrados (Total: {clientes.length})</h2>
            <input 
                type="text" 
                placeholder="Filtrar por Nome ou CPF" 
                value={filterCliente} 
                onChange={e => setFilterCliente(e.target.value)} 
                style={{ padding: '8px', marginBottom: '15px', width: '300px' }}
            />
            <table style={{ width: '100%' }}>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Nome</th>
                        <th>CPF</th>
                        <th>E-mail</th>
                        <th>Ações</th>
                    </tr>
                </thead>
                <tbody>
                    {filteredClientes.map(c => (
                        <tr key={c.id}>
                            <td>{c.id}</td>
                            <td>{c.nome}</td>
                            <td>{c.cpf}</td>
                            <td>{c.email}</td>
                            <td>
                                <button onClick={() => setEditingCliente(c)} className="btn-primary" style={{ marginRight: '10px' }}>
                                    Editar
                                </button>
                                <button onClick={() => handleDeleteCliente(c.id)} className="btn-danger">
                                    Excluir
                                </button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default Home;