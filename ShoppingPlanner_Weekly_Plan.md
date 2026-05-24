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

**Где остановилась:** скелет проекта стоит, демо-эндпоинт `/weatherforecast` работает, Swagger UI открывается. Следующий шаг — реальный код.

---

### Неделя 2 — C# refresh + первый CRUD (в памяти)

**Цель:** один работающий ресурс `Product` с CRUD-эндпоинтами в памяти, чтобы вспомнить ASP.NET Core и закрыть страх перед чистым проектом.

**Учёба (5–6 ч за неделю):**
- [ ] Nick Chapsas — "ASP.NET Core for Beginners" на YouTube (~5 часов). Смотрим **до раздела про EF Core**, его делаем на неделе 3.
- [ ] Параллельно: дочитать гайд Microsoft "Tutorial: Create a controller-based web API with ASP.NET Core" (~1 час).

**Код (5–7 ч за неделю):**

День 1–2 — модель и DTO:
- [x] Удалить `WeatherForecast.cs` и `Controllers/WeatherForecastController.cs`.
- [x] Создать папку `Models/` и в ней `Product.cs`:
  - поля: `Id (int)`, `Name (string)`, `Category (string)`, `DefaultUnit (string)` (kg, l, pcs), `CreatedAt (DateTime)`.
- [x] Создать папку `Dtos/` и в ней `ProductDto.cs`, `CreateProductDto.cs`, `UpdateProductDto.cs`.
- [x] **Почему DTO отдельно от Model:** на собеседовании всегда спрашивают. Domain model — внутреннее представление, DTO — что выставлено наружу. Это паттерн для всех будущих сущностей.

День 3–4 — in-memory storage и сервис:
- [x] Создать папку `Services/` и в ней `IProductService.cs` + `ProductService.cs`.
- [x] Внутри сервиса — простой `List<Product>` + методы `GetAll`, `GetById`, `Create`, `Update`, `Delete`.
- [x] В `Program.cs` зарегистрировать сервис: `builder.Services.AddSingleton<IProductService, ProductService>();`
- [x] **Зачем интерфейс:** DI, моки в тестах, замена реализации на EF Core на неделе 3 без правки контроллера.

День 5 — контроллер:
- [ ] Создать `Controllers/ProductsController.cs`.
- [ ] Пять методов: `GET /products`, `GET /products/{id}`, `POST /products`, `PUT /products/{id}`, `DELETE /products/{id}`.
- [ ] Правильные HTTP-коды: 200, 201 (с `CreatedAtAction`), 204, 404, 400.
- [ ] Валидация через `[Required]`, `[StringLength]` атрибуты на DTO.

День 6 — тесты:
- [ ] В `tests/` написать 5–7 unit-тестов для `ProductService` через xUnit.
- [ ] Структура AAA (Arrange-Act-Assert). Имена тестов: `MethodName_Scenario_ExpectedResult`.

День 7 — оформление:
- [ ] Прогнать через Swagger UI вручную каждый эндпоинт.
- [ ] Создать ветку `feature/products-crud-in-memory`, открыть PR в main, смерджить.
- [ ] Обновить README — поставить галочку на пункт "Products CRUD".

**Definition of Done для недели 2:**
- `dotnet build` — без warnings.
- `dotnet test` — все зелёные.
- В Swagger 5 эндпоинтов для Products, ручная проверка прошла.
- PR закрыт в main, на GitHub видно зелёный коммит.

---

### Неделя 3 — EF Core + PostgreSQL

**Цель:** заменить in-memory storage на реальную БД через EF Core, добавить связанные сущности.

**Учёба (4–5 ч):**
- [ ] Nick Chapsas — "Entity Framework Core for Beginners" (~3 часа).
- [ ] Microsoft Docs — Migrations overview (~30 мин).

**Код (6–8 ч):**

День 1 — PostgreSQL локально:
- [ ] Установить Docker Desktop (если ещё нет).
- [ ] Поднять Postgres контейнером:
  ```bash
  docker run -d --name shoppingplanner-db \
    -e POSTGRES_PASSWORD=dev \
    -e POSTGRES_DB=shoppingplanner \
    -p 5432:5432 \
    postgres:16
  ```
- [ ] Поставить клиент: DBeaver или pgAdmin. Подключиться, проверить, что БД пустая работает.

День 2 — EF Core пакеты и DbContext:
- [ ] Добавить пакеты:
  ```bash
  dotnet add src/ShoppingPlanner.Api package Microsoft.EntityFrameworkCore
  dotnet add src/ShoppingPlanner.Api package Npgsql.EntityFrameworkCore.PostgreSQL
  dotnet add src/ShoppingPlanner.Api package Microsoft.EntityFrameworkCore.Design
  ```
- [ ] Создать папку `Data/` и в ней `AppDbContext.cs` с `DbSet<Product>`.
- [ ] Connection string в `appsettings.Development.json` (НЕ коммитить пароли — пока dev окей).
- [ ] Зарегистрировать `AppDbContext` в `Program.cs`.

День 3 — первая миграция:
- [ ] Установить EF tools глобально: `dotnet tool install --global dotnet-ef`.
- [ ] Создать миграцию: `dotnet ef migrations add InitialCreate --project src/ShoppingPlanner.Api`.
- [ ] Применить: `dotnet ef database update --project src/ShoppingPlanner.Api`.
- [ ] Проверить через DBeaver — таблица `Products` должна появиться.

День 4–5 — переключить сервис на EF:
- [ ] Создать `ProductRepository.cs` (или сразу инжектить `AppDbContext` в сервис — на этом этапе оба варианта ок).
- [ ] Переписать `ProductService` под async (`Task<...>`, `await`).
- [ ] Контроллер тоже async — все методы `async Task<IActionResult>`.
- [ ] **Внимание:** не используй `.Result` или `.Wait()` нигде. Только `await`. Это типовой вопрос на собесе.

День 6 — расширить модель:
- [ ] Добавить сущность `Category` (Id, Name).
- [ ] У `Product` сделать связь many-to-one: `CategoryId` + navigation property `Category`.
- [ ] Создать новую миграцию `AddCategories`. Применить.
- [ ] Эндпоинт `GET /categories` — список всех категорий.
- [ ] В `GET /products` подгружать категорию через `.Include(p => p.Category)`.

День 7 — оформление:
- [ ] Seed-данные: 5 категорий, 15 продуктов через `HasData` в `OnModelCreating` или отдельный seeder.
- [ ] Тесты обновить: для EF Core либо in-memory provider, либо мок репозитория. Лучше второй вариант — на собесе спросят про in-memory подводные камни.
- [ ] PR `feature/ef-core-postgres` → main.

**Definition of Done:**
- PostgreSQL поднимается одной командой.
- Миграции работают (`dotnet ef database update`).
- Все эндпоинты Products работают с реальной БД.
- Есть Category и связь с Product.
- Тесты зелёные.

---

### Неделя 4 — Доменная логика + полировка месяца 1

**Цель:** добавить главную сущность проекта — `ShoppingList` — и связи. К концу недели проект должен выглядеть как "реальный планировщик покупок", а не CRUD-демо.

**Учёба (3 ч):**
- [ ] Nick Chapsas — видео про "Repository Pattern" и "Unit of Work" (~1 час). Решить — оставлять как есть или внедрять.
- [ ] Глава "REST API design best practices" из любого источника (Microsoft Docs / Microservices.io).

**Код (8–10 ч):**

День 1–2 — сущности списков покупок:
- [ ] Сущность `ShoppingList`: Id, Name, CreatedAt, (потом — UserId на неделе 5–6).
- [ ] Сущность `ShoppingListItem`: Id, ShoppingListId, ProductId, Quantity (decimal), Note (string?), IsCompleted (bool).
- [ ] Связи: ShoppingList → many ShoppingListItems; ShoppingListItem → one Product.
- [ ] Миграция `AddShoppingLists`. Применить.

