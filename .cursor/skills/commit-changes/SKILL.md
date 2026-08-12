---
name: commit-changes
description: Commits all uncommitted changes in monorepo. Use when user asks to commit changes, "зафиксируй все изменения", "commit all changes", or similar.
---

# Зафиксировать все изменения

Запуск из корня проекта.

## 1. Проверить репозитории

```bash
./infrastructure/scripts/git-check-committed.sh
```

- **exit 0** — везде чисто, сообщить пользователю
- **exit 1** — есть незакоммиченные изменения

## 2. При наличии изменений

Для каждого репозитория с изменениями:

1. Перейти в каталог репозитория
2. `git add` (или `git add .` по контексту)
3. Коммит по `git.mdc`:
   - префикс: `feat:`, `fix:`, `refactor:` и т.д.
   - ссылка на задачу: `feat(TASK-XXX): ...`
   - осмысленное сообщение по содержимому изменений

## 3. Проверить результат

```bash
./infrastructure/scripts/git-check-committed.sh
```

Выход 0 — готово. Коммитить только там, где реально есть изменения.
