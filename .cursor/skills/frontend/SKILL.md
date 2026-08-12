---
name: frontend
description: Frontend development: Vue 3, Composition API, Pinia, TypeScript, Axios. Use when writing Vue components, working in frontend/, or frontend development.
---

# Frontend (Vue 3)

## Обязательно

- Composition API, `<script setup>`, TypeScript
- JSON от API в **camelCase** — типы и поля только camelCase (hotelId, countryName)

## Структура компонента

```vue
<template>...</template>
<script setup lang="ts">
// imports, props, emits, ref/reactive, computed, methods
</script>
<style scoped>...</style>
```

## Pinia, API, роутинг

- Pinia stores для глобального состояния
- Axios + interceptors для токенов и ошибок
- Vue Router, route guards для защиты

## Производительность

- v-show вместо v-if для частых переключений
- key для списков
- Динамические импорты для тяжёлых компонентов
