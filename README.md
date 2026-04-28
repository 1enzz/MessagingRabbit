# 🐇 MessagingRabbit

Projeto de estudo em **C# .NET** focado em mensageria com **RabbitMQ**, explorando os conceitos de exchanges, filas, publicação e consumo de mensagens em uma arquitetura de microsserviços.

---

## 📋 Sobre o Projeto

O **MessagingRabbit** simula um fluxo de gerenciamento de produtos com comunicação assíncrona entre serviços via RabbitMQ. O objetivo é praticar os principais conceitos de mensageria como exchanges, routing keys, filas e consumidores.

---

## 🔄 Fluxos Implementados

### ✅ Criação de Produto
1. O **OrderService** recebe a requisição de criação de produto
2. Publica o evento `ProductCreated` em uma **Exchange** (Fanout ou Direct)
3. O **NotificationService** consome a mensagem da fila vinculada
4. Uma notificação de **"Produto criado com sucesso"** é gerada

### ✏️ Atualização de Produto
1. O **OrderService** recebe a requisição de atualização de produto
2. Publica o evento `ProductUpdated` diretamente em uma **Fila**
3. O **NotificationService** consome a mensagem
4. Uma notificação de **"Produto atualizado com sucesso"** é gerada

---

## 🏗️ Arquitetura

```
┌─────────────────┐        ┌─────────────────┐        ┌──────────────────────┐
│   OrderService  │───────▶│    RabbitMQ     │───────▶│ NotificationService  │
│                 │        │                 │        │                      │
│ POST /products  │  pub   │ Exchange/Queue  │  sub   │ Gera Notificação     │
│ PUT  /products  │───────▶│                 │───────▶│                      │
└─────────────────┘        └─────────────────┘        └──────────────────────┘
```

---

## 🛠️ Tecnologias

| Tecnologia | Versão | Uso |
|---|---|---|
| C# / .NET | 10 | Plataforma base |
| RabbitMQ | 3.x | Broker de mensagens |
| RabbitMQ.Client | Latest | SDK para .NET |
| ASP.NET Core | 10 | APIs REST |
| Docker | Latest | Subir o RabbitMQ localmente |

---

## 📁 Estrutura do Projeto

```
MessagingRabbit/
├── src/
│   ├── OrderService/
│   │   ├── OrderService.API/
│   │   ├── OrderService.Application/
│   │   ├── OrderService.Domain/
│   │   └── OrderService.Infrastructure/
│   │
│   └── NotificationService/
│       ├── NotificationService.API/
│       ├── NotificationService.Application/
│       ├── NotificationService.Domain/
│       └── NotificationService.Infrastructure/
│
└── MessagingDemo.slnx
```

---

## 🚀 Como Executar

### Pré-requisitos
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/)

### 1. Subir o RabbitMQ com Docker

```bash
docker run -d \
  --name rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  rabbitmq:3-management
```

Acesse o painel de administração em: [http://localhost:15672](http://localhost:15672)
- **Usuário:** `guest`
- **Senha:** `guest`

### 2. Clonar o repositório

```bash
git clone https://github.com/1enzz/MessagingRabbit.git
cd MessagingRabbit
```

### 3. Executar os serviços

Em terminais separados:

```bash
# Terminal 1 - OrderService
cd src/OrderService/OrderService.API
dotnet run

# Terminal 2 - NotificationService
cd src/NotificationService/NotificationService.API
dotnet run
```

---

## 📡 Endpoints

### OrderService

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/orders` | Cria um novo produto e publica evento na Exchange |
| `PUT` | `/orders/{id}` | Atualiza um produto e publica evento na fila |

---

## 📚 Conceitos Praticados

- **Exchange** — roteamento de mensagens para múltiplas filas
- **Queue** — fila de mensagens para consumo direto
- **Publisher** — publicação de eventos entre serviços
- **Consumer** — consumo assíncrono de mensagens
- **Routing Key** — chave de roteamento de mensagens
- **Fanout / Direct Exchange** — tipos de exchange utilizados

---

## 📖 Referências

- [RabbitMQ Docs](https://www.rabbitmq.com/documentation.html)
- [RabbitMQ com .NET - Getting Started](https://www.rabbitmq.com/tutorials/tutorial-one-dotnet)
- [Documentação ASP.NET Core](https://learn.microsoft.com/aspnet/core)

---

> Projeto desenvolvido para fins de estudo de mensageria e arquitetura de microsserviços.
