# 🦸 HQVerse API

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791?logo=postgresql)](https://www.postgresql.org/)
[![Render](https://img.shields.io/badge/Render-Deployed-46E3B7?logo=render)](https://render.com)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Version](https://img.shields.io/badge/version-2.0.0-blue.svg)](https://github.com/SEU_USUARIO/HQVerse/releases)

API para gestão de Scans de HQs - Comunidade de leitores de quadrinhos digitalizados.

> **"IMDb + Skoob + Discogs para HQs"** 📚

---

## 📋 Funcionalidades

### 📖 Editorial
- ✅ **Publishers**: Editoras (Marvel, DC, Panini, Abril)
- ✅ **Characters**: Personagens com detalhes e afiliações
- ✅ **Teams**: Equipes (Liga da Justiça, Vingadores, X-Men)
- ✅ **ComicSeries**: Séries com contagem de edições
- ✅ **ComicIssues**: Edições individuais com metadados
- ✅ **StoryArcs**: Arcos de história (Guerra Civil, Crise Infinita)
- ✅ **Creators**: Autores, artistas, desenhistas

### 👥 Comunidade
- ✅ **Usuários**: Registro, login, perfil com JWT + Refresh Token
- ✅ **Coleções**: Organização pessoal de HQs (WISHLIST, READING, READ, ABANDONED)
- ✅ **Reviews**: Avaliações com notas de 1 a 10
- ✅ **Comentários**: Discussões nas reviews
- ✅ **Likes**: Sistema de curtidas em reviews
- ✅ **Favoritos**: Personagens, séries, editoras favoritas
- ✅ **Histórico de Leitura**: Progresso de leitura com páginas e percentual

### 📥 Scans
- ✅ **ScanGroups**: Grupos de scan (Darkseid Club, Zona Fantasma, Guardiões do Globo)
- ✅ **Scans**: Versões digitalizadas com metadados completos
- ✅ **ScanLinks**: Links para download e leitura online (DOWNLOAD, READONLINE, MIRROR, TORRENT)

### 🔄 Integração
- ✅ **Comic Vine API**: Busca e sincronização de metadados
- ✅ **Sync Automático**: Importação de Publishers, Characters, Teams, Creators, Volumes, Issues, StoryArcs
- ✅ **Busca Global**: Pesquisa unificada em todas as entidades locais
- ✅ **Busca Comic Vine**: Pesquisa direta na API com 7 tipos de recursos

---

## 🏗️ Arquitetura

```
HQVerse/
├── src/
│   ├── HQVerse.Domain/          # Entidades, Enums, Interfaces (25+ entidades)
│   ├── HQVerse.Application/     # DTOs, Services, Validators, Interfaces
│   ├── HQVerse.Infrastructure/  # EF Core, Repositories, DbUp, ComicVine Client
│   ├── HQVerse.API/             # Controllers (8), Middlewares, Config
│   └── HQVerse.CrossCutting/    # Logging, Exception Handling, Extensions
├── tests/
│   ├── HQVerse.UnitTests/       # Testes Unitários (xUnit + NSubstitute)
│   └── HQVerse.IntegrationTests/ # Testes de Integração
├── db/migrations/               # 16 scripts SQL versionados (DbUp)
├── .github/workflows/           # CI/CD Pipeline
└── docs/                        # Documentação
```

### Padrões e Princípios
- 🏛️ **Domain-Driven Design (DDD)** - 5 camadas independentes
- 🧱 **SOLID** - Princípios de design de software
- 🗄️ **Repository Pattern** - Abstração de acesso a dados
- 📦 **Unit of Work** - Controle transacional
- 🚦 **Rate Limiting** - Proteção contra abusos
- 🔐 **JWT Authentication** - Autenticação stateless
- 🌐 **RESTful** - API REST completa

---

## 🚀 Como Rodar Localmente

### Pré-requisitos
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL 16](https://www.postgresql.org/download/)
- [Git](https://git-scm.com/)

### 1. Clonar o Repositório
```bash
git clone https://github.com/SEU_USUARIO/HQVerse.git
cd HQVerse
```

### 2. Configurar Banco de Dados
```sql
CREATE DATABASE hqverse_dev;
CREATE USER hqverse_user WITH PASSWORD 'hqverse_password_dev';
GRANT ALL PRIVILEGES ON DATABASE hqverse_dev TO hqverse_user;
```

### 3. Configurar Secrets
```bash
cd src/HQVerse.API

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=hqverse_dev;Username=hqverse_user;Password=hqverse_password_dev"
dotnet user-secrets set "Jwt:Secret" "Sua-Chave-Secreta-JWT-Com-32-Caracteres-No-Minimo!!"
dotnet user-secrets set "ComicVine:ApiKey" "sua-api-key-da-comic-vine"
```

### 4. Executar
```bash
cd ../..
dotnet run --project src/HQVerse.API
```

### 5. Acessar
- **Scalar API Docs**: http://localhost:5001/scalar
- **Health Check**: http://localhost:5001/api/health

---

## 📡 API Endpoints (v2.0)

### 🔐 Autenticação
| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| POST | `/api/auth/register` | Registrar usuário | ❌ |
| POST | `/api/auth/login` | Login | ❌ |
| POST | `/api/auth/refresh` | Renovar token | ❌ |
| POST | `/api/auth/logout` | Logout | 🔒 |

### 📖 Editorial
| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/publishers` | Listar editoras (paginado) | ❌ |
| GET | `/api/publishers/{id}` | Detalhes da editora | ❌ |
| POST | `/api/publishers` | Criar editora | ❌ |
| PUT | `/api/publishers/{id}` | Atualizar editora | ❌ |
| DELETE | `/api/publishers/{id}` | Remover editora | ❌ |
| GET | `/api/characters` | Listar personagens | ❌ |
| GET | `/api/characters/search?query=` | Buscar personagens | ❌ |
| GET | `/api/characters/{id}` | Detalhes do personagem | ❌ |
| GET | `/api/comic-series` | Listar séries | ❌ |
| GET | `/api/comic-series/search?query=` | Buscar séries | ❌ |
| GET | `/api/comic-series/{id}` | Série com edições | ❌ |
| GET | `/api/comic-issues` | Listar edições (paginado) | ❌ |
| GET | `/api/comic-issues/{id}` | Detalhes da edição | ❌ |
| GET | `/api/comic-issues/series/{id}` | Edições por série | ❌ |
| POST | `/api/comic-issues` | Criar edição | ❌ |

### ⭐ Reviews
| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/reviews/issue/{issueId}` | Avaliações da edição | ❌ |
| GET | `/api/reviews/user/{userId}` | Avaliações do usuário | ❌ |
| GET | `/api/reviews/{id}` | Detalhes da avaliação | ❌ |
| POST | `/api/reviews` | Criar avaliação | 🔒 |
| PUT | `/api/reviews/{id}` | Editar avaliação | 🔒 |
| DELETE | `/api/reviews/{id}` | Remover avaliação | 🔒 |
| POST | `/api/reviews/{reviewId}/comments` | Comentar | 🔒 |
| DELETE | `/api/reviews/comments/{commentId}` | Remover comentário | 🔒 |
| POST | `/api/reviews/{reviewId}/like` | Curtir/Descurtir | 🔒 |

### 📚 Collections
| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/collections/user/{userId}` | Listar coleções | ❌ |
| GET | `/api/collections/{id}` | Detalhes da coleção | ❌ |
| POST | `/api/collections` | Criar coleção | 🔒 |
| PUT | `/api/collections/{id}` | Editar coleção | 🔒 |
| DELETE | `/api/collections/{id}` | Remover coleção | 🔒 |
| POST | `/api/collections/{id}/issues` | Adicionar edição | 🔒 |
| PUT | `/api/collections/{id}/issues/{issueId}` | Atualizar status | 🔒 |
| DELETE | `/api/collections/{id}/issues/{issueId}` | Remover edição | 🔒 |
| POST | `/api/collections/reading/start` | Iniciar leitura | 🔒 |
| PUT | `/api/collections/reading/{issueId}` | Atualizar progresso | 🔒 |
| GET | `/api/collections/reading/{issueId}` | Ver progresso | 🔒 |
| GET | `/api/collections/reading/current` | Leituras ativas | 🔒 |

### 📥 Scans
| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/scans/groups` | Listar grupos | ❌ |
| GET | `/api/scans/groups/{id}` | Detalhes do grupo | ❌ |
| POST | `/api/scans/groups` | Criar grupo | 🔒 |
| GET | `/api/scans/issue/{issueId}` | Scans por edição | ❌ |
| GET | `/api/scans/latest` | Scans recentes | ❌ |
| GET | `/api/scans/search?query=` | Buscar scans | ❌ |
| GET | `/api/scans/{id}` | Detalhes do scan | ❌ |
| POST | `/api/scans` | Criar scan | 🔒 |
| DELETE | `/api/scans/{id}` | Remover scan | 🔒 |

### 🔄 Comic Vine
| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/comicvine/search?query=&resourceType=` | Buscar na API | ❌ |
| POST | `/api/comicvine/sync/publisher/{id}` | Sincronizar editora | 🔒 |
| POST | `/api/comicvine/sync/character/{id}` | Sincronizar personagem | 🔒 |
| POST | `/api/comicvine/sync/team/{id}` | Sincronizar equipe | 🔒 |
| POST | `/api/comicvine/sync/creator/{id}` | Sincronizar criador | 🔒 |
| POST | `/api/comicvine/sync/volume/{id}` | Sincronizar série | 🔒 |
| POST | `/api/comicvine/sync/issue/{id}` | Sincronizar edição | 🔒 |
| POST | `/api/comicvine/sync/storyarc/{id}` | Sincronizar arco | 🔒 |

### 🔍 Busca
| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/search?query=` | Busca global | ❌ |

### 🏥 Sistema
| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/health` | Health check | ❌ |

🔒 = Requer autenticação (Bearer Token)  
🔒* = Requer role Admin ou Moderator

---

## 🔧 Tecnologias

| Tecnologia | Versão | Uso |
|-----------|--------|-----|
| .NET | 10.0 | Framework principal |
| Entity Framework Core | 10.0 | ORM |
| PostgreSQL | 16 | Banco de dados |
| DbUp | 6.x | Migrations SQL versionadas |
| Serilog | 4.x | Logging estruturado |
| Scalar | 2.x | Documentação da API |
| JWT Bearer | 10.0 | Autenticação |
| BCrypt.Net | 4.x | Hash de senhas |
| AutoMapper | 16.x | Mapeamento DTOs |
| xUnit | 2.x | Testes unitários |
| NSubstitute | 5.x | Mocks para testes |
| FluentAssertions | 8.x | Assertions fluentes |
| FluentValidation | 11.x | Validação de entrada |

---

## 🌍 Deploy

O deploy é feito automaticamente no [Render](https://render.com) via GitHub Actions.

### Ambientes
| Ambiente | URL | Status |
|----------|-----|--------|
| Produção | `https://hqverse-api.onrender.com` | 🟢 |
| Scalar Docs | `https://hqverse-api.onrender.com/scalar` | 📚 |

### CI/CD Pipeline
1. Push na branch `main`
2. GitHub Actions: Build + Test
3. Deploy automático no Render via Deploy Hook
4. Health check verificado automaticamente

📖 [Guia completo de deploy](docs/DEPLOY.md)

---

## 🧪 Testes

```bash
# Executar todos os testes
dotnet test

# Unit Tests (Services)
dotnet test tests/HQVerse.UnitTests

# Integration Tests (API)
dotnet test tests/HQVerse.IntegrationTests

# Com cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

### Cobertura Atual
- ✅ PublisherService: 6 testes
- ✅ AuthService: 5 testes
- ✅ HealthEndpoint: 2 testes de integração

---

## 🔐 Segurança

- ✅ **JWT Authentication** com refresh tokens (2h access + 7d refresh)
- ✅ **Rate Limiting**: 100 req/min global, 5 req/min auth, 30 req/min público
- ✅ **CORS**: Configurado por ambiente (dev/prod)
- ✅ **Security Headers**: X-Content-Type-Options, X-Frame-Options, X-XSS-Protection
- ✅ **HTTPS**: Forçado em produção com HSTS
- ✅ **BCrypt**: Hash de senhas com salt automático
- ✅ **Global Exception Handler**: Tratamento padronizado de erros
- ✅ **Input Validation**: FluentValidation em todas as entradas
- ✅ **SQL Injection**: Prevenido via EF Core + parâmetros
- ✅ **Correlation ID**: Rastreamento de requisições

---

## 📊 Observabilidade

- ✅ **Serilog**: Logging estruturado com níveis configuráveis
- ✅ **Correlation ID**: Header `X-Correlation-Id` em todas as respostas
- ✅ **Request Logging**: Método, path, status code, tempo de resposta
- ✅ **HTTP Logging**: Request/Response body (limitado a 4KB)
- ✅ **Health Check**: Endpoint `/api/health` com status, versão e ambiente

---

## 📈 Estatísticas do Projeto

| Métrica | Valor |
|---------|-------|
| **Entidades** | 25+ |
| **Controllers** | 8 |
| **Endpoints** | 55+ |
| **Migrations SQL** | 16 scripts |
| **Testes** | 13+ |
| **Serviços** | 8 |
| **Repositórios** | 7 |
| **DTOs** | 30+ |

---

## 🗺️ Roadmap

### ✅ v2.0 (Atual)
- [x] Integração completa com Comic Vine API
- [x] Sistema de Reviews, Comments e Likes
- [x] Coleções com status de leitura
- [x] Progresso de leitura
- [x] Scans e ScanGroups com links
- [x] 7 tipos de recursos Comic Vine sincronizáveis
- [x] 55+ endpoints REST

### 🔜 v2.1 (Próxima)
- [ ] Cache com Redis para Comic Vine
- [ ] Importação automática de capas
- [ ] Sistema de notificações
- [ ] Feed de atividades
- [ ] Ranking de colecionadores

### 🔮 v3.0 (Futuro)
- [ ] App mobile (React Native)
- [ ] Marketplace de HQs físicas
- [ ] Recomendações com IA
- [ ] Desafios de leitura
- [ ] Importação via ISBN/UPC

---

## 👥 Contribuição

1. Fork o projeto
2. Crie uma branch: `git checkout -b feature/nova-feature`
3. Commit: `git commit -m 'feat: nova feature'`
4. Push: `git push origin feature/nova-feature`
5. Abra um Pull Request

### Padrão de Commits
Seguimos [Conventional Commits](https://www.conventionalcommits.org/):
- `feat:` Nova funcionalidade
- `fix:` Correção de bug
- `docs:` Documentação
- `test:` Testes
- `refactor:` Refatoração
- `chore:` Tarefas de manutenção

---

## 📄 Licença

MIT © 2026 HQVerse

---

## 🦸‍♂️ Autor

**HQVerse Team**
- 📧 contact@hqverse.dev
- 🌐 https://hqverse-api.onrender.com
- 📚 [Documentação Scalar](https://hqverse-api.onrender.com/scalar)

---

⭐ **Se este projeto te ajudou, deixe uma estrela!**
