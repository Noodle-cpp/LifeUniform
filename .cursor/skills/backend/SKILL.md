---
name: backend
description: Backend development: C# .NET 8, Clean Architecture, CQRS/MediatR, EF Core, API design, validation. Use when writing C# code, creating API controllers, working with services/*, or backend development.
---

# Backend (C# .NET 8)

## JSON (обязательно)

В `Program.cs`:
```csharp
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);
```

## Архитектура микросервиса

Слои: Domain → Application → Infrastructure → API. CQRS/MediatR в Application.

```
ServiceName/
├── Domain/          Entities, Interfaces, ValueObjects
├── Application/     Commands, Queries, Handlers, DTOs, Validators
├── Infrastructure/  Data, Services, Configurations
└── API/             Controllers (тонкие), Middleware
```

## CQRS + MediatR

- Command/Query: `IRequest<Result>`, Handler: `IRequestHandler<TRequest, TResponse>`
- ValidationBehavior + FluentValidation
- Контроллеры: только `await _mediator.Send(command)`, без бизнес-логики

## API, EF Core, безопасность

- DTOs для запросов/ответов, не доменные сущности
- `.AsNoTracking()` для read-only, `.Include()` против N+1
- async/await, nullable enable, JWT + `[Authorize]`
