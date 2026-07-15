# ShoppingPlanner.Api

> REST API for shopping list management. Built with ASP.NET Core 8.
> **Status: work in progress — Week 6 of an 8-week build.** See the [Roadmap](#roadmap) for what is done and what is planned.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![License](https://img.shields.io/badge/license-MIT-blue.svg)
![Status](https://img.shields.io/badge/status-work_in_progress-orange)
![CI](https://github.com/IzabellaGerman/ShoppingPlanner.Api/actions/workflows/ci.yml/badge.svg)
[![Swagger UI](https://img.shields.io/badge/Swagger-live-brightgreen)](https://shoppingplannerapi-production.up.railway.app/swagger)


A backend service that will let users manage shopping lists shared across devices. The end goal: authenticated users create lists, add products with categories and quantities, mark items as purchased, and retrieve their history.

I'm building it incrementally and in public — each week adds one production-grade capability (persistence, auth, containerization, CI, deployment). The README is kept honest: it describes what actually runs today, not the final vision.

---

## What works today (Week 6)

🔗 **Live API:** [Swagger UI](https://shoppingplannerapi-production.up.railway.app/swagger)

A production-grade REST API with authentication, PostgreSQL persistence, and full containerization.

- **JWT authentication** — `POST /api/auth/register` and `POST /api/auth/login`; protected endpoints require Bearer token
- **Three domain entities** — `Product`, `Category`, `ShoppingList` / `ShoppingListItem` with navigation properties
- **EF Core + PostgreSQL** — async everywhere, Fluent API configuration, migrations applied automatically on startup
- **Global exception handling** — `IExceptionHandler` (.NET 8), all errors return RFC 7807 `ProblemDetails`
- **Docker** — `docker compose up` starts API + PostgreSQL; multi-stage Dockerfile keeps image at ~335MB
- **GitHub Actions CI** — build → test → docker build on every push and PR
- **Swagger / OpenAPI** — JWT Bearer configured, all endpoints documented with `[ProducesResponseType]`
- **16 xUnit tests** — EF Core InMemory provider, `NullLogger<T>.Instance`, AAA style

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
| Testing | xUnit + Moq + EF Core InMemory | [x] |
| ORM | Entity Framework Core 8 | [x] |
| Database | PostgreSQL 16 | [x] |
| Logging | `ILogger<T>` (built-in) | [x] |
| Authentication | JWT Bearer + ASP.NET Core Identity | [x] |
| Containerization | Docker, docker-compose | [x] |
| CI/CD | GitHub Actions | [x] |
| Hosting | TBD (Railway / Render / Fly.io) | [ ] Week 7 |

---

## Target architecture

```
+-------------+
|   Client    |  (Swagger UI today; SPA / mobile later)
+------+------+
       | HTTP (+ JWT, from Week 5)
+------v--------------------------------------+
|  ShoppingPlanner.Api                        |
|                                             |
|  Controllers -> Services -> AppDbContext    |
|       |            |             |          |
|       |            |             +- EF Core |
|       |            +- Business logic        |
|       +- DTOs, validation, logging          |
+------+--------------------------------------+
       |
+------v------+
| PostgreSQL  |
+-------------+
```

Layering:

- **Controllers** — thin HTTP layer: routing, status codes, DTO in/out.
- **Services** — business logic behind an interface (`IProductService`, `ICategoryService`, `IShoppingListService`).
- **AppDbContext** — EF Core, injected directly into services (no repository wrapper — deliberate choice, see design notes).
- **Domain entities** — `Product`, `Category`, `ShoppingList`, `ShoppingListItem`; `User` to follow.

---

## Getting started

> **Live demo:** [https://shoppingplannerapi-production.up.railway.app/swagger](https://shoppingplannerapi-production.up.railway.app/swagger)

### Option A — Docker (recommended)

No .NET SDK or PostgreSQL needed.

```bash
git clone https://github.com/IzabellaGerman/ShoppingPlanner.Api.git
cd ShoppingPlanner.Api
docker compose up
```

Then open `http://localhost:8080/swagger`.

### Option B — Local

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- PostgreSQL 16 running on port 5432

```bash
git clone https://github.com/IzabellaGerman/ShoppingPlanner.Api.git
cd ShoppingPlanner.Api
dotnet run --project src/ShoppingPlanner.Api
```

### Configure the database

Set the connection string in `src/ShoppingPlanner.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=shoppingplanner;Username=postgres;Password=yourpassword"
  }
}
```

### Run migrations

```bash
dotnet tool run dotnet-ef database update --project src/ShoppingPlanner.Api
```


### Run the tests

```bash
dotnet test
```

---

## API example

### Create a shopping list

```http
POST /api/shopping-lists
Content-Type: application/json

{
  "name": "Weekend shopping",
  "items": [
    { "productId": 1, "quantity": 2, "note": "fresh" }
  ]
}
```

Response — `201 Created`:

```json
{
  "id": 1,
  "name": "Weekend shopping",
  "createdAt": "2026-07-08T10:00:00Z",
  "items": [
    {
      "id": 1,
      "productId": 1,
      "productName": "Milk",
      "quantity": 2,
      "note": "fresh",
      "isCompleted": false
    }
  ]
}
```

Invalid input returns `400` with a `ProblemDetails` body. Unhandled server errors return `500` with a safe `ProblemDetails` — no stack traces exposed to clients.

Full reference is in the Swagger UI.

---

## Project structure

```
ShoppingPlanner.Api/
+-- src/
|   +-- ShoppingPlanner.Api/
|       +-- Controllers/        # ProductsController, CategoriesController, ShoppingListsController
|       +-- Services/           # IProductService, ProductService, ICategoryService, CategoryService,
|       |                       # IShoppingListService, ShoppingListService
|       +-- Models/             # Product, Category, ShoppingList, ShoppingListItem
|       +-- Dtos/               # DTOs per entity per operation
|       +-- Data/               # AppDbContext, Migrations/
|       +-- Middleware/         # GlobalExceptionHandler
|       +-- Program.cs
+-- tests/
|   +-- ShoppingPlanner.Api.Tests/   # ProductServiceTests, CategoryServiceTests,
|                                    # ProductsControllerTests, ShoppingListServiceTests
+-- README.md
+-- INTERVIEW_NOTES.md
+-- LICENSE
```

---

## Roadmap

- [x] **Week 1** — project skeleton, Git workflow (feature branch -> PR -> main), README, LICENSE
- [x] **Week 2** — in-memory `Product` CRUD, DTOs, validation + `ProblemDetails`, Swagger, xUnit tests
- [x] **Week 3** — PostgreSQL + EF Core migrations, `Category` entity, navigation properties, async everywhere
- [x] **Week 4** — `ShoppingList` / `ShoppingListItem` domain, global exception handling (`IExceptionHandler`), structured logging, 16 unit tests
- [x] **Week 5** — JWT authentication + ASP.NET Core Identity, ownership checks
- [x] **Week 6** — Docker Compose, GitHub Actions CI (build -> test -> Docker image)
- [ ] **Week 7** — deployment to a free host, live Swagger link in this README
- [ ] **Future** — pagination, list sharing, AI-suggested categories, mobile client

---

## About the author

I'm Izabella, a C#/.NET developer based in Prague with 6 years of enterprise experience (financial reporting and ERP systems). After a career break, I'm modernizing my stack and looking for mid-level backend roles in Prague or remote across the EU. This repository is part of that — built in the open, one production-grade step at a time.

- LinkedIn: [izabella-german](https://www.linkedin.com/in/izabella-german/)
- GitHub: [@IzabellaGerman](https://github.com/IzabellaGerman)

---

## License

MIT — see [LICENSE](LICENSE) for details.
