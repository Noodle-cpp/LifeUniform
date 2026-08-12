<!-- BEGIN FILE: user-manual.md -->
# User Manual: LifeUniform Cursor Rules

This manual describes how to use the Cursor rules/agents for the LifeUniform project.

LifeUniform stack:

- .NET 10
- ASP.NET Core Razor Pages
- Bootstrap 5
- EF Core
- FluentValidation
- Clean layered architecture
- No CQRS/MediatR
- No Docker
- Publish via normal build artifacts

## Project model

LifeUniform is a single solution:

LifeUniform/
├─ src/
│  ├─ LifeUniform.Domain/
│  ├─ LifeUniform.Application/
│  ├─ LifeUniform.Infrastructure/
│  └─ LifeUniform.Web/
├─ tests/
├─ docs/
└─ LifeUniform.sln

There is no separate Vue frontend. UI is implemented with Razor Pages and Bootstrap.

## Main entry points

### Full backend/web task lifecycle

Use when you want the full flow:

plan → implement → test → review → document → commit → PR

Example prompts:

- Start backend task
- Start LifeUniform task
- New backend task
- Сделай backend задачу

Use:

- backend-workflow-orchestrator

This orchestrator is responsible for:

- git status check;
- branch creation;
- delegating the code pipeline to backend-orchestrator;
- documentation;
- commit;
- push;
- PR creation.

### UI-only task lifecycle

Use when the task is only about Razor Pages UI:

- page markup;
- Bootstrap layout;
- forms;
- validation UI;
- partials;
- ViewComponents;
- navigation;
- admin screens.

Example prompts:

- Start webui task
- UI task
- Update product catalog UI
- Сделай webui задачу

Use:

- webui-workflow-orchestrator

If the UI task requires new backend services, entities, EF Core migrations, payments, email, or identity logic, use backend-workflow-orchestrator instead.

## Code pipeline without git workflow

If you already have a branch and only want the development loop:

plan → implement → test → review → document

Use:

- backend-orchestrator

Example prompts:

- Run backend pipeline
- Implement this LifeUniform feature
- Выполни задачу по плану

## Point commands

These commands run a specific role without the full lifecycle.

### Planning

Example prompts:

- Plan backend
- Plan this feature
- Спланируй задачу

Use:

- backend-planner

For UI-only planning:

- webui-planner

### Implementation

Example prompts:

- Implement the plan
- Do the next task
- Реализуй план

Use:

- backend-implementator

For UI-only implementation:

- webui-implementator

### Testing

Example prompts:

- Test backend
- Run checks
- Протестируй проект

Use:

- backend-tester

For UI-only checks:

- webui-tester

The tester only reports results. It does not fix code.

### Review

Example prompts:

- Review backend
- Review changed files
- Сделай ревью

Use:

- backend-reviewer

For UI review:

- webui-reviewer

### Documentation

Example prompts:

- Document backend
- Document this task
- Задокументируй задачу

Use:

- backend-documentator

For UI documentation:

- webui-documentator

### Codebase navigation

Example prompts:

- Where is Product declared?
- How does checkout work?
- Show project structure

Use:

- explorer

### Debugging

Example prompts:

- Why does this not work?
- Debug this error
- Test failure

Use:

- debugger

### Pull request review

Example prompts:

- Review PR
- Code review
- Сделай код ревью

Use:

- code-reviewer

## Typical workflows

### Full backend task

User:

Start backend task — add product catalog

Flow:

backend-workflow-orchestrator
  ├── git setup
  ├── branch creation
  ├── backend-orchestrator
  │     ├── backend-planner
  │     ├── backend-implementator
  │     ├── backend-tester
  │     ├── backend-reviewer
  │     └── backend-documentator
  ├── commit
  ├── push
  └── PR

### UI-only task

User:

Start webui task — improve product cards

Flow:

webui-workflow-orchestrator
  ├── git setup
  ├── branch creation
  ├── webui-planner
  ├── webui-implementator
  ├── webui-tester
  ├── webui-reviewer
  ├── webui-documentator
  ├── commit
  ├── push
  └── PR

### Manual development loop

Example:

- User: Plan product catalog
- User: Implement the plan
- User: Test backend
- User: Review changed files
- User: Document this task

## Task IDs

Task IDs are optional.

If a task ID exists, use it:

TASK-123

Branch example:

feature/TASK-123-product-catalog

Commit example:

feat(TASK-123): add product catalog

Documentation example:

docs/TASK-123-product-catalog.md

If there is no task ID, use a short kebab-case description:

feature/product-catalog
feat: add product catalog
docs/2026-08-11-product-catalog.md

## Git conventions

Base branch:

main

or if the project uses:

develop

Feature branches:

feature/TASK-XXX-short-title
feature/short-title

Commits:

feat(TASK-XXX): short title
fix(TASK-XXX): short title
docs(TASK-XXX): short title

## Build and publish

Default checks:

dotnet build LifeUniform.sln
dotnet test LifeUniform.sln --logger "console;verbosity=normal"

Release publish:

dotnet publish src/LifeUniform.Web -c Release -o artifacts/publish/LifeUniform.Web

Docker is not used.

## What is not used in LifeUniform

Do not use these old concepts:

- services/{service-name}/
- separate frontend Vue project
- CQRS/MediatR
- Commands/Queries/Handlers
- Dockerfile
- docker-compose
- docker-up.sh
- git-pull-all.sh
- mandatory Singularity task agent
- separate backend/frontend monorepos

## Recommended rule files

Core backend/web pipeline:

- backend-planner.md
- backend-implementator.md
- backend-dev.md
- backend-tester.md
- backend-reviewer.md
- backend-documentator.md
- backend-orchestrator.md
- backend-workflow-orchestrator.md

UI-specific rules, if needed:

- webui-planner.md
- webui-implementator.md
- webui-dev.md
- webui-tester.md
- webui-reviewer.md
- webui-documentator.md
- webui-workflow-orchestrator.md

Support rules:

- explorer.md
- debugger.md
- code-reviewer.md

## Rules of thumb

- One task = one workflow.
- Confirm branch and task scope before development.
- Tests gate review.
- Reviewer does not fix code.
- Tester only reports.
- Orchestrator coordinates.
- Do not add Docker.
- Do not add CQRS/MediatR.
- Do not add a separate SPA frontend.
<!-- END FILE: user-manual.md -->


<!-- BEGIN FILE: analysis-rules-skills-agents.md -->
# Analysis: LifeUniform Rules, Skills, and Agents

Date: 2026-08-11

This document analyzes the Cursor rule/agent structure for LifeUniform after migrating away from the old microservice/Vue/CQRS setup.

Current target stack:

- .NET 10
- ASP.NET Core Razor Pages
- Bootstrap 5
- EF Core
- FluentValidation
- Clean layered architecture
- No CQRS/MediatR
- No Docker
- Publish via build artifacts

## 1. Project model

LifeUniform is a single monolithic solution, not a microservice repository.

Solution structure:

LifeUniform/
├─ src/
│  ├─ LifeUniform.Domain/
│  ├─ LifeUniform.Application/
│  ├─ LifeUniform.Infrastructure/
│  └─ LifeUniform.Web/
├─ tests/
├─ docs/
└─ LifeUniform.sln

Important consequences:

- There is no services/{service-name}/ folder.
- There is no separate frontend repository.
- There is no Docker-based local environment.
- There is no mandatory Singularity task agent.
- Backend and UI are developed inside the same solution.
- Razor Pages replaces the old API + separate frontend model.

## 2. Delegation principle

The rules preserve delegation: orchestrators coordinate, specialized agents execute.

### Full backend/web task lifecycle

| Level | Responsibility | Delegates to |
|---|---|---|
| backend-workflow-orchestrator | Full task lifecycle: git setup, branch, pipeline, documentation, commit, PR | backend-orchestrator, backend-documentator, git/PR commands |
| backend-orchestrator | Development loop | backend-planner, backend-implementator, backend-tester, backend-reviewer |
| backend-planner | Planning | None |
| backend-implementator | Implementation | None |
| backend-tester | Build/test/publish checks | None, report only |
| backend-reviewer | Code review | None, report only |
| backend-documentator | Documentation | None |

### Optional UI-only lifecycle

| Level | Responsibility | Delegates to |
|---|---|---|
| webui-workflow-orchestrator | UI-only task lifecycle | webui-planner, webui-implementator, webui-tester, webui-reviewer, webui-documentator |
| webui-planner | UI planning | None |
| webui-implementator | Razor Pages UI implementation | None |
| webui-tester | UI-related build/test checks | None, report only |
| webui-reviewer | UI review | None, report only |
| webui-documentator | UI documentation | None |

UI orchestrator should only be used when the task is purely UI-related. If backend services, entities, migrations, payments, email, files, or identity logic are required, the task belongs to backend-workflow-orchestrator.

## 3. Responsibility zones

| Agent | Own responsibility | Must not do |
|---|---|---|
| backend-workflow-orchestrator | Coordinate task lifecycle, git setup, branch, PR | Write implementation code |
| backend-orchestrator | Coordinate plan/implement/test/review loop | Write implementation code |
| backend-planner | Produce atomic implementation plan | Write code |
| backend-implementator | Implement tasks from plan | Change plan without need |
| backend-tester | Run checks and report | Fix code |
| backend-reviewer | Review changed files | Fix code |
| backend-documentator | Document completed work | Speculate or rewrite existing docs |
| webui-workflow-orchestrator | Coordinate UI-only lifecycle | Implement backend logic |
| webui-implementator | Implement Razor Pages UI | Add business logic or EF Core queries |
| explorer | Navigate codebase | Make changes |
| debugger | Find root cause and propose minimal fix | Rewrite architecture |
| code-reviewer | Review PRs or branch diffs | Fix code |

## 4. Orchestration chains

### Full backend/web task

User
 └── backend-workflow-orchestrator
       ├── git status/pull
       ├── branch creation
       ├── user confirmation
       ├── backend-orchestrator
       │     ├── backend-planner
       │     ├── backend-implementator
       │     ├── backend-tester
       │     ├── backend-reviewer
       │     └── backend-documentator
       ├── commit
       ├── push
       └── PR

### UI-only task

User
 └── webui-workflow-orchestrator
       ├── git status/pull
       ├── branch creation
       ├── user confirmation
       ├── webui-planner
       ├── webui-implementator
       ├── webui-tester
       ├── webui-reviewer
       ├── webui-documentator
       ├── commit
       ├── push
       └── PR

### Point commands

User: plan feature
 └── backend-planner or webui-planner

User: implement plan
 └── backend-implementator or webui-implementator

User: test backend
 └── backend-tester or webui-tester

User: review code
 └── backend-reviewer, webui-reviewer, or code-reviewer

User: document task
 └── backend-documentator or webui-documentator

User: find where X is declared
 └── explorer

User: debug error
 └── debugger

## 5. Removed legacy concepts

The following concepts are no longer part of the LifeUniform rule system:

### Removed architecture concepts

- CQRS
- MediatR
- Commands
- Queries
- Handlers
- IRequest
- IRequestHandler
- ValidationBehavior as MediatR pipeline

### Removed repository concepts

- services/{service-name}/
- separate git repositories per service
- monorepo with multiple backend services
- separate frontend repository
- Vue 3 frontend
- Pinia
- Vite
- vue-tsc
- vitest

### Removed infrastructure concepts

- Docker
- Dockerfile
- docker-compose
- docker-up.sh
- git-pull-all.sh
- git-check-committed.sh
- git-list-open-prs.sh
- mandatory infrastructure scripts

### Removed task tracking assumptions

- mandatory Singularity task agent
- mandatory task creation through a special agent
- mandatory task status transitions through a special agent

Task IDs are still supported, but they are optional. If no task tracker is available, workflows use branch names and documentation dates.

## 6. Consistency checks

The updated rules should satisfy these constraints:

### Architecture

- Domain depends on nothing.
- Application depends only on Domain.
- Infrastructure implements Application interfaces.
- Web uses Application services.
- Razor Pages PageModels are thin.
- DTOs are used for web input/output.
- Domain entities are not exposed directly to UI.

### Testing

- Tester reports only.
- Tester does not call implementator.
- Tester does not fix tests.
- Tester runs dotnet build and dotnet test.
- Publish check is optional and only for release/artifact verification.

### Review

- Reviewer does not modify code.
- Reviewer blocks on Critical and Warning issues.
- Suggestions do not block.
- Reviewer checks architecture, validation, EF Core, Razor Pages, security, and tests.

### Documentation

- Documentation is stored in root docs/.
- Documentation is not stored in services/{service}/docs/.
- Documentation is not stored in frontend/docs/.
- Documentator does not overwrite existing docs.
- Documentator records facts, not plans.

### Workflow

- One workflow = one task.
- User confirmation is requested after branch setup.
- User confirmation can be requested before PR creation.
- Commit message follows conventional style.
- Branch names are kebab-case.
- PR body includes summary, tests, review verdict, documentation link.

## 7. Remaining minor recommendations

### 7.1. Workflow skills

If separate skill files exist, ensure they mention:

- delegate testing to backend-tester or webui-tester;
- delegate review to backend-reviewer, webui-reviewer, or code-reviewer;
- delegate commits explicitly or describe that the orchestrator performs git commit.

### 7.2. Task tracker

If LifeUniform later uses a task tracker again, introduce a single optional integration point. Do not hardcode Singularity into all orchestrators.

Recommended approach:

Task ID is optional.
If task agent exists, use it.
If not, use branch/date-based documentation.

### 7.3. UI rules

Keep UI rules separate from backend rules only if they add value.

If the project remains mostly full-stack Razor Pages, it is acceptable to remove all webui-* orchestrators and use only:

- backend-planner
- backend-implementator
- backend-tester
- backend-reviewer
- backend-documentator
- backend-orchestrator
- backend-workflow-orchestrator

UI-specific concerns can be included inside backend-dev.md, backend-reviewer.md, and backend-implementator.md.

## 8. Final assessment

The updated rule system is consistent with the new LifeUniform architecture if:

- no rule references CQRS/MediatR;
- no rule references microservices or services/{service-name}/;
- no rule references Vue/Pinia/Vite;
- no rule requires Docker;
- all build/test commands use the single LifeUniform.sln;
- all documentation goes to root docs/;
- Razor Pages and Bootstrap are treated as the UI layer;
- testers remain read-only/report-only;
- reviewers remain read-only/report-only;
- orchestrators coordinate but do not implement.

The old user-manual.md and analysis-rules-skills-agents.md should be replaced with the updated versions because they describe the previous microservice/CQRS/Vue/Docker workflow.
<!-- END FILE: analysis-rules-skills-agents.md -->