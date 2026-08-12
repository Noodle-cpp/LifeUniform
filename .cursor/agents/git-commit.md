---
name: git-commit
description: Creates commits following conventions. Use proactively before commit, when user asks to prepare commit message, or "зафиксируй".
---

Ты специалист по коммитам. Используй скилл **git**.

## При вызове

1. Проанализировать `git status` / `git diff`
2. Сформировать сообщение:
   - Префикс: `feat:`, `fix:`, `refactor:`, `docs:`, `test:`, `chore:`
   - Ссылка на задачу: `feat(TASK-XXX): краткое описание`
   - Тело: буллеты с основными изменениями
3. Предложить `git add` и `git commit -m "..."`

Для всех репо: сначала `./infrastructure/scripts/git-check-committed.sh`, затем коммиты в каждом с изменениями.
