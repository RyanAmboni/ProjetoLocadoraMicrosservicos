import React, { useState } from 'react';
import { FilmesAPI } from '../api';

const CadastroFilme = () => {
    const [filme, setFilme] = useState({ titulo: '', genero: '', quantidadeTotal: 0 });
    const [mensagem, setMensagem] = useState('');

    const handleChange = (e) => {
        setFilme({ ...filme, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setMensagem('Cadastrando...');

        const filmeData = {
            ...filme,
            quantidadeTotal: parseInt(filme.quantidadeTotal, 10), 
        };

        try {

            await FilmesAPI.cadastrar(filmeData); 
            setMensagem('Filme cadastrado com sucesso!');
            setFilme({ titulo: '', genero: '', quantidadeTotal: 0 });
        } catch (error) {
            console.error("Erro no cadastro:", error);
            const errorMessage = error.response?.data?.message || "Falha ao cadastrar filme. Verifique os dados.";
            setMensagem(`Erro: ${errorMessage}`);
        }
    };

    return (
        <div style={{ maxWidth: '400px', margin: '0 auto' }}>
            <h2>Cadastro de Filme</h2>
            <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '15px' }}>
                <input
                    type="text"
                    name="titulo"
                    placeholder="Título do Filme"
                    value={filme.titulo}
                    onChange={handleChange}
                    required
                    style={{ padding: '10px', border: '1px solid #ccc' }}
                />
                <input
                    type="text"
                    name="genero"
                    placeholder="Gênero (Ex: Ação, Drama)"
                    value={filme.genero}
                    onChange={handleChange}
                    required
                    style={{ padding: '10px', border: '1px solid #ccc' }}
                />
                <input
                    type="number"
                    name="quantidadeTotal"
                    placeholder="Quantidade Inicial em Estoque"
                    value={filme.quantidadeTotal}
                    onChange={handleChange}
                    required
                    min="1"
                    style={{ padding: '10px', border: '1px solid #ccc' }}
                />
                <button type="submit" className="btn-primary">
                    Cadastrar Filme
                </button>
            </form>
            {mensagem && <p style={{ marginTop: '15px', color: mensagem.startsWith('Erro') ? '#7b241c' : '#004d40', fontWeight: 'bold' }}>{mensagem}</p>}
        </div>
    );
};

export default CadastroFilme;