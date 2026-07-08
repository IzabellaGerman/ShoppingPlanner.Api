# ShoppingPlanner.Api — недельный план

Operational-документ по pet-проекту. Используется параллельно с карьерным планом — там стратегия и месяцы, здесь конкретные шаги по дням и неделям.

**Repo:** https://github.com/IzabellaGerman/ShoppingPlanner.Api
**Стек:** ASP.NET Core 8 Web API, EF Core, PostgreSQL, JWT, Docker
**Цель проекта:** production-grade backend в портфолио, который заменит "учебные эксперименты" на GitHub и подтвердит уровень из LinkedIn.

---

## Месяц 1 — Refresh + ShoppingPlanner v2 (production-grade backend)

### ✅ Неделя 1 — Git + initial setup (СДЕЛАНО)

- [x] Git refresh: LearnGitBranching.js.org
- [x] Создан новый репо `ShoppingPlanner.Api`
- [x] Initial ASP.NET Core 8 Web API project setup
- [x] Решение по версии: .NET 8 LTS (а не .NET 11)
- [x] Структура: `src/`, `tests/`, `.sln`, `.gitignore`, `LICENSE`, `README.md`
- [x] Первый PR закрыт в main (`feat: initial ASP.NET Core 8 Web API project setup (#1)`)

---

### ✅ Неделя 2 — C# refresh + первый CRUD (в памяти) (СДЕЛАНО)

- [x] `Product` модель + DTO (`CreateProductDto`, `UpdateProductDto`, `ProductDto`)
- [x] `IProductService` / `ProductService` с in-memory `List<Product>`
- [x] `ProductsController` — 5 эндпоинтов, правильные HTTP-коды
- [x] Валидация через DataAnnotations, `[ApiController]`
- [x] Swagger / OpenAPI с `[ProducesResponseType]`
- [x] 11 xUnit unit-тестов для `ProductService` (AAA)
- [x] PR `feature/products-crud-in-memory` → main

---

### ✅ Неделя 3 — EF Core + PostgreSQL (СДЕЛАНО)

- [x] Нативный PostgreSQL на Windows (порт 5432), база `shoppingplanner`
- [x] `AppDbContext`, пакеты Npgsql + EF Core Design
- [x] Миграции: `InitialCreate`, `AddCategories`, `SeedData`
- [x] `ProductService` переписан на async (EF Core)
- [x] `Category` сущность, связь many-to-one с `Product`
- [x] `CategoriesController` — полный CRUD
- [x] Seed-данные: 5 категорий, 15 продуктов
- [x] Тесты обновлены (EF Core InMemory provider + Moq для контроллеров)
- [x] PR `feature/ef-core-postgres` → main

---

### ✅ Неделя 4 — Доменная логика + полировка месяца 1 (СДЕЛАНО)

- [x] `ShoppingList` и `ShoppingListItem` сущности, миграция `AddShoppingLists`
- [x] `DeleteBehavior.Cascade` для Items, `DeleteBehavior.Restrict` для Product (задокументировано в `INTERVIEW_NOTES.md`)
- [x] `ShoppingListsController` — 8 эндпоинтов (CRUD + item-операции)
- [x] `ShoppingListService` — все async методы, `MapToDto` helper
- [x] Глобальный exception handler (`IExceptionHandler`, .NET 8), RFC 7807 `ProblemDetails`
- [x] Структурное логирование через `ILogger<T>`
- [x] 16 xUnit unit-тестов для `ShoppingListService` (EF Core InMemory + `NullLogger.Instance`)
- [x] README обновлён до Week 4
- [x] PR `feature/shopping-lists` + `feature/shopping-list-tests` + `docs/update-readme` → main

**Definition of Done месяца 1 — выполнено:**
- Работающее API с тремя сущностями (Product, Category, ShoppingList) на PostgreSQL ✅
- Async везде ✅
- Swagger показывает осмысленные эндпоинты с DTO ✅
- 16 тестов, все зелёные ✅
- README выглядит как у production-проекта ✅
- Регулярные коммиты через PR-flow на GitHub ✅

