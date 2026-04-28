# MessagingDemo

Projeto de estudo para aplicação e aprimoramento dos conceitos de mensageria com .NET, RabbitMQ e MassTransit.  
Composto por dois microsserviços independentes que se comunicam de forma assíncrona através de um broker de mensagens.

---

## Visão Geral

```
┌─────────────────┐        ┌──────────────┐        ┌──────────────────────┐
│  OrderService   │──────▶ │   RabbitMQ   │──────▶ │ NotificationService  │
│  :5100          │        │  :5672/15672 │        │ :5200                │
└─────────────────┘        └──────────────┘        └──────────────────────┘

  POST /orders  ──────────── Publish/Subscribe ──────▶  OrderCreatedConsumer
  PATCH /orders/{id}/status ── Queue ─────────────────▶  OrderStatusChangedConsumer
```

---

## Tecnologias

- **.NET 10** — plataforma base
- **ASP.NET Core Minimal APIs** — camada HTTP
- **MassTransit 10** — abstração de mensageria
- **RabbitMQ 3** — broker de mensagens
- **Swashbuckle (Swagger)** — documentação dos endpoints
- **Docker** — execução local do RabbitMQ

---

## Arquitetura

Ambos os serviços seguem **Clean Architecture** com quatro camadas por serviço:

```
┌─────────────────────────────────────────────┐
│                    API                      │  Endpoints HTTP, Swagger, Program.cs
├─────────────────────────────────────────────┤
│               Application                  │  Casos de uso, DTOs, Interfaces, Consumers
├─────────────────────────────────────────────┤
│               Infrastructure               │  Repositórios, MessagePublisher, config MassTransit
├─────────────────────────────────────────────┤
│                  Domain                    │  Entidades, Enums, Interfaces de repositório
└─────────────────────────────────────────────┘
```

**Regra de dependência:** as setas sempre apontam para dentro. Domain não conhece nenhuma camada. Infrastructure conhece Application e Domain. API conhece Application e Infrastructure.

---

## Estrutura de Pastas

```
messaging-demo/
├── MessagingDemo.slnx
└── src/
    ├── Shared.Contracts/
    │   └── Messages/
    │       ├── OrderCreated.cs             ← contrato Pub/Sub
    │       └── OrderStatusChanged.cs       ← contrato Queue
    │
    ├── OrderService/
    │   ├── OrderService.Domain/
    │   │   ├── Entities/
    │   │   │   ├── Order.cs
    │   │   │   └── OrderItem.cs
    │   │   ├── Enums/
    │   │   │   └── OrderStatus.cs
    │   │   └── Interfaces/
    │   │       └── IOrderRepository.cs
    │   │
    │   ├── OrderService.Application/
    │   │   ├── DTOs/
    │   │   │   ├── CreateOrderRequest.cs
    │   │   │   ├── UpdateOrderStatusRequest.cs
    │   │   │   ├── OrderItemDto.cs
    │   │   │   └── OrderResponse.cs
    │   │   ├── Interfaces/
    │   │   │   ├── IOrderService.cs
    │   │   │   └── IMessagePublisher.cs
    │   │   └── Services/
    │   │       └── OrderAppService.cs
    │   │
    │   ├── OrderService.Infrastructure/
    │   │   ├── Messaging/
    │   │   │   └── MessagePublisher.cs     ← implementa IMessagePublisher com MassTransit
    │   │   ├── Repositories/
    │   │   │   └── InMemoryOrderRepository.cs
    │   │   └── DependencyInjection.cs
    │   │
    │   └── OrderService.API/
    │       ├── Endpoints/
    │       │   └── OrderEndpoints.cs
    │       └── Program.cs
    │
    └── NotificationService/
        ├── NotificationService.Domain/
        │   ├── Entities/
        │   │   └── Notification.cs
        │   ├── Enums/
        │   │   └── NotificationType.cs
        │   └── Interfaces/
        │       └── INotificationRepository.cs
        │
        ├── NotificationService.Application/
        │   ├── Consumers/
        │   │   ├── OrderCreatedConsumer.cs          ← Pub/Sub
        │   │   └── OrderStatusChangedConsumer.cs    ← Queue
        │   ├── DTOs/
        │   │   ├── CreateNotificationRequest.cs
        │   │   └── NotificationResponse.cs
        │   ├── Interfaces/
        │   │   └── INotificationService.cs
        │   └── Services/
        │       └── NotificationAppService.cs
        │
        ├── NotificationService.Infrastructure/
        │   ├── Repositories/
        │   │   └── InMemoryNotificationRepository.cs
        │   └── DependencyInjection.cs               ← registra consumers no MassTransit
        │
        └── NotificationService.API/
            ├── Endpoints/
            │   └── NotificationEndpoints.cs
            └── Program.cs
```

