# Introdução  

Bem-vindo ao teste técnico da Thunders! 🚀 

Estamos empolgados por você estar participando deste desafio e animados para conhecer melhor suas habilidades e seu potencial. Aproveite este momento para demonstrar sua criatividade, conhecimento técnico e capacidade de resolver problemas. 

Lembre-se: você não está sozinho nessa jornada! Caso tenha qualquer dúvida ou precise de suporte, sinta-se à vontade para entrar em contato com o nosso time. Estamos aqui para ajudar e garantir que você tenha a melhor experiência possível. 

Boa sorte e mãos à obra! Estamos ansiosos para ver o que você pode criar. 

# Requisitos Funcionais 

O governo anunciou a abertura de uma licitação para o desenvolvimento e implementação de um sistema informatizado voltado à geração de relatórios detalhados de faturamento das unidades de pedágio do país. Como vencedor dessa licitação, você será responsável por projetar e implementar uma solução eficiente e escalável, 
capaz de receber dados sobre as utilizações de cada unidade e consolidá-los em um relatório no formato especificado pelo edital. De acordo com informações do UOL, o Brasil conta com mais de 1.800 praças de pedágio distribuídas pelas 27 unidades federativas, o que evidencia a magnitude e a importância do projeto. Este software deverá não apenas atender aos requisitos técnicos, 
mas também ser capaz de lidar como grande volume de dados gerado diariamente, garantindo a precisão e a agilidade necessárias para a tomada de decisões administrativas e estratégicas. 

Os dados de utilização devem ser unitários e conter minimamente os atributos a seguir: 

- Data e hora de utilização 
- Praça 
- Cidade 
- Estado 
- Valor pago 
- Tipo de veículo (Moto, Carro ou Caminhão) 

 

Os relatórios a seguir foram solicitados: 

- Valor total por hora por cidade 
- As praças que mais faturaram por mês (a quantidade a ser processada deve ser configurável) 
- Quantos tipos de veículos passaram em uma determinada praça 


# Requisitos Técnicos 

 
A solução deve utilizar o template já estruturado disponível neste repositório, basta criar um fork ou clonar para começar.

- Toda implementação deve ser feita dentro do projeto ApiService encontrado no template. Recomenda-se não alterar o código dos outros projetos, porém, caso julgue necessário, alterações podem ser realizadas. 
- A solução deverá fornecer uma API para que as empresas dos pedágios possam enviar os dados.  
- O gatilho para processamento dos relatórios deve ser via API, simulando um agendamento. 
- Persistir os dados de utilização e os resultados dos relatórios. 
- O Timeout padrão é de 10 segundos e não pode ser alterado. 
- A solução utiliza .NET Aspire, então serviços externos como RabbitMQ, SQL Server e outros estão disponíveis de antemão. Para iniciar a aplicação basta manter o projeto AppHost como startup project. 
- Para facilitar o uso do broker a biblioteca Rebus está disponível, bastando apenas a criação de mensagens e seus respectivos “ouvintes”. 
- A implementação de testes para demonstrar o potencial da solução garantirá pontos extras. 
- A solução fornece suporte para OpenTelemetry 
- Considerar que milhões de registros serão ingeridos pela aplicação. 
- Os componentes existentes podem ser alterados, por exemplo SQL Server -> Postgres ou RabbitMQ -> Kafka. 
- Novos componentes podem ser agregados a solução, caso seja necessário.

 

Alguns componentes foram criados e disponibilizados para facilitar a implementação do teste: 

- Interface ‘IMessageSender’ do projeto OutOfBox: permite o envio de mensagens para o broker. 
- Features: para habilitar o uso de Mensageria ou Entity Framework através do padrão de configurações do .NET

# Entendendo a Implementação no ApiService

Esta seção detalha a arquitetura e o fluxo de dados implementados no projeto `Thunders.TechTest.ApiService` para atender aos requisitos do teste técnico.

## Fluxo de Dados e Arquitetura

O sistema foi projetado para lidar com um grande volume de dados de utilização de pedágios de forma eficiente e escalável, utilizando uma abordagem de processamento assíncrono.

1.  **Ingestão de Dados via API:**
    *   As concessionárias de pedágio enviam os dados de utilização (data/hora, praça, cidade, estado, valor, tipo de veículo) para um endpoint específico da API no `ApiService`.
    *   Este endpoint é responsável por receber os dados, realizar uma validação inicial e, em seguida, enfileirar uma mensagem para processamento assíncrono.

2.  **Processamento Assíncrono com Mensageria (Rebus e RabbitMQ):**
    *   Ao receber os dados, a API publica uma mensagem (por exemplo, `ProcessTollUsageDataCommand`) em uma fila do RabbitMQ. Isso desacopla o recebimento dos dados do seu processamento, permitindo que a API responda rapidamente.
    *   Um "Handler" (ouvinte de mensagem), como o `TollUsageDataHandler`, consome as mensagens da fila.
    *   Este handler é responsável por processar os dados de utilização, o que inclui a persistência dos dados brutos.

