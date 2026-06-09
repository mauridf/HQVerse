#!/bin/bash
# Script para deploy no Render via GitHub Actions

echo "🏗️  Iniciando build da HQVerse API..."

# Restaurar dependências
dotnet restore

# Compilar
dotnet build --configuration Release --no-restore

# Executar testes
dotnet test --configuration Release --no-build --verbosity normal

# Publicar
dotnet publish src/HQVerse.API/HQVerse.API.csproj \
    --configuration Release \
    --output ./publish \
    --no-build

echo "✅ Build concluído! Arquivos em ./publish"