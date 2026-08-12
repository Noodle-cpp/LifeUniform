---
name: create-prs
description: Creates Pull Requests in all repos where current branch is ahead of target. Use when user asks to create PRs, "создай МРы", "create PRs", or similar.
---

# Создать PR

Запуск из корня проекта. Требует GitHub CLI (`gh auth login`).

## Команда

```bash
./infrastructure/scripts/git-create-prs.sh
```

## Что делает скрипт

- Для каждого репозитория (основной, services/*, frontend, anderoof), где ветка опережает целевую (develop или main):
  - Пушит ветку
  - Создаёт PR через `gh pr create`
- Репозитории без опережающих коммитов и уже открытые PR пропускает

## Опции

```bash
TARGET_BRANCH=main ./infrastructure/scripts/git-create-prs.sh
```

Для PR в main вместо develop.
