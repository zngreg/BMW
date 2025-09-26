# BMW Book Online Shop Services (.NET 9)

Three services, dockerized, minimal, stateless.

- Book Catalogue Service: add, list, get books. Logs to auditing via UDP.
- Order Service: create and get orders. Checks book existence via HTTP to Book service. Logs to auditing via UDP.
- Auditing Service: no public API. Listens on UDP and prints messages.

## Prerequisites

- Docker and Docker Compose
- .NET 9 SDK if you want to run locally without Docker

## Run

```bash
docker compose up --build
```

Endpoints:

- Book: http://localhost:8080/swagger
- Order: http://localhost:8081/swagger
- Auditing health: http://localhost:8082/health
  Console of auditing container will show [AUDIT] messages.

## Quick test

Use `test.http` or curl:

1. Add a book

```bash
curl -sX POST http://localhost:8080/books -H "Content-Type: application/json" -d '{"title":"DDD","author":"Evans","price":799.00}'
```

2. List books

```bash
curl -s http://localhost:8080/books
```

3. Place an order (replace BOOK_ID)

```bash
curl -sX POST http://localhost:8081/orders -H "Content-Type: application/json" -d '{"bookId":"BOOK_ID","quantity":2}'
```

Check auditing logs in the `auditing-service` container.
