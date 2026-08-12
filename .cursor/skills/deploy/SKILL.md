---
name: deploy
description: Деплой: Test (локально), RC, Prod. Use when deploying or configuring environments.
---

# Деплой

## Три среды

| Среда | Описание | URL | Скрипт |
|-------|----------|-----|--------|
| **Test** | Локально в Docker | http://localhost:8888, https://localhost:8443 | docker-up.sh |
| **RC** | Release Candidate, удалённый сервер | `DEPLOY_HOST` / `PUBLIC_BASE_URL` | deploy-rc.sh |
| **Prod** | Production | `DEPLOY_HOST` / `PUBLIC_BASE_URL` | deploy-prod.sh |

## Команды

```bash
# Test — локально
./infrastructure/scripts/docker-up.sh

# RC — деплой (см. deploy-rc.sh, переменные окружения)
./infrastructure/scripts/deploy-rc.sh

# Prod — деплой (см. deploy-prod.sh)
./infrastructure/scripts/deploy-prod.sh
```

## Bootstrap нового сервера (RC/Prod)

```bash
ssh root@<IP> 'bash -s' < infrastructure/scripts/server-bootstrap.sh
```

## Универсальный деплой (переменные)

```bash
DEPLOY_HOST=<RC_IP> PUBLIC_BASE_URL=<RC_URL> ./infrastructure/scripts/deploy-remote.sh
DEPLOY_HOST=<PROD_IP> PUBLIC_BASE_URL=<PROD_URL> ./infrastructure/scripts/deploy-remote.sh
```