День 3–4 — эндпоинты для списков:
- [ ] `GET /shopping-lists` — все списки (потом фильтр по юзеру).
- [ ] `GET /shopping-lists/{id}` — один список с items и продуктами (Include).
- [ ] `POST /shopping-lists` — создать список (с items в теле запроса).
- [ ] `PUT /shopping-lists/{id}` — переименовать список.
- [ ] `DELETE /shopping-lists/{id}` — удалить.
- [ ] `POST /shopping-lists/{id}/items` — добавить продукт в список.
- [ ] `PATCH /shopping-lists/{id}/items/{itemId}` — отметить как купленный / поменять количество.
- [ ] `DELETE /shopping-lists/{id}/items/{itemId}` — убрать продукт из списка.

День 5 — обработка ошибок:
- [ ] Глобальный exception handler через middleware или `IExceptionHandler` (.NET 8 фича — упомянуть на собесе).
- [ ] Возврат `ProblemDetails` (RFC 7807) при ошибках. Это стандарт, спрашивают.
- [ ] Логирование через встроенный `ILogger<T>`.

День 6 — тесты:
- [ ] Unit-тесты для сервиса ShoppingLists.
- [ ] Хотя бы 2–3 integration-теста через `WebApplicationFactory<Program>` — это покажет на собесе, что ты понимаешь, как тестировать ASP.NET-приложение целиком.
- [ ] Цель — линейное покрытие минимум 40–50% к концу месяца 1.

День 7 — оформление и ретроспектива месяца 1:
- [ ] Обновить README: убрать заглушки "coming on week X" с того, что уже сделано.
- [ ] Скриншоты Swagger UI в README.
- [ ] Записать в дневник: что было трудно, что узнала нового, какие концепции до конца не понятны.
- [ ] PR `feature/shopping-lists` → main.

**Definition of Done месяца 1:**
- Работающее API с тремя сущностями (Product, Category, ShoppingList) на PostgreSQL.
- Async везде.
- Swagger показывает осмысленные эндпоинты с DTO.
- Тесты есть, проходят.
- README выглядит как у production-проекта (не "Trying to apply MVVM pattern").
- На GitHub видно регулярные коммиты через PR-flow, а не "Initial commit".

---

## Что дальше — превью месяца 2

(Детальный недельный план составим в конце месяца 1, когда увидим скорость.)

**Месяц 2 целиком про:**
- Аутентификация (JWT + refresh tokens, ASP.NET Core Identity).
- Авторизация — пользователь видит только свои списки.
- Docker Compose для всего стека (API + Postgres одной командой).
- GitHub Actions: build → test → docker image на каждый PR.
- Deploy на бесплатный хостинг (Railway / Render / Fly.io) — чтобы в README была живая ссылка.

К концу месяца 2 проект готов как portfolio piece: можно дать ссылку рекрутеру, и он откроет работающий Swagger.

---

## Правила работы над проектом

1. **Каждый код в feature-ветке, в main через PR.** Даже если PR от тебя к тебе. Это создаёт видимость работающего процесса в репозитории и тренирует git-flow.
2. **Коммиты по Conventional Commits:** `feat:`, `fix:`, `refactor:`, `test:`, `docs:`, `chore:`. На собесе плюс.
3. **Дневник в `JOURNAL.md` в корне репо** (или отдельный файл). После каждой сессии — 2–3 строчки: что сделала, на чём застряла, что узнала. Через 3 месяца это будет твой набор баек для собеседований.
4. **Если застряла >30 минут — пиши в чат.** Не залипай. Лучше быстро разблокировать и идти дальше.
5. **Не рефакторь раньше времени.** Сначала работает — потом красиво. На неделе 2 нормально иметь `List<Product>` в памяти, на неделе 3 заменишь на EF.
6. **Помни про "почему", а не только "как".** На каждом значимом решении (зачем DTO, зачем async, зачем интерфейс сервиса) — формулируй ответ в одно предложение. Это и есть подготовка к собеседованию параллельно с кодом.

---

*Последнее обновление: 22 мая 2026*