3.  **Persistência de Dados (Entity Framework Core e SQL Server):**
    *   Os dados brutos de utilização recebidos são armazenados em um banco de dados SQL Server. A interação com o banco de dados é gerenciada pelo Entity Framework Core.
    *   Os resultados dos relatórios gerados também são persistidos no banco de dados para consulta futura.

4.  **Geração e Consulta de Relatórios via API:**
    *   A geração dos relatórios solicitados (`Valor total por hora por cidade`, `Praças que mais faturaram por mês`, `Quantos tipos de veículos passaram em uma determinada praça`) é acionada através de endpoints específicos na API.
    *   Esses endpoints podem, dependendo da complexidade e do volume de dados, iniciar um processo de geração de relatório (que pode ser síncrono para relatórios rápidos ou também assíncrono para relatórios mais demorados).
    *   Os serviços responsáveis pela lógica de negócio dos relatórios consultam os dados persistidos (tanto os dados brutos de utilização quanto os resultados de relatórios previamente processados) para gerar as informações solicitadas.
    *   Os resultados são então retornados pela API.

## Estrutura do Projeto `Thunders.TechTest.ApiService`

Para organizar o código e as responsabilidades, a seguinte estrutura de pastas (sugestão) pode ser adotada dentro do `ApiService`:

*   **`Controllers`**: Contém os controladores da API, responsáveis por expor os endpoints HTTP para ingestão de dados e acionamento/consulta de relatórios.
    *   Ex: `TollUsagesController.cs`, `ReportController.cs`
*   **`Messages`**: Define as classes de mensagem utilizadas para a comunicação via broker (Rebus/RabbitMQ).
    *   Ex: `ProcessTollUsageDataCommand.cs`
*   **`Handlers`**: Contém os manipuladores de mensagens que processam os comandos/eventos recebidos do broker.
    *   Ex: `TollUsageDataHandler.cs`
*   **`Services`**: Agrupa a lógica de negócio principal, incluindo o processamento de dados de utilização e a geração dos relatórios.
    *   Ex: `ReportService.cs`, `TollUsageProcessingService.cs`
*   **`Data`**: Responsável pela interação com o banco de dados, incluindo o DbContext do Entity Framework e as configurações das entidades.
    *   Ex: `ApiServiceDbContext.cs`, `Repositories/` (opcional, para implementar o padrão Repository)
*   **`Models` (ou `Entities`)**: Define as entidades do domínio que são mapeadas para o banco de dados.
    *   Ex: `TollUsage.cs`, `HourlyCityRevenueReport.cs`
*   **`DTOs` (Data Transfer Objects)**: Classes usadas para transferir dados entre as camadas, especialmente para as requisições e respostas da API.
    *   Ex: `TollUsageInputDto.cs`, `TopTollsReportDto.cs`

## Interação dos Componentes

*   **AppHost**: Orquestra o ambiente de desenvolvimento com .NET Aspire, iniciando o `ApiService`, o SQL Server, o RabbitMQ e outros serviços configurados.
*   **ApiService**:
    *   Recebe requisições HTTP nos `Controllers`.
    *   Usa `IMessageSender` (do projeto `OutOfBox`) para enviar mensagens para o RabbitMQ.
    *   Os `Handlers` consomem mensagens do RabbitMQ e utilizam os `Services`.
    *   Os `Services` utilizam o `DbContext` (de `Data`) para interagir com o SQL Server.
*   **RabbitMQ**: Atua como broker de mensagens, garantindo o processamento assíncrono e desacoplado.
*   **SQL Server**: Armazena os dados de utilização e os relatórios gerados.

## Orientações Gerais

*   **Configuração**: Verifique as configurações no `appsettings.json` do `ApiService` e as configurações de features para habilitar/desabilitar o uso de Mensageria ou Entity Framework.
*   **Testes**: A implementação de testes unitários e de integração é crucial para garantir a qualidade e a robustez da solução. Considere testar os controllers, services e handlers.
*   **Escalabilidade**: A arquitetura com mensageria foi escolhida pensando na escalabilidade. O número de instâncias do `ApiService` (ou de workers processando as mensagens) pode ser aumentado para lidar com uma carga maior.
*   **OpenTelemetry**: Utilize o suporte a OpenTelemetry para monitorar a performance da aplicação e identificar gargalos.
*   **Timeout**: Lembre-se do timeout padrão de 10 segundos para as operações síncronas da API. Operações mais longas devem ser preferencialmente assíncronas.

---