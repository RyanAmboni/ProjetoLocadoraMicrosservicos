# PROJETO DE ARQUITETURA DE MICROSSERVIÇOS: LOCADORA DE VÍDEOS

**EQUIPE:** Ryan Candeu Amboni e Nathan Henrique Frassetto

---

## 1. DOCUMENTO DE REQUISITOS 

### a. Propósito do Sistema

O objetivo é construir um sistema de gestão de locadora de vídeos utilizando a arquitetura de Microsserviços, focado em separação de responsabilidades e isolamento de dados para garantir alta resiliência nas operações transacionais de Locação e Estoque.

### b. Usuários

* **Administrador/Estoque:** Gerencia o catálogo de filmes e estoque.
* **Atendente de Balcão:** Gerencia o cadastro de clientes e as transações de locação/devolução.

### c. Requisitos Funcionais 

1.  **RF01: Clientes:** Manter o cadastro de clientes (ID, Nome, CPF, Email). O CPF deve ser único.
2.  **RF02: Filmes:** Gerenciar o catálogo e o inventário (Título, Gênero, Quantidade Total e Quantidade Disponível).
3.  **RF03: Transação de Locação:** Criar uma Locação somente após a validação remota da existência do Cliente e da disponibilidade do Estoque.
4.  **RF04: Controle de Estoque:** Ao locar, o microsserviço Locacoes.API deve disparar uma alteraçã* (`PUT /decrementar-estoque`) no Filmes.API.
5.  **RF05: Devolução:** Registrar a data de devolução e incrementar o estoque no Filmes.API.
6.  **RF06: Integridade Distribuída (Regra de Exclusão):** Não permitir a exclusão de Clientes ou Filmes que possuam Locações Ativas pendentes.

---

## 2. DESCRITIVO TÉCNICO E ARQUITETURA 

### Microsserviços Existentes

| Microsserviço | Porta (HTTPS) | Função | Persistência  |
| :--- | :--- | :--- | :--- |
| Clientes.API | `5101` | Identidade/Cadastro | .NET 8, REST, SQLite (`clientes.db`) |
| Filmes.API | `5001` | Inventário e Lógica de Estoque | .NET 8, REST, SQLite (`filmes.db`) |
| Locacoes.API | `5201` | Orquestrador de Transações | .NET 8, REST, SQLite (`locacoes.db`) |

### Padrão Arquitetural

Todo o Back-End utiliza o Padrão Repository/Service para separar as responsabilidades e garantir a testabilidade do código. O Front-End foi desenvolvido em React.

### Detalhamento das Integrações 

A comunicação é síncrona, usando Typed Clients (HttpClient) para alta coesão e baixo acoplamento.

| Fluxo de Integração | Tipo  | Endpoint Consumido | Descrição |
| :--- | :--- | :--- | :--- |
| **1. Validação de Cliente** | Busca de Dados  | `Clientes.API: GET /api/clientes/{id}` | Locacoes.API verifica se o Cliente está ativo. |
| **2. Validação de Estoque** | Busca de Dados  | `Filmes.API: GET /api/filmes/{id}` | Locacoes.API consulta o estado do estoque antes da locação. |
| **3. Decremento de Estoque** | Alteração de Dados  | `Filmes.API: PUT /decrementar-estoque/{id}` | Locacoes.API dispara a*alteração de estado no microsserviço de Filmes. |

---

## 3. INSTRUÇÕES DE INICIALIZAÇÃO 

### Pré-requisitos:

1.  SDK .NET 8.0
2.  Node.js (npm)
3.  Git

### A. Restauração e Banco de Dados

Execute os seguintes comandos na pasta raiz do projeto para restaurar e recriar os bancos de dados vazios (SQLite):

```bash
# 1. Restaura pacotes NuGet
dotnet restore

# 2. Recria os bancos de dados (clientes.db, filmes.db, locacoes.db)
dotnet ef database update --project .\Clientes.API
dotnet ef database update --project .\Filmes.API
dotnet ef database update --project .\Locacoes.API
```

### B. Inicialização do Projeto:

O Front-End React está configurado para acessar as APIs em https://localhost:5001/5101/5201.

#### BACK-END: 
Inicie as 3 APIs em terminais separados:

```bash
# Filmes.API 
cd Filmes.API
dotnet run --launch-profile https
# Porta: https://localhost:5001

# Clientes.API 
cd ../Clientes.API
dotnet run --launch-profile https
# Porta: https://localhost:5101

# Locacoes.API 
cd ../Locacoes.API
dotnet run --launch-profile https
# Porta: https://localhost:5201
```

#### FRONT-END: 
Inicie no terminal:

```bash
cd locadora-ui
npm install
npm start
# Acesse: http://localhost:3000
```