---

## Месяц 2 — Auth + Infrastructure (portfolio-ready)

### Неделя 5 — JWT аутентификация

**Цель:** пользователи могут регистрироваться, логиниться и получать JWT-токен. Защищённые эндпоинты доступны только с токеном.

**Учёба (3–4 ч):**
- [ ] Nick Chapsas — "ASP.NET Core JWT Authentication" на YouTube (~1.5 часа)
- [ ] Microsoft Docs — "Overview of ASP.NET Core authentication" (~30 мин)
- [ ] Прочитать про разницу `Authentication` vs `Authorization` — на собесе спрашивают всегда

**Код (8–10 ч):**

День 1 — User модель и Identity:
- [ ] Добавить пакет `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
- [ ] Создать `ApplicationUser : IdentityUser` в `Models/`
- [ ] Изменить `AppDbContext : IdentityDbContext<ApplicationUser>`
- [ ] Миграция `AddIdentity`. Применить.
- [ ] **Почему Identity, а не вручную:** готовая реализация хэширования паролей, управления пользователями, claims. На собесе: "не изобретаем колесо".

День 2 — JWT настройка:
- [ ] Добавить пакет `Microsoft.AspNetCore.Authentication.JwtBearer`
- [ ] В `appsettings.json` добавить секцию `Jwt`: `Key`, `Issuer`, `Audience`, `ExpiresInMinutes`
- [ ] В `Program.cs` настроить `AddAuthentication` + `AddJwtBearer`
- [ ] `app.UseAuthentication()` — до `app.UseAuthorization()` (порядок важен!)
- [ ] **На собесе:** объяснить разницу между `UseAuthentication` и `UseAuthorization`

День 3 — Register / Login эндпоинты:
- [ ] Создать `AuthController` с двумя эндпоинтами: `POST /api/auth/register` и `POST /api/auth/login`
- [ ] DTOs: `RegisterDto` (Email, Password), `LoginDto` (Email, Password), `AuthResponseDto` (Token, ExpiresAt)
- [ ] `Register`: создать юзера через `UserManager<ApplicationUser>.CreateAsync`
- [ ] `Login`: проверить пароль через `UserManager.CheckPasswordAsync`, сгенерировать токен
- [ ] Метод `GenerateJwtToken(ApplicationUser user)` — приватный, в контроллере или отдельном сервисе

День 4 — Защита эндпоинтов:
- [ ] Добавить `[Authorize]` на `ShoppingListsController`
- [ ] Добавить `UserId` поле в `ShoppingList` модель — миграция `AddUserIdToShoppingList`
- [ ] В `ShoppingListService` фильтровать списки по `UserId` из claims токена
- [ ] Вспомогательный метод: получить `userId` из `HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)`
- [ ] **На собесе:** объяснить claims — это данные внутри JWT-токена, не требуют обращения к БД при каждом запросе

День 5 — Тесты для Auth:
- [ ] Unit-тест: `GenerateJwtToken` возвращает токен с правильными claims
- [ ] Обновить существующие `ShoppingListService` тесты — добавить `UserId` в seed-данные
- [ ] **Не тестировать Identity напрямую** — это внешняя библиотека, тестируем свою логику вокруг неё

День 6–7 — Полировка и PR:
- [ ] Проверить в Swagger: Register → Login → скопировать токен → Authorize → защищённые эндпоинты работают
- [ ] Swagger настроить для JWT: `AddSecurityDefinition` + `AddSecurityRequirement` в `Program.cs`
- [ ] Добавить в `INTERVIEW_NOTES.md`: JWT структура (header.payload.signature), stateless auth, claims
- [ ] PR `feature/jwt-auth` → main

**Definition of Done:**
- `POST /api/auth/register` создаёт пользователя
- `POST /api/auth/login` возвращает JWT токен
- `GET /api/shopping-lists` без токена → 401
- `GET /api/shopping-lists` с токеном → только списки этого пользователя
- Swagger UI позволяет авторизоваться и тестировать защищённые эндпоинты
- Тесты зелёные

---

### Неделя 6 — Docker + GitHub Actions CI

**Цель:** весь стек запускается одной командой через Docker Compose. GitHub Actions запускает build и тесты на каждый PR.

**Учёба (2–3 ч):**
- [ ] Docker официальный туториал "Get started" — часть 1–3 (~1 час)
- [ ] GitHub Actions quickstart (~30 мин)

**Код (6–8 ч):**

День 1–2 — Dockerfile:
- [ ] Создать `Dockerfile` в корне репо (multi-stage build):
  - Stage 1 `build`: `mcr.microsoft.com/dotnet/sdk:8.0` — restore, build, publish
  - Stage 2 `runtime`: `mcr.microsoft.com/dotnet/aspnet:8.0` — копируем publish output
- [ ] Проверить: `docker build -t shoppingplanner-api .` собирается без ошибок
- [ ] `docker run -p 8080:8080 shoppingplanner-api` — API отвечает
- [ ] **На собесе:** объяснить multi-stage build — зачем два слоя (SDK тяжелее runtime, в production не нужен компилятор)

День 3 — Docker Compose:
- [ ] Создать `docker-compose.yml` в корне:
  - сервис `api` — из нашего Dockerfile, порт 8080
  - сервис `db` — `postgres:16`, volume для данных, env-переменные
  - зависимость: `api` depends_on `db`
- [ ] Connection string в `docker-compose.yml` через env-переменную `ConnectionStrings__DefaultConnection`
- [ ] `docker compose up` — оба сервиса стартуют, API подключается к БД
- [ ] **На собесе:** volumes — почему важны (данные не пропадают при перезапуске контейнера)

День 4 — Environment и secrets:
- [ ] `.env` файл для локальных секретов (JWT Key, DB password) — добавить в `.gitignore`
- [ ] Убедиться, что в `appsettings.json` нет реальных паролей
- [ ] README: обновить "Getting started" — теперь есть два способа запуска (локально и через Docker)

День 5–6 — GitHub Actions:
- [ ] Создать `.github/workflows/ci.yml`
- [ ] Триггер: `push` и `pull_request` на `main`
- [ ] Jobs:
  - `build`: `dotnet restore` → `dotnet build --no-restore`
  - `test`: `dotnet test --no-build`
- [ ] Проверить: открыть PR → GitHub Actions запускается → зелёная галочка
- [ ] Добавить badge в README: `![CI](https://github.com/IzabellaGerman/ShoppingPlanner.Api/actions/workflows/ci.yml/badge.svg)`

День 7 — PR и полировка:
- [ ] PR `feature/docker-ci` → main
- [ ] Добавить в `INTERVIEW_NOTES.md`: зачем Docker (воспроизводимость среды), зачем CI (не ломаем main)

**Definition of Done:**
- `docker compose up` поднимает API + PostgreSQL, Swagger открывается на `localhost:8080/swagger`
- GitHub Actions зелёный на каждом PR
- README содержит CI badge и инструкции по Docker

---

### Неделя 7 — Deploy + финальная полировка

**Цель:** живая ссылка в README. Рекрутер может открыть Swagger и потыкать API.

**Учёба (1 ч):**
- [ ] Документация выбранного хостинга (Railway / Render / Fly.io) — Getting Started

**Код (5–6 ч):**

День 1–2 — Выбор хостинга и деплой:
- [ ] Railway (рекомендуется — простой, есть бесплатный tier, поддерживает Docker)
- [ ] Подключить GitHub репо → Railway автоматически деплоит из `main`
- [ ] Настроить env-переменные в Railway UI: Connection String, JWT Key
- [ ] PostgreSQL как Railway сервис (или внешний — Neon.tech бесплатный PostgreSQL)
- [ ] Применить миграции на prod БД

День 3 — Production настройки:
- [ ] `appsettings.Production.json` — убрать Development-специфичные настройки
- [ ] Health check эндпоинт: `GET /health` → `200 OK` (для мониторинга хостинга)
  ```csharp
  app.MapHealthChecks("/health");
  builder.Services.AddHealthChecks();
  ```
- [ ] HTTPS redirect: `app.UseHttpsRedirection()`
- [ ] Swagger только в Development: `if (app.Environment.IsDevelopment()) { app.UseSwagger(); ... }`
  - **Внимание:** для portfolio удобнее оставить Swagger в production тоже — чтобы рекрутер мог потыкать

День 4–5 — Финальная полировка:
- [ ] README: добавить живую ссылку на Swagger
- [ ] README: добавить скриншот Swagger UI
- [ ] Пройтись по всем `INTERVIEW_NOTES.md` — дополнить пропущенные темы
- [ ] Проверить: `dotnet build` без warnings, `dotnet test` все зелёные
- [ ] Убедиться, что все PR смержены, ветки удалены

День 6–7 — Ретроспектива месяца 2:
- [ ] Записать: что было труднее всего (JWT? Docker?), что теперь понимаешь лучше
- [ ] Составить список тем для повторения перед собеседованиями (из `INTERVIEW_NOTES.md`)
- [ ] PR `feature/deploy-production` → main

**Definition of Done месяца 2:**
- Живой URL в README, Swagger открывается
- Register → Login → авторизованные запросы работают в production
- GitHub Actions зелёный
- `docker compose up` локально работает
- Все тесты зелёные

---

### Неделя 8 — Буферная неделя / бонус

Если недели 5–7 прошли по плану — выбери одно из:

**Опция A — Refresh Tokens:**
- [ ] Добавить `RefreshToken` сущность (токен, userId, expiresAt, isRevoked)
- [ ] Эндпоинт `POST /api/auth/refresh` — принимает refresh token, возвращает новый JWT
- [ ] Эндпоинт `POST /api/auth/logout` — инвалидирует refresh token
- [ ] **На собесе:** зачем refresh tokens (короткий JWT + долгий refresh = безопасность без постоянного логина)

**Опция B — Pagination + Filtering:**
- [ ] `GET /api/shopping-lists?page=1&pageSize=10` — пагинация
- [ ] `GET /api/products?category=Dairy&search=milk` — фильтрация
- [ ] Возвращать `PaginatedResponse<T>` с `totalCount`, `page`, `pageSize`
- [ ] **На собесе:** cursor-based vs offset pagination — когда что использовать

**Опция C — Подготовка к собеседованиям:**
- [ ] Пройтись по всем темам в `INTERVIEW_NOTES.md` вслух
- [ ] SQL: correlated subqueries (было выявлено как слабое место)
- [ ] Порепетировать "расскажи о проекте" в 2–3 минуты

---

## Правила работы над проектом

1. **Каждый код в feature-ветке, в main через PR.** Даже если PR от тебя к тебе. Это создаёт видимость работающего процесса в репозитории и тренирует git-flow.
2. **Коммиты по Conventional Commits:** `feat:`, `fix:`, `refactor:`, `test:`, `docs:`, `chore:`. На собесе плюс.
3. **Дневник в `JOURNAL.md` в корне репо** (или отдельный файл). После каждой сессии — 2–3 строчки: что сделала, на чём застряла, что узнала. Через 3 месяца это будет твой набор баек для собеседований.
4. **Если застряла >30 минут — пиши в чат.** Не залипай. Лучше быстро разблокировать и идти дальше.
5. **Не рефакторь раньше времени.** Сначала работает — потом красиво.
6. **Помни про "почему", а не только "как".** На каждом значимом решении — формулируй ответ в одно предложение. Это и есть подготовка к собеседованию параллельно с кодом.

---

*Последнее обновление: 8 июля 2026*
