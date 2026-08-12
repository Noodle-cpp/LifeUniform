---
name: git
description: Git workflow: branches, commits, PR. Use when creating commits, working with branches, or creating pull requests.
---

# Git

## Ветки

- `feature/TASK-{номер}-{описание}` (kebab-case)
- hotfix: `hotfix/TASK-{номер}-{описание}` → merge в main

## Коммиты

Префикс + описание:
- `feat:`, `fix:`, `refactor:`, `docs:`, `test:`, `chore:`
- Ссылка на задачу: `feat(TASK-001): add user auth`

```
feat(TASK-001): add user registration

- Implemented endpoint
- Added JWT generation
```

## PR

- PR для всех изменений
- Минимум один approve
