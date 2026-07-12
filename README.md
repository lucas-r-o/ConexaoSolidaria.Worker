# Conexão Solidária — Worker de Processamento de Doações

Serviço de background desenvolvido em C#/.NET para o Hackathon Conexão Solidária. Este Worker é responsável por consumir de forma assíncrona os eventos de doação recebidos em uma fila do RabbitMQ, computar os valores e atualizar o progresso financeiro das campanhas diretamente no banco de dados.

## 🛠️ Tecnologias Utilizadas
* .NET (C#)
* Entity Framework Core & PostgreSQL
* MassTransit & RabbitMQ (Mensageria assíncrona)
* Kubernetes (Docker Desktop)

## 📌 Pré-requisitos
* SDK do .NET instalado.
* Docker Desktop instalado e executando, com o Kubernetes habilitado (a infraestrutura é compartilhada com o repositório principal).
* `kubectl` configurado apontando para o cluster local do Docker Desktop.

---

## 🚀 Como Rodar o Worker Localmente

Existem duas formas de subir o ambiente: via **Docker Compose** (mais simples, para desenvolvimento do dia a dia) ou via **Kubernetes** (ambiente que replica a arquitetura final do projeto). Escolha uma delas e configure o `Host` de acordo com a tabela abaixo.

### Passo 1: Iniciar a Infraestrutura Compartilhada

A infraestrutura (banco PostgreSQL e o broker RabbitMQ) é centralizada no repositório da **API Principal** (repositório `application`).

**Opção A — Docker Compose:**
```bash
docker compose up -d
```
*(Caso não tenha feito isso antes, consulte o README do repositório principal para criar o arquivo `.env` necessário para o banco de dados).*

**Opção B — Kubernetes:** veja a seção [Executando via Kubernetes](#-executando-via-kubernetes) mais abaixo.

### Passo 2: Configurar as Variáveis de Ambiente

As configurações de conexão são feitas via `appsettings.json`.

1. Para rodar localmente va na pasta `ConexaoSolidaria.Worker`, crie (ou edite) o arquivo `appsettings.Development.json` com o seguinte conteúdo:

```json
{
  "ConnectionStrings": {
    "Worker": "Host=;Port=;Database=conexaosolidaria;Username=postgres;Password=sua_senha_aqui"
  },
  "Bus": {
    "Host": "localhost",
    "Port": "5672",
    "User": "guest",
    "Password": "guest"
  }
}
```

2. **Atenção ao valor de `Bus:Host`**, ele muda dependendo de como você subiu a infraestrutura:

| Ambiente | Valor de `Bus:Host` |
|---|---|
| Docker Compose | `localhost` |
| Kubernetes (Docker Desktop) | `host.docker.internal` |

3. Localmente, a porta do RabbitMQ é sempre `5672`, com usuário e senha `guest`.

### Passo 3: Aplicar as Migrações de Banco do Worker

Este Worker possui tabelas próprias (como a tabela de controle consolidado `ArrecadacaoCampanha`). Aplique as migrations no banco de dados compartilhado executando:
```bash
dotnet ef database update
```

### Passo 4: Executar o Worker

Com a infraestrutura ativa e o `appsettings.Development.json` devidamente configurado, execute:
```bash
dotnet run
```

---

## ☸️ Executando via Kubernetes

O projeto também pode ser executado inteiramente dentro do cluster Kubernetes local (Docker Desktop).

### Passo 1: Gerar a imagem Docker do Worker

Antes de aplicar os manifests, é necessário buildar a imagem localmente com o nome e tag esperados pelo deployment:

```bash
docker build -t conexao-solidaria-worker:latest .
```

> ⚠️ O nome da imagem precisa ser exatamente `conexao-solidaria-worker:latest`, pois é essa referência que os manifests do Kubernetes utilizam. Sem isso, o pod não sobe corretamente.

### Passo 2: Aplicar os manifests do Kubernetes

Acesse a pasta com o Dockerfile do Worker:
```bash
cd ConexaoSolidaria.Worker/k8s
```

Primeiro, crie o namespace:
```bash
kubectl apply -f namespace.yaml
```

Depois, aplique o restante dos manifests (Deployments, Services, ConfigMaps):
```bash
kubectl apply -f .
```

### Passo 3: Verificar se o pod subiu corretamente

```bash
kubectl get pods -n apps
```

O pod do Worker deve aparecer com status `Running`.

> 💡 Lembre-se: como o cluster é local (Docker Desktop), o Worker rodando dentro de um pod precisa se conectar ao RabbitMQ e ao Postgres usando `host.docker.internal` como Host, e não `localhost` — conforme indicado na tabela do Passo 2.

---

## 🔄 CI/CD

O pipeline está em `.github/workflows/ci.yml` e é executado automaticamente a cada **Pull Request com destino à branch `main`**, ou manualmente via `workflow_dispatch`. Ele realiza, nesta ordem:

1. Restore e build da solution `ConexaoSolidaria.Worker.sln` (.NET 10, configuração Release).
2. Build e push da imagem Docker do Worker (`conexao-solidaria-worker`) para o Docker Hub, com as tags `latest` e o SHA do commit.

> ⚠️ **Atenção:** este pipeline dispara em **Pull Request**, não em push direto na `main`. O edital do hackathon exige um pipeline acionado a cada push na branch principal — vale revisar se esse comportamento atende ao requisito antes da entrega final, ou ajustar o trigger para incluir `push: branches: [main]`.

---