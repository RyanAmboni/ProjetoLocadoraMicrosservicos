import React, { useState } from 'react';
import { ClientesAPI } from '../api';

const CadastroCliente = () => {
    const [cliente, setCliente] = useState({ nome: '', cpf: '', email: '' });
    const [mensagem, setMensagem] = useState('');

    const handleChange = (e) => {
        setCliente({ ...cliente, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setMensagem('Cadastrando...');
        try {
            await ClientesAPI.cadastrar(cliente);
            setMensagem('Cliente cadastrado com sucesso!');
            setCliente({ nome: '', cpf: '', email: '' });
        } catch (error) {
            console.error("Erro no cadastro:", error);
            const errorMessage = error.response?.data?.message || "Falha ao cadastrar. Verifique o CPF.";
            setMensagem(`Erro: ${errorMessage}`);
        }
    };

    return (
        <div style={{ maxWidth: '400px', margin: '0 auto' }}>
            <h2> Cadastro de Cliente</h2>
            <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '15px' }}>
                <input
                    type="text"
                    name="nome"
                    placeholder="Nome Completo"
                    value={cliente.nome}
                    onChange={handleChange}
                    required
                    className="input-retro"
                    style={{ padding: '10px', border: '1px solid #ccc' }}
                />
                <input
                    type="text"
                    name="cpf"
                    placeholder="CPF (apenas números)"
                    value={cliente.cpf}
                    onChange={handleChange}
                    required
                    className="input-retro"
                    style={{ padding: '10px', border: '1px solid #ccc' }}
                />
                <input
                    type="email"
                    name="email"
                    placeholder="Email"
                    value={cliente.email}
                    onChange={handleChange}
                    required
                    className="input-retro"
                    style={{ padding: '10px', border: '1px solid #ccc' }}
                />
                <button type="submit" className="btn-primary">
                    Cadastrar
                </button>
            </form>
            {mensagem && <p style={{ marginTop: '15px', color: mensagem.startsWith('Erro') ? '#7b241c' : '#004d40', fontWeight: 'bold' }}>{mensagem}</p>}
        </div>
    );
};

export default CadastroCliente;