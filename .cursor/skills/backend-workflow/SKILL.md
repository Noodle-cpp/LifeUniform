---
name: backend-workflow
description: Full backend task lifecycle — from git-pull and Singularity task to PR creation. Combines git operations with the backend orchestrator pipeline. Use when starting a new backend task, "новая бекенд задача", "new backend task", or any full-cycle backend work.
---

# Backend Workflow (полный цикл)

Полный процесс бэкенд-задачи от постановки до PR. Объединяет git-операции (skills) и код-пайплайн (субагенты).

## Обзор пайплайна

```
Phase 0: Setup
  ├── git-pull-all
  ├── Singularity task (через **tasks**)
  ├── Определить целевой сервис
  ├── Branch (feature/TASK-XXX-*)
  └── Подтверждение пользователя

Phase 1: Development (backend-orchestrator)
  ├── Planner → план по слоям (Domain → Application → Infrastructure → API → Tests)
  ├── Implementator → код
  ├── Tester → build + dotnet test
  ├── Reviewer → code review (Clean Architecture, CQRS, best practices)
  └── (цикл до 3 итераций)

Phase 2: Finalize
  ├── Documentator → services/{service}/docs/TASK-XXX-*.md
  ├── Commit (git-commit)
  └── Push + Create PR (create-prs)

Phase 3: Post-PR (по запросу)
  ├── Code review открытых PR
  └── Fix → recommit → repush
```

## Phase 0: Setup

### 0.1. Подтянуть изменения

```bash
./infrastructure/scripts/git-pull-all.sh
```

### 0.2. Задача в Singularity

Делегировать **tasks**: выбрать или создать задачу, перевести в «В работу». Описание/цель/сервис/шаги/критерии — из Singularity. Не вызывать MCP напрямую — только через tasks.

Если номер/название не указаны — уточнить у пользователя.

### 0.3. Определить целевой сервис

Определить, какой из сервисов затронут: auth-service, catalog-service, content-service, media-service, payment-service, subscription-service, admin-service, notification-service. Если не очевидно — спросить пользователя.

### 0.4. Создать ветку

```bash
cd services/{service-name}
git checkout -b feature/TASK-{номер}-{описание}
```

Именование: kebab-case, по правилам `git`.

### 0.5. Подтверждение

Показать пользователю: задачу из Singularity (id, title) + имя ветки + целевой сервис. Дождаться подтверждения перед Phase 1.

## Phase 1: Development

Делегировать **`backend-orchestrator`** (субагент). Передать:
- Описание задачи
- Целевой сервис и путь
- Контекст (какие сущности, хендлеры, эндпоинты затронуты, если известно)

Оркестратор сам прогонит цикл: Planner → Implementator → Tester → Reviewer (до 3 итераций).

Дождаться финального отчёта от оркестратора.

**Если оркестратор завершился с нерешёнными проблемами** — показать пользователю и спросить: продолжить вручную или исправить?

## Phase 2: Finalize

### 2.1. Документация

Если оркестратор не вызвал documentator (или нужна дополнительная документация) — вызвать **`backend-documentator`** с описанием задачи, именем сервиса и списком изменённых файлов.

### 2.2. Коммит

Всегда делегировать **git-commit** (субагент). Передать путь `services/{service-name}`, сообщение `feat(TASK-XXX): краткое описание`, список изменённых файлов. Проверить после: `./infrastructure/scripts/git-check-committed.sh`

### 2.3. Push + PR

```bash
./infrastructure/scripts/git-create-prs.sh
```

Или вручную:

```bash
cd services/{service-name}
git push -u origin HEAD
gh pr create --title "feat(TASK-XXX): описание" --body "..."
```

### 2.4. Задача в Singularity

Делегировать **tasks** для перевода задачи в «Готово».

### 2.5. Отчёт

Показать пользователю итоговый статус:

```
## ✅ Backend Workflow Complete

**Задача:** TASK-XXX — описание
**Сервис:** {service-name}
**Ветка:** feature/TASK-XXX-описание
**PR:** [ссылка]

### Что сделано
- [ключевые изменения]

### Файлы
- [список]

### Тесты
- Build: ✅/❌ | Tests: ✅/❌

### Документация
- services/{service-name}/docs/TASK-XXX-*.md
```

## Phase 3: Post-PR (по запросу пользователя)

При команде «код ревью» — делегировать **code-reviewer**:

```bash
./infrastructure/scripts/git-list-open-prs.sh
```

При замечаниях:
1. Сформировать fix-план → **`backend-implementator`**
2. **`backend-tester`** — проверить исправления (с инструкцией «report only»)
3. Делегировать **git-commit** для коммита, затем push

## Быстрые команды (shortcuts)

Не обязательно проходить все фазы — можно вызвать отдельные части:

| Команда | Что делает |
|---------|------------|
| «спланируй бекенд» | Только Phase 1 → Planner |
| «реализуй бекенд план» | Только Implementator (по готовому плану) |
| «протестируй бекенд» | Только Tester |
| «ревью бекенда» | Только Reviewer |
| «сделай бекенд задачу» | Phase 1 целиком (Orchestrator) |
| «задокументируй бекенд» | Только Documentator |
| «зафиксируй все» | Phase 2.2 (Commit) |
| «создай МРы» | Phase 2.3 (Push + PR) |

## Правила

- **Не пропускать Phase 0.** Без актуальной ветки, выбранной задачи в Singularity и определённого сервиса — не начинать разработку.
- **Подтверждение пользователя** обязательно после Phase 0 и перед Phase 2.3 (создание PR).
- **Задачу в Singularity обновлять** — делегировать **tasks** (описание, прогресс, выполненные шаги).
- **При изменении Docker** — запустить `./infrastructure/scripts/docker-up.sh`.
- **При изменении схемы БД** — убедиться что EF Core миграция создана и применима.
- **Перед паузой** — закоммитить все изменения.
- **Один workflow = одна задача.** Для нескольких задач — запускать последовательно.
- **Сервис-скоупд git.** Каждый сервис — отдельный git-репо. Ветки и коммиты внутри `services/{service-name}/`.
