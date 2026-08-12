---
name: code-review
description: Conducts code review of open Pull Requests across all repos. Use when user asks for code review, "сделай код ревью", "code review", or similar.
---

# Код-ревью

Запуск из корня проекта. Требует GitHub CLI (`gh`).

## 1. Получить список открытых PR (обязательно)

```bash
./infrastructure/scripts/git-list-open-prs.sh
```

Использовать **только** этот скрипт — он даёт полный список по всем репозиториям.

## 2. Провести ревью каждого PR

Для каждого PR из вывода:

- `gh pr view <номер>` и `gh pr diff <номер>` (в каталоге репозитория)
- Проверить по правилам: для backend (services/) — `architecture.mdc`, `best-practices-backend.mdc`, `csharp.mdc`; для frontend — `architecture-frontend.mdc`, `best-practices-frontend.mdc`, `vue.mdc`
- Оценить: читаемость, структура, производительность, безопасность, обработка ошибок, тесты
- Сформировать итог: замечания, предложения, что сделано хорошо

## 3. Выдать сводку

Для каждого MR: репозиторий, номер/URL PR, вывод ревью, рекомендации (approve / запросить правки).

Если открытых PR нет — сообщить об этом.
