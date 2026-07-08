# Conexão Solidária — Worker de Processamento de Doações

Serviço de background desenvolvido em C#/.NET para o Hackathon Conexão Solidária. Este Worker é responsável por consumir de forma assíncrona os eventos de doação recebidos em uma fila do RabbitMQ, computar os valores e atualizar o progresso financeiro das campanhas diretamente no banco de dados.

## 🛠️ Tecnologias Utilizadas
* .NET (C#)
* Entity Framework Core & PostgreSQL
* MassTransit & RabbitMQ (Mensageria assíncrona)

## 📌 Pré-requisitos
* SDK do .NET instalado.
* Docker Desktop instalado e executando (a infraestrutura é compartilhada com o repositório principal).

---

## 🚀 Como Rodar o Worker Localmente

### Passo 1: Iniciar a Infraestrutura Compartilhada
A infraestrutura (banco PostgreSQL e o broker RabbitMQ) é centralizada no Docker Compose do repositório da **API Principal** (repositório `application`).

1. Acesse o repositório da **API Principal** e certifique-se de que a infraestrutura está ativa executando:
   ```bash
   docker compose up -d
   ```
   *(Caso não tenha feito isso antes, consulte o README do repositório principal para criar o arquivo `.env` necessário para o banco de dados).*

### Passo 2: Configurar as Variáveis de Ambiente e Segredos (Secrets)
Para que o Worker se conecte ao banco de dados e ao RabbitMQ locais criados pela API principal, utilize o .NET Secret Manager.

1. Abra o terminal na pasta deste projeto (onde fica o arquivo `ConexaoSolidaria.Worker.sln`) e navegue até a pasta interna do projeto:
   ```bash
   cd ConexaoSolidaria.Worker
   ```
2. Inicialize o Secret Manager:
   ```bash
   dotnet user-secrets init
   ```
3. Configure os segredos de conexão local (lembre-se de usar a mesma senha de banco configurada no seu `.env` da infraestrutura):
   ```bash
   dotnet user-secrets set "ConnectionStrings:Worker" "Host=localhost;Port=5432;Database=conexaosolidaria;Username=postgres;Password=sua_senha_aqui"
   dotnet user-secrets set "Bus:Host" "localhost"
   dotnet user-secrets set "Bus:Port" "5672"
   dotnet user-secrets set "Bus:User" "guest"
   dotnet user-secrets set "Bus:Password" "guest"
   ```

### Passo 3: Aplicar as Migrações de Banco do Worker
Este Worker possui tabelas próprias (como a tabela de controle consolidado `ArrecadacaoCampanha`). Aplique as migrations no banco de dados compartilhado executando:
```bash
dotnet ef database update
```

### Passo 4: Executar o Worker
Com a infraestrutura activa e os segredos devidamente configurados, execute o comando para iniciar o processamento:
```bash
dotnet run
```

---

## 🧪 Validando a Integração (Fluxo Ponta a Ponta)

Com a **API Principal** e o **Worker** rodando ao mesmo tempo:

1. No repositório da **API Principal**, utilize o arquivo `ConexaoSolidaria.API.http` para realizar os testes:
   - Cadastre uma ONG e faça login para obter o Token.
   - Crie uma nova Campanha de doações (copie o `ID` retornado).
   - Cadastre um Doador e faça login para obter o Token.
   - Envie uma requisição de **Simular Doação** (`POST /doacoes`) informando o `ID` da campanha criada e o valor desejado.
2. A API publicará o evento `DoacaoRecebida` na fila do RabbitMQ.
3. No terminal deste **Worker**, você verá a mensagem sendo recebida pelo Consumer `DoacaoRecebidaConsumer` e a base de dados sendo atualizada com sucesso.
