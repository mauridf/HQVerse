# 🚀 Deploy da HQVerse API no Render

## Pré-requisitos

1. Conta no [Render.com](https://render.com) (plano gratuito)
2. Repositório no GitHub com o código
3. PostgreSQL configurado no Render

## Passo a Passo

### 1. Criar Banco de Dados no Render

1. Acesse **Render Dashboard → New → PostgreSQL**
2. Configure:
   - Name: `hqverse-db`
   - Database: `hqverse_prod`
   - User: `hqverse_user`
   - Region: `Oregon (US West)`
   - Plan: `Free`
3. Clique **Create Database**
4. Copie a **Internal Database URL** (será usada depois)

### 2. Criar Web Service

1. Acesse **Render Dashboard → New → Web Service**
2. Conecte seu repositório GitHub
3. Configure:
   - Name: `hqverse-api`
   - Runtime: `Docker`
   - Branch: `main`
   - Plan: `Free`
4. Adicione as variáveis de ambiente:
   - `ASPNETCORE_ENVIRONMENT` = `Production`
   - `ConnectionStrings__DefaultConnection` = (Internal Database URL)
   - `Jwt__Secret` = (Generate)
   - `ComicVine__ApiKey` = sua chave da Comic Vine
5. Clique **Create Web Service**

### 3. Configurar Deploy Hook (CI/CD)

1. No Render, vá em **Settings → Deploy Hook**
2. Copie a URL do deploy hook
3. No GitHub, vá em **Settings → Secrets → Actions**
4. Adicione: `RENDER_DEPLOY_HOOK` = URL copiada

### 4. Verificar Deploy

- Health check: `https://hqverse-api.onrender.com/api/health`
- Documentação: `https://hqverse-api.onrender.com/scalar`

## URLs Importantes

| Ambiente | URL |
|----------|-----|
| Produção | `https://hqverse-api.onrender.com` |
| Scalar | `https://hqverse-api.onrender.com/scalar` |
| Health | `https://hqverse-api.onrender.com/api/health` |