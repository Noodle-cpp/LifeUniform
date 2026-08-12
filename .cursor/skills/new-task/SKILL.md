---
name: new-task
description: Starts a new task: pull repos, choose/create task in Singularity, decompose, create branch. Use when user asks "новая задача", "start new task", or similar.
---

# Новая задача

Запуск из корня проекта. Скрипты — в `infrastructure/scripts/`.

## Шаг 1: Подтянуть изменения (обязательно)

```bash
./infrastructure/scripts/git-pull-all.sh
```

Опционально: `BRANCH=main ./infrastructure/scripts/git-pull-all.sh` — для pull в main.

## Шаг 2: Задача в Singularity

Делегировать **субагент tasks** для списка и выбора задачи. Не вызывать MCP напрямую — только через tasks. Показать пользователю задачу и содержание, дождаться подтверждения.

## Шаг 3: Workflow

1. Ветка `feature/TASK-{номер}-{описание}` (номер из title задачи, например TASK-007).
2. Перевод в «В работу» — делегировать **tasks**.
3. Реализация, тесты, коммит, MR, ревью, мерж — делегировать **backend-workflow-orchestrator** или **frontend-workflow-orchestrator** в зависимости от типа задачи.
4. По завершении — перевод в «Готово» делегировать **tasks**.

Номер/название не указаны — уточнить.
