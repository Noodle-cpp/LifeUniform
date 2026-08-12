---
name: storage
description: S3/MinIO: IStorageService, presigned URLs. Use when working with file uploads, S3, media.
---

# S3/MinIO

## IStorageService

UploadAsync, DownloadAsync, ExistsAsync, DeleteAsync, GeneratePresignedUploadUrl, GeneratePresignedDownloadUrl

## Ключи

`{entity-type}/{entity-id}/{file-id}/{filename}` — lowercase, / разделитель

## Presigned URLs

Для прямой загрузки клиентом; TTL ограничен
