# Conexão Solidária Worker

Background service built in C#/.NET to process donation events from the Conexão Solidária ecosystem. This worker listens to RabbitMQ messages, updates campaign fundraising totals, persists the data in PostgreSQL, and emits result events for downstream consumers.

## Overview

The Worker is responsible for handling donation events asynchronously, ensuring that each campaign's total amount is updated without blocking the main application flow. It consumes a donation queue, validates the campaign state, updates the aggregate value, and publishes the outcome of the operation.

## Technologies Used

- .NET 10 / C#
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- MassTransit
- RabbitMQ
- Docker / Docker Compose
- Kubernetes (Docker Desktop local cluster)
- Prometheus metrics integration

## Prerequisites

Before running the project, make sure you have:

- .NET SDK installed
- Docker Desktop running
- Kubernetes enabled in Docker Desktop (when using the Kubernetes deployment)
- `kubectl` configured to the local Docker Desktop cluster
- Access to the shared PostgreSQL instance used by the project

## Project Structure

```text
ConexaoSolidaria.Worker/
├── ConexaoSolidaria.Worker/        # Main project
│   ├── Events/                     # Message contracts
│   ├── Infrastructure/             # Consumers, jobs, persistence, extensions
│   ├── Models/                     # Domain models
│   ├── Migrations/                 # EF Core migrations
│   ├── k8s/                        # Kubernetes manifests
│   ├── Program.cs                  # Startup configuration
│   ├── appsettings.json            # Base configuration
│   ├── appsettings.Development.json # Local environment settings
│   ├── docker-compose.yml          # RabbitMQ local infrastructure
│   └── ConexaoSolidaria.Worker.csproj
├── .github/workflows/ci.yml        # CI/CD pipeline
├── README.md
├── ConexaoSolidaria.Worker.sln
└── .gitignore
```

## Local Execution

There are two common ways to run the environment:

- Local development via Docker Compose and direct `dotnet run`
- Kubernetes deployment for the final architecture replicated locally

### 1) Start Shared Infrastructure

Start RabbitMQ locally with Docker Compose:

```bash
docker compose up -d
```

This will start the RabbitMQ broker used by the worker.

### 2) Configure Local Settings

From the project folder, create or edit `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "Worker": "Host=localhost;Port=5432;Database=conexaosolidaria;Username=postgres;Password=your_password"
  },
  "Bus": {
    "Host": "localhost",
    "Port": "5672",
    "User": "guest",
    "Password": "guest"
  }
}
```

Important notes:

- If you are running the infrastructure locally with Docker Compose, set `Bus:Host` to `localhost`.
- If the worker is running inside a Kubernetes pod in Docker Desktop, use `host.docker.internal` instead of `localhost`.
- RabbitMQ default credentials are `guest / guest`.

### 3) Apply Database Migrations

The worker keeps its own database table set, including the `ArrecadacaoCampanha` aggregate table. Apply the migrations to the shared database:

```bash
dotnet ef database update
```

If the .NET Entity Framework CLI is not available, install it first:

```bash
dotnet tool install --global dotnet-ef
```

### 4) Run the Worker

```bash
dotnet run
```

The service will start using the project launch settings, which expose the app on the local URL configured in `Properties/launchSettings.json` (for example `http://localhost:5001`).

### 5) Validate Health and Metrics

The worker exposes a health endpoint and Prometheus metrics:

```bash
curl http://localhost:5001/health
curl http://localhost:5001/metrics
```

## Kubernetes Deployment

The project can also be executed fully inside a local Kubernetes cluster using Docker Desktop.

### 1) Build the Docker Image

Build the image with the exact tag expected by the manifests:

```bash
docker build -t conexao-solidaria-worker:latest .
```

> The image name must be exactly `conexao-solidaria-worker:latest`, because the Kubernetes manifests reference that tag.

### 2) Apply the Kubernetes Manifests

The `k8s/secrets.yaml` file is versioned as a template and contains blank sensitive values. Fill it in before using it in a non-local environment.

From the project directory:

```bash
cd k8s
kubectl apply -f namespace.yaml
kubectl apply -f .
```

### 3) Check Pod Status

```bash
kubectl get pods -n apps
```

The worker pod should appear in a `Running` state.

> Inside the Kubernetes cluster, the worker must connect to RabbitMQ and PostgreSQL using `host.docker.internal` instead of `localhost`.

## CI/CD

The pipeline is defined in `.github/workflows/ci.yml` and runs automatically on every pull request targeting the `main` branch or can be triggered manually via `workflow_dispatch`.

The workflow performs the following steps:

1. Restore dependencies for `ConexaoSolidaria.Worker.sln`
2. Build the solution in Release mode
3. Log in to Docker Hub
4. Build the worker image
5. Push the image with the `latest` tag and the current commit SHA

## Notes

- `Bus` configuration is loaded from `appsettings.Development.json` or environment variables in Kubernetes.
- The worker is designed to be resilient and decoupled from user-facing services, processing asynchronous donation events in the background.
- The project is intended to run alongside the main Conexão Solidária application, sharing infrastructure such as RabbitMQ and PostgreSQL.

## Troubleshooting

- If the worker cannot connect to RabbitMQ, confirm the `Bus:Host`, `Port`, `User`, and `Password` values.
- If migrations fail, verify that the PostgreSQL connection string is valid and the database is reachable.
- If the Kubernetes pod does not start, confirm that the image was built with the exact tag `conexao-solidaria-worker:latest`.
- If the service is unhealthy, check the application logs and ensure the database and message broker are running.

---

This README reflects the current implementation and expected local and Kubernetes execution flows for the project.
