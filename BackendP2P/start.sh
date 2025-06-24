#!/bin/bash
set -e

# Применяем миграции
dotnet ef database update

# Запускаем приложение
dotnet BackendP2P.dll