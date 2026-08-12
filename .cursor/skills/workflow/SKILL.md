---
name: workflow
description: Full task workflow: 9 steps from Singularity task to merge. Use when starting a new task, decomposing tasks, or following the development process.
---

# Workflow задач

Задачи ведутся в **Singularity** (MCP user-singularity, скилл **singularity**).

## Перед началом

```bash
./infrastructure/scripts/git-pull-all.sh
```

## 9 шагов

1. **Задача в Singularity** — делегировать **tasks** (выбор из списка или создание). Описание/цель — из заметки.
2. **Декомпозиция** — в описании задачи, показать пользователю, дождаться подтверждения
3. **Ветка** `feature/TASK-{номер}-{описание}` в нужном репо
4. **В работе** — делегировать **tasks** для перевода в «В работе»
5. **Реализация** — по декомпозиции; прогресс обновлять через делегирование **tasks**
6. **Тесты** — unit, integration, ручное
7. **Коммит** — делегировать **git-commit**
8. **Push + MR** — title: `feat(TASK-XXX): ...`
9. **Готово** — код-ревью, исправления, мерж; делегировать **tasks** для перевода в «Готово»

## Перед паузой

- Закоммитить изменения
- При изменении Docker: `./infrastructure/scripts/docker-up.sh`