---

## Padrões de Mensageria

### Publish/Subscribe — `OrderCreated`

Disparado quando um novo pedido é criado. Segue o padrão fanout: qualquer serviço inscrito recebe uma cópia da mensagem. O OrderService não sabe quem vai consumir.

```
OrderAppService.Create()
        │
        ▼
IPublishEndpoint.Publish<OrderCreated>()
        │
        ▼
RabbitMQ Exchange (Fanout) — "Shared.Contracts.Messages:OrderCreated"
        │
        ▼
Queue ──▶ OrderCreatedConsumer (NotificationService)
```

### Queue — `OrderStatusChanged`

Disparado quando o status de um pedido é atualizado. Enviado diretamente para uma fila nomeada. Apenas um consumer processa cada mensagem.

```
OrderAppService.UpdateStatus()
        │
        ▼
ISendEndpointProvider.GetSendEndpoint("queue:order-status-changed")
        │
        ▼
RabbitMQ Queue — "order-status-changed"
        │
        ▼
OrderStatusChangedConsumer (NotificationService)
```

---

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/products/docker-desktop)
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) ou VS Code

---

## Como Executar

### 1. Subir o RabbitMQ

```bash
docker run -d \
  --name rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  rabbitmq:3-management
```

Acesse a Management UI em: [http://localhost:15672](http://localhost:15672)  
Credenciais: `guest` / `guest`

### 2. Rodar os serviços

**Via terminal** — abra dois terminais na raiz do projeto:

```bash
# Terminal 1
dotnet run --project src/OrderService/OrderService.API

# Terminal 2
dotnet run --project src/NotificationService/NotificationService.API
```

**Via Visual Studio** — clique com botão direito na Solution → *Set Startup Projects* → *Multiple startup projects* → marque `OrderService.API` e `NotificationService.API` como *Start* → `F5`.

---

## Endpoints

### OrderService — `http://localhost:5100`

| Método | Rota | Descrição | Mensagem publicada |
|--------|------|-----------|-------------------|
| `GET` | `/orders` | Lista todos os pedidos | — |
| `GET` | `/orders/{id}` | Busca pedido por ID | — |
| `POST` | `/orders` | Cria novo pedido | `OrderCreated` (Pub/Sub) |
| `PATCH` | `/orders/{id}/status` | Atualiza status | `OrderStatusChanged` (Queue) |

**Swagger:** [http://localhost:5100/swagger](http://localhost:5100/swagger)

#### Exemplo — Criar pedido

```json
POST /orders
{
  "customerName": "João Silva",
  "items": [
    {
      "productName": "Notebook",
      "quantity": 1,
      "unitPrice": 4500.00
    }
  ]
}
```

#### Exemplo — Atualizar status

```json
PATCH /orders/{id}/status
{
  "status": 2
}
```

> Status: `1` Pending · `2` Confirmed · `3` Shipped · `4` Delivered · `0` Cancelled

---

### NotificationService — `http://localhost:5200`

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/notifications` | Lista todas as notificações |
| `GET` | `/notifications/{id}` | Busca notificação por ID |
| `POST` | `/notifications` | Cria notificação manualmente |
| `PATCH` | `/notifications/{id}/read` | Marca como lida |

**Swagger:** [http://localhost:5200/swagger](http://localhost:5200/swagger)

---

## Contratos de Mensagem

Localizados em `Shared.Contracts/Messages/`, referenciados por ambos os serviços.

```csharp
// Pub/Sub — criação de pedido
public record OrderCreated
{
    public Guid OrderId { get; init; }
    public string CustomerName { get; init; }
    public decimal TotalOrder { get; init; }
    public DateTime CreatedOn { get; init; }
}

// Queue — atualização de status
public record OrderStatusChanged
{
    public Guid OrderId { get; init; }
    public string OrderStatus { get; init; }
    public DateTime ChangedAt { get; init; }
}
```

---

## Dependências entre Projetos

```
OrderService.API
  └── OrderService.Application
        └── OrderService.Domain
  └── OrderService.Infrastructure
        └── OrderService.Domain
        └── OrderService.Application
        └── Shared.Contracts

NotificationService.API
  └── NotificationService.Application
        └── NotificationService.Domain
        └── Shared.Contracts
  └── NotificationService.Infrastructure
        └── NotificationService.Domain
        └── NotificationService.Application
        └── Shared.Contracts
```
