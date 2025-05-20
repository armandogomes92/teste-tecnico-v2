# Thunders Tech Test

## Objetivo do Projeto

Este projeto tem como objetivo gerenciar e analisar dados de pedágios, permitindo a ingestão de informações de passagens em praças de pedágio e a geração de relatórios analíticos, como faturamento por cidade/hora, praças que mais faturaram e contagem de veículos por praça.

## Tecnologias Utilizadas
- **.NET Aspire** (moderno, focado em aplicações distribuídas)
- **.NET 8**
- **Docker** (necessário para dependências externas, como banco de dados)
- **Postman** (para testes de API)

## Estrutura dos Principais Projetos
- `Thunders.TechTest.AppHost`: ponto de entrada padrão da aplicação. É o projeto que deve ser executado.
- `Thunders.TechTest.ApiService`: contém a API principal e lógica de negócio.

## Pré-requisitos
- Docker instalado e em execução na máquina
- .NET 8 SDK instalado

## Como Executar a Aplicação
1. **Certifique-se de que o Docker está rodando** (necessário para dependências externas, como banco de dados).
2. No terminal, navegue até a raiz do projeto.
3. Execute o AppHost:
   ```sh
   dotnet run --project Thunders.TechTest.AppHost
   ```
4. A aplicação estará disponível nas URLs configuradas (ex: https://localhost:7405 ou http://localhost:5373).
5. **As migrations do banco de dados são aplicadas automaticamente ao iniciar a aplicação.** Não é necessário rodar comandos manuais para preparar o banco.
6. **Um plaza de pedágio padrão é criado automaticamente** durante a inicialização, permitindo testes imediatos dos endpoints.

## Como Utilizar a API
A API expõe os seguintes principais endpoints (veja exemplos na collection do Postman):

### 1. Ingestão de Dados de Pedágio
- **Endpoint:** `POST /api/TollUsage/ingest`
- **Exemplo de body:**
  ```json
  {
    "timestamp": "2024-06-01T12:00:00Z",
    "tollPlazaId": 1,
    "city": 0,
    "state": 0,
    "amountPaid": 10.5,
    "vehicleType": 1
  }
  ```

### 2. Geração de Relatórios
- **Valor total por hora por cidade:**
  - `POST /api/Report/generate/hourly-city-revenue?forDate=2024-06-01`
- **Praças que mais faturaram por mês:**
  - `POST /api/Report/generate/top-earning-toll-plazas?year=2024&month=6&quantityOfPlazas=5`
- **Contagem de tipos de veículos por praça:**
  - `POST /api/Report/generate/vehicle-count-by-toll-plaza?tollPlazaId=1&reportDate=2024-06-01`

### 3. Consulta de Status de Relatórios
- **Exemplo:**
  - `GET /api/Report/status/hourly-city-revenue/{reportId}`
  - `GET /api/Report/status/top-earning-toll-plazas/{reportId}`
  - `GET /api/Report/status/vehicle-count-by-toll-plaza/{reportId}`

## Testando com o Postman
1. Importe o arquivo `collection for postman/Thunders.postman_collection.json` no Postman.
2. Utilize os exemplos prontos para testar os endpoints da API.

## Observações
- O projeto não inclui arquivos Dockerfile/docker-compose, mas requer que o Docker esteja ativo para prover dependências externas (ex: banco de dados).
- O AppHost é o ponto de entrada padrão e orquestra os demais serviços.
- **As migrations são aplicadas automaticamente e um plaza padrão é criado ao iniciar a aplicação.**
- Para dúvidas ou problemas, consulte os exemplos da collection do Postman.
