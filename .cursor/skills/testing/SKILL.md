---
name: testing
description: Testing: unit tests, integration tests, Moq, FluentAssertions. Use when writing tests for .NET.
---

# Тесты

## Unit

- AAA: Arrange, Act, Assert
- Moq для моков, FluentAssertions для assert
- Именование: `{Method}_{Scenario}_{ExpectedResult}`

## Integration

- WebApplicationFactory, in-memory или Testcontainers

## Запуск

```bash
dotnet test
```
