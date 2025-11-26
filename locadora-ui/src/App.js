import React from 'react';
import Home from './pages/Home';
import GerenciarLocacao from './pages/GerenciarLocacao';
import CadastroCliente from './pages/CadastroCliente';
import CadastroFilme from './pages/CadastroFilme'; 
import './App.css'; 

function App() {
  const [currentPage, setCurrentPage] = React.useState('home');

  const renderPage = () => {
    switch (currentPage) {
      case 'cadastrarCliente': 
        return <CadastroCliente />;
      case 'cadastrarFilme': 
        return <CadastroFilme />;
      case 'locacao':
        return <GerenciarLocacao />;
      case 'home':
      default:
        return <Home />;
    }
  };

  return (
    <div className="App">
      <header className="App-header">
        <h1>Locadora Microsserviços</h1>
        <nav style={{ marginTop: '10px' }}>
          <button onClick={() => setCurrentPage('home')}>
            Página Inicial 
          </button>
          <button onClick={() => setCurrentPage('cadastrarCliente')}>
            Cadastrar Cliente
          </button>
          <button onClick={() => setCurrentPage('cadastrarFilme')}>
            Cadastrar Filme
          </button>
          <button onClick={() => setCurrentPage('locacao')}>
            Gerenciar Locação
          </button>
        </nav>
      </header>
      <div style={{ padding: '20px' }}>
        {renderPage()}
      </div>
    </div>
  );
}

export default App;