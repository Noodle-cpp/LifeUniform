# Анализ: правила, скиллы и агенты

*Дата: 3 марта 2025*

## 1. Принцип делегирования

### ✅ Соблюдено

| Уровень | Действие | Делегация |
|---------|----------|-----------|
| **workflow-orchestrator** | Задачи | **tasks** |
| | Коммит | **git-commit** |
| | Код-ревью | **code-reviewer** |
| | Бекенд/фронт полный цикл | **backend/frontend-workflow-orchestrator** |
| **backend-workflow-orchestrator** | Задача (выбор, «В работе», «Готово») | **tasks** |
| | Разработка | **backend-orchestrator** |
| | Документация | **backend-documentator** |
| | Коммит | **git-commit** |
| **frontend-workflow-orchestrator** | Аналогично | **tasks**, **frontend-orchestrator**, **frontend-documentator**, **git-commit** |
| **backend-orchestrator** | План, реализация, тесты, ревью, документ | planner, implementator, tester, reviewer, documentator |
| **frontend-orchestrator** | Аналогично | planner, implementator, tester, reviewer, documentator |
| **new-task** | Задача | **tasks** |
| | Реализация | **backend/frontend-workflow-orchestrator** |
| **workflow skill** | Шаги 1, 4, 5, 7, 9 | tasks, git-commit |
| **backend/frontend-workflow skills** | Задачи, коммит, код-ревью, Phase 3 fix | tasks, git-commit, code-reviewer, implementator, tester |
| **singularity.mdc** | Операции с задачами | **tasks** |
| **backend/frontend-tester** | «report only» | не вызывает implementator |

---

## 2. Зоны ответственности агентов

| Агент | Своя работа | Чужая работа делегируется |
|-------|-------------|---------------------------|
| **workflow-orchestrator** | Маршрутизация запросов, координация последовательности | ✅ по таблице |
| **backend/frontend-workflow-orchestrator** | Координация фаз, запуск скриптов (git-pull, ветка, push, gh pr) | ✅ tasks, orchestrator, documentator, git-commit |
| **backend/frontend-orchestrator** | Цикл план→реализация→тест→ревью | ✅ planner, implementator, tester, reviewer, documentator |
| **tasks** | Вся работа с Singularity (список, статусы, создание, обновление) | — (его зона) |
| **git-commit** | Создание коммитов | — |
| **code-reviewer** | Ревью PR | — |
| **backend/frontend-tester** | Запуск проверок и отчёт | при «report only» не вызывает implementator ✅ |

Скрипты (git-pull-all, git-check-committed, git-create-prs, docker-up) выполняют оркестраторы — это их координационная работа, отдельного агента для скриптов нет.

---

## 3. Оркестрация

### Цепочки делегирования

```
Пользователь → workflow-orchestrator
    ├── «задачи» → tasks
    ├── «новая задача» → new-task (скилл) → tasks → backend/frontend-workflow-orchestrator
    ├── «бекенд задача» → backend-workflow-orchestrator
    │       → tasks → backend-orchestrator → git-commit → tasks
    ├── «фронт задача» → frontend-workflow-orchestrator
    │       → tasks → frontend-orchestrator → git-commit → tasks
    ├── «зафиксируй» → git-commit
    ├── «код ревью» → code-reviewer
    └── …
```

### Согласованность с 00-rules-guide

Таблицы триггеров в `00-rules-guide` и `workflow-orchestrator` совпадают (tasks, git-commit, code-reviewer и т.д.).

---

## 4. Мелкие замечания (низкий приоритет)

### 4.1. Скилл workflow, шаг 6

```
6. **Тесты** — unit, integration, ручное
```

Не указано делегирование. Для полной консистентности можно добавить: при бекенд/фронт контексте делегировать **backend-tester** или **frontend-tester**.

### 4.2. Скилл workflow, шаг 9

```
9. **Готово** — код-ревью, исправления, мерж; делегировать **tasks** для перевода в «Готово»
```

«Код-ревью» не делегируется явно. Можно дополнить: делегировать **code-reviewer** перед переводом в «Готово».

### 4.3. Скилл workflow, «Перед паузой»

```
- Закоммитить изменения
```

Для консистентности можно заменить на: делегировать **git-commit**.

---

## 5. Итог

Делегирование выстроено корректно:
- **tasks** — единая точка входа для Singularity
- **git-commit** — единая точка для коммитов
- **code-reviewer** — единая точка для код-ревью
- Оркестраторы координируют и не занимаются разработкой напрямую
- Tester соблюдает «report only» и не вызывает implementator

Мелкие улучшения в скилле workflow (шаги 6, 9, «Перед паузой») необязательны.
