---
name: docker
description: Docker: Dockerfile, docker-compose. Use when creating Dockerfile, configuring containers, or infrastructure.
---

# Docker

## Dockerfile

- Многоэтапная сборка (build → runtime)
- .dockerignore, alpine/base
- Кэшировать зависимости до копирования кода

## Docker Compose

- Health checks, restart policies
- Переменные окружения для конфигурации, не секреты в yml

## Команды

```bash
# Test — запуск/перезапуск контейнеров (пересборка)
./infrastructure/scripts/docker-up.sh

# 404 на API после изменений:
docker exec <nginx-container> nginx -s reload
```

**Триггеры:** «перезапусти контейнеры», «пересобери», «docker-up»

RC/Prod — см. скилл **deploy**.
