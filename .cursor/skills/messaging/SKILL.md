---
name: messaging
description: RabbitMQ: Publisher/Consumer, queues. Use when working with async processing, message queues.
---

# RabbitMQ

- **Publisher:** интерфейс в Domain, реализация в Infrastructure
- **Consumer:** HostedService (BackgroundService), `IServiceProvider.CreateScope()` для scoped сервисов
- durable queues, persistent messages, autoAck: false, retry для подключения
- BasicNack + requeue для временных ошибок
