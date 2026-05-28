# ShoppingPlanner.Api

> REST API for shopping list management. Built with ASP.NET Core 8.
> **Status: work in progress — Week 2 of an 8-week build.** See the [Roadmap](#roadmap) for what is done and what is planned.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![License](https://img.shields.io/badge/license-MIT-blue.svg)
![Status](https://img.shields.io/badge/status-work_in_progress-orange)

A backend service that will let users manage shopping lists shared across devices. The end goal: authenticated users create lists, add products with categories and quantities, mark items as purchased, and retrieve their history.

I'm building it incrementally and in public — each week adds one production-grade capability (persistence, auth, containerization, CI, deployment). The README is kept honest: it describes what actually runs today, not the final vision.

---

## What works today (Week 2)

A working in-memory CRUD API for the `Product` resource, with a clean controller/service/DTO separation that the rest of the project will build on.

- **`Product` CRUD** — five REST endpoints with correct HTTP semantics:
  - `GET /api/products` — list all (returns `200` + `[]` when empty)
  - `GET /api/products/{id}` — `200` if found, `404` if not
  - `POST /api/products` — `201 Created` + `Location` header
  - `PUT /api/products/{id}` — `200` if found, `404` if not
  - `DELETE /api/products/{id}` — `204 No Content`, `404` if not found
- **DTO / domain separation** — `CreateProductDto`, `UpdateProductDto`, `ProductDto` kept separate from the `Product` domain model.
- **Validation** — DataAnnotations on DTOs, surfaced automatically by `[ApiController]`. Invalid input returns `400` with an RFC 7807 `ProblemDetails` body (`content-type: application/problem+json`).
- **Service behind an interface** — `IProductService` / `ProductService`, registered in DI. This is the seam where in-memory storage will be swapped for EF Core in Week 3 without touching the controller.
- **Swagger / OpenAPI** — interactive docs; all endpoints annotated with `[ProducesResponseType]` so the documented response codes match the real ones.
- **Tests** — 11 xUnit unit tests for `ProductService` (AAA style), build is warning-free.

Storage is an in-memory `List<Product>` for now — data resets on restart. PostgreSQL persistence lands in Week 3.

---

## Why this project exists

I built this as a hands-on way to modernize my .NET stack from legacy ASP.NET / VB.NET to the current ecosystem (ASP.NET Core 8, EF Core). The goal is a small but production-grade backend with everything a real service needs: authentication, persistence, tests, CI, containerization, and a public deployment — added one step at a time.

An earlier iteration of the planner lived as a desktop WPF app in my [ClaudeExperiments](https://github.com/IzabellaGerman/ClaudeExperiments) repository. This is the rebuilt server-side version.

---

## Tech stack

Legend: **[x]** in use today · **[ ]** planned (see Roadmap).

| Layer | Technology | Status |
|---|---|---|
| Language | C# 12 | [x] |
| Framework | ASP.NET Core 8 (Web API) | [x] |
| API documentation | Swagger / OpenAPI (Swashbuckle) | [x] |
| Validation | DataAnnotations + `ProblemDetails` (RFC 7807) | [x] |
| Testing | xUnit | [x] |
| ORM | Entity Framework Core 8 | [ ] Week 3 |
| Database | PostgreSQL 16 | [ ] Week 3 |
| Authentication | JWT Bearer + ASP.NET Core Identity | [ ] Week 4 |
| Containerization | Docker, docker-compose | [ ] Week 4 |
| CI/CD | GitHub Actions | [ ] Week 6 |
| Hosting | TBD (Railway / Render / Fly.io) | [ ] Week 7 |

---

## Target architecture

This is where the project is heading. Today only the **Controllers -> Services -> DTOs** part exists; repositories, EF Core, and PostgreSQL arrive in Week 3.

```
+-------------+
|   Client    |  (Swagger UI today; SPA / mobile later)
+------+------+
       | HTTP (+ JWT, from Week 4)
+------v--------------------------------------+
|  ShoppingPlanner.Api                        |
|                                             |
|  Controllers -> Services -> (Repositories)  |
|       |            |             |          |
|       |            |             +- EF Core |  (Week 3)
|       |            +- Business logic        |
|       +- DTOs, validation, auth             |
+------+--------------------------------------+
       |
+------v------+
| PostgreSQL  |  (Week 3)
+-------------+
```

Layering:

- **Controllers** — thin HTTP layer: routing, status codes, DTO in/out.
- **Services** — business logic behind an interface (`IProductService`).
- **Repositories** — *(planned)* EF Core queries behind interfaces.
- **Domain entities** — `Product` today; `Category`, `ShoppingList`, `ShoppingListItem`, `User` to follow.

---

## Getting started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

No database or Docker needed yet — storage is in-memory at this stage.

### Run it

```bash
git clone https://github.com/IzabellaGerman/ShoppingPlanner.Api.git
cd ShoppingPlanner.Api
dotnet run --project src/ShoppingPlanner.Api
```

Then open Swagger UI at the URL printed in the console (e.g. `http://localhost:5271/swagger`).

### Run the tests

```bash
dotnet test
```

---

## API example

### Create a product

```http
POST /api/products
Content-Type: application/json

{
  "name": "Sourdough Bread",
  "category": "Bakery",
  "defaultUnit": "pcs"
}
```

Response — `201 Created`, with a `Location` header pointing at the new resource:

```json
{
  "id": 1,
  "name": "Sourdough Bread",
  "category": "Bakery",
  "defaultUnit": "pcs",
  "createdAt": "2026-05-28T17:10:12.0431969Z"
}
```

Invalid input (e.g. empty `name`) returns `400` with a `ProblemDetails` body:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": ["The Name field is required."]
  },
  "traceId": "00-..."
}
```

Full reference is in the Swagger UI.

---

## Project structure

```
ShoppingPlanner.Api/
+-- src/
|   +-- ShoppingPlanner.Api/
|       +-- Controllers/        # ProductsController
|       +-- Services/           # IProductService, ProductService
|       +-- Models/             # Product (domain entity)
|       +-- Dtos/               # CreateProductDto, UpdateProductDto, ProductDto
|       +-- Program.cs
+-- tests/
|   +-- ShoppingPlanner.Api.Tests/   # ProductServiceTests (xUnit)
+-- README.md
+-- LICENSE
```

Folders for repositories, migrations, middleware, and infrastructure will be added as the corresponding features land.

---

## Roadmap

- [x] **Week 1** — project skeleton, Git workflow (feature branch -> PR -> main), README, LICENSE
- [x] **Week 2** — in-memory `Product` CRUD, DTOs, validation + `ProblemDetails`, Swagger, xUnit tests
- [ ] **Week 3** — PostgreSQL + EF Core migrations, `Category` entity, navigation properties, async everywhere
- [ ] **Week 4** — `ShoppingList` / `ShoppingListItem` domain, global exception handling, integration tests
- [ ] **Week 5–6** — JWT authentication + refresh tokens, ownership checks, Docker Compose
- [ ] **Week 6** — GitHub Actions CI (build -> test -> Docker image)
- [ ] **Week 7** — deployment to a free host, live Swagger link in this README
- [ ] **Future** — pagination, structured logging, list sharing, AI-suggested categories, mobile client

---

## About the author

I'm Izabella, a C#/.NET developer based in Prague with 6 years of enterprise experience (financial reporting and ERP systems). After a career break, I'm modernizing my stack and looking for mid-level backend roles in Prague or remote across the EU. This repository is part of that — built in the open, one production-grade step at a time.

- LinkedIn: [izabella-german](https://www.linkedin.com/in/izabella-german/)
- GitHub: [@IzabellaGerman](https://github.com/IzabellaGerman)

---

## License

MIT — see [LICENSE](LICENSE) for details.
