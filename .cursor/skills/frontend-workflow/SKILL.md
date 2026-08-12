---
name: frontend-workflow
description: Full frontend task lifecycle — from git-pull and task file to PR creation. Combines git operations with the frontend orchestrator pipeline. Use when starting a new frontend task, "новая фронтенд задача", "new frontend task", or any full-cycle frontend work.
---

# Frontend Workflow (полный цикл)

Полный процесс фронтенд-задачи от постановки до PR. Объединяет git-операции (skills) и код-пайплайн (субагенты).

## Обзор пайплайна

```
Phase 0: Setup
  ├── git-pull-all
  ├── Singularity task (через **tasks**)
  ├── Branch (feature/TASK-XXX-*)
  └── Подтверждение пользователя

Phase 1: Development (frontend-orchestrator)
  ├── Planner → план
  ├── Implementator → код
  ├── Tester → lint + types + tests
  ├── Reviewer → code review
  └── (цикл до 3 итераций)

Phase 2: Finalize
  ├── Documentator → frontend/docs/TASK-XXX-*.md
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

Делегировать **tasks**: выбрать или создать задачу, перевести в «В работу». Описание/цель/шаги/критерии — из Singularity. Не вызывать MCP напрямую — только через tasks.

Если номер/название не указаны — уточнить у пользователя.

### 0.3. Создать ветку

```bash
cd frontend
git checkout -b feature/TASK-{номер}-{описание}
```

Именование: kebab-case, по правилам `git`.

### 0.4. Подтверждение

Показать пользователю: задачу из Singularity (id, title) + имя ветки. Дождаться подтверждения перед Phase 1.

## Phase 1: Development

Делегировать **`frontend-orchestrator`** (субагент). Передать:
- Описание задачи из файла
- Контекст (какие компоненты/страницы затронуты, если известно)

Оркестратор сам прогонит цикл: Planner → Implementator → Tester → Reviewer (до 3 итераций).

Дождаться финального отчёта от оркестратора.

**Если оркестратор завершился с нерешёнными проблемами** — показать пользователю и спросить: продолжить вручную или исправить?

## Phase 2: Finalize

### 2.1. Документация

Если оркестратор не вызвал documentator (или нужна дополнительная документация) — вызвать **`frontend-documentator`** с описанием задачи и списком изменённых файлов.

### 2.2. Коммит

Всегда делегировать **git-commit** (субагент). Передать путь `frontend`, сообщение `feat(TASK-XXX): краткое описание`, список изменённых файлов. Проверить после: `./infrastructure/scripts/git-check-committed.sh`

### 2.3. Push + PR

```bash
./infrastructure/scripts/git-create-prs.sh
```

Или вручную:

```bash
cd frontend
git push -u origin HEAD
gh pr create --title "feat(TASK-XXX): описание" --body "..."
```

### 2.4. Отчёт

Показать пользователю итоговый статус:

```
## ✅ Frontend Workflow Complete

**Задача:** TASK-XXX — описание
**Ветка:** feature/TASK-XXX-описание
**PR:** [ссылка]

### Что сделано
- [ключевые изменения]

### Файлы
- [список]

### Тесты
- Lint: ✅/❌ | Types: ✅/❌ | Tests: ✅/❌

### Документация
- frontend/docs/TASK-XXX-*.md
```

## Phase 3: Post-PR (по запросу пользователя)

При команде «код ревью» — делегировать **code-reviewer**:

```bash
./infrastructure/scripts/git-list-open-prs.sh
```

При замечаниях:
1. Сформировать fix-план → **`frontend-implementator`**
2. **`frontend-tester`** — проверить исправления (с инструкцией «report only»)
3. Делегировать **git-commit** для коммита, затем push

## Быстрые команды (shortcuts)

Не обязательно проходить все фазы — можно вызвать отдельные части:

| Команда | Что делает |
|---------|------------|
| «спланируй фронтенд» | Только Phase 1 → Planner |
| «реализуй план» | Только Implementator (по готовому плану) |
| «протестируй фронтенд» | Только Tester |
| «ревью фронтенда» | Только Reviewer |
| «сделай фронтенд задачу» | Phase 1 целиком (Orchestrator) |
| «задокументируй» | Только Documentator |
| «зафиксируй все» | Phase 2.2 (Commit) |
| «создай МРы» | Phase 2.3 (Push + PR) |

## Правила

- **Не пропускать Phase 0.** Без актуальной ветки и выбранной задачи в Singularity — не начинать разработку.
- **Подтверждение пользователя** обязательно после Phase 0 и перед Phase 2.3 (создание PR).
- **Задачу в Singularity обновлять** — делегировать **tasks** (описание, прогресс, выполненные шаги).
- **При изменении Docker** — запустить `./infrastructure/scripts/docker-up.sh`.
- **Перед паузой** — закоммитить все изменения.
- **Один workflow = одна задача.** Для нескольких задач — запускать последовательно.
