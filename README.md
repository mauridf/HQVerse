# 🦸 HQVerse API

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791?logo=postgresql)](https://www.postgresql.org/)
[![Render](https://img.shields.io/badge/Render-Deployed-46E3B7?logo=render)](https://render.com)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

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
- ✅ **Usuários**: Registro, login, perfil
- ✅ **Coleções**: Organização pessoal de HQs
- ✅ **Reviews**: Avaliações e notas
- ✅ **Comentários**: Discussões nas reviews
- ✅ **Favoritos**: Personagens, séries, editoras favoritas
- ✅ **Histórico de Leitura**: Progresso de leitura

### 📥 Scans
- ✅ **ScanGroups**: Grupos de scan (Darkseid Club, Zona Fantasma)
- ✅ **Scans**: Versões digitalizadas com metadados
- ✅ **ScanLinks**: Links para download e leitura online

### 🔄 Integração
- ✅ **Comic Vine**: Sincronização automática de metadados
- ✅ **Busca Global**: Pesquisa unificada em todas as entidades

---

## 🏗️ Arquitetura

```
HQVerse/
├── src/
│   ├── HQVerse.Domain/          # Entidades, Enums, Interfaces
│   ├── HQVerse.Application/     # DTOs, Services, Validators
│   ├── HQVerse.Infrastructure/  # EF Core, Repositories, Migrations
│   ├── HQVerse.API/             # Controllers, Middlewares
│   └── HQVerse.CrossCutting/    # Logging, Exceptions
├── tests/
│   ├── HQVerse.UnitTests/       # Testes Unitários (xUnit)
│   └── HQVerse.IntegrationTests/ # Testes de Integração
├── db/migrations/               # Scripts SQL versionados (DbUp)
└── docs/                        # Documentação
```

### Padrões e Princípios
- 🏛️ **Domain-Driven Design (DDD)**
- 🧱 **SOLID**
- 🗄️ **Repository Pattern**
- 📦 **Unit of Work**
- 🚦 **Rate Limiting**
- 🔐 **JWT Authentication**

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

## 📡 API Endpoints

### Autenticação
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/auth/register` | Registrar usuário |
| POST | `/api/auth/login` | Login |
| POST | `/api/auth/refresh` | Renovar token |
| POST | `/api/auth/logout` | Logout (🔒) |

### Editorial
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/publishers` | Listar editoras |
| GET | `/api/publishers/{id}` | Detalhes da editora |
| POST | `/api/publishers` | Criar editora |
| PUT | `/api/publishers/{id}` | Atualizar editora |
| DELETE | `/api/publishers/{id}` | Remover editora |
| GET | `/api/characters` | Listar personagens |
| GET | `/api/characters/search?query=` | Buscar personagens |
| GET | `/api/comic-series` | Listar séries |
| GET | `/api/comic-series/{id}` | Série com edições |
| GET | `/api/comic-issues` | Listar edições |
| GET | `/api/comic-issues/{id}` | Detalhes da edição |
| GET | `/api/comic-issues/series/{id}` | Edições por série |

### Busca
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/search?query=` | Busca global |

### Sistema
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/health` | Health check |

🔒 = Requer autenticação (Bearer Token)

---

## 🔧 Tecnologias

| Tecnologia | Versão | Uso |
|-----------|--------|-----|
| .NET | 10.0 | Framework principal |
| Entity Framework Core | 10.0 | ORM |
| PostgreSQL | 16 | Banco de dados |
| DbUp | 6.x | Migrations SQL |
| Serilog | 4.x | Logging estruturado |
| Scalar | 2.x | Documentação da API |
| JWT Bearer | 10.0 | Autenticação |
| xUnit | 2.x | Testes |
| NSubstitute | 5.x | Mocks |
| BCrypt.Net | 4.x | Hash de senhas |
| AutoMapper | 13.x | Mapeamento DTOs |

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
3. Deploy automático no Render
4. Health check verificado

📖 [Guia completo de deploy](docs/DEPLOY.md)

---

## 🧪 Testes

```bash
# Executar todos os testes
dotnet test

# Unit Tests
dotnet test tests/HQVerse.UnitTests

# Integration Tests
dotnet test tests/HQVerse.IntegrationTests

# Com cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

---

## 🔐 Segurança

- ✅ JWT Authentication com refresh tokens
- ✅ Rate Limiting (100 req/min global, 5 req/min auth)
- ✅ CORS configurado por ambiente
- ✅ Security Headers (XSS, Clickjacking, etc.)
- ✅ HTTPS em produção
- ✅ Senhas com BCrypt hash
- ✅ Tratamento global de exceções
- ✅ Validação de entrada (FluentValidation)
- ✅ SQL Injection prevenido (EF Core + parametrização)

---

## 📊 Observabilidade

- ✅ Serilog com logging estruturado
- ✅ Correlation ID em todas as requisições
- ✅ Request/Response logging
- ✅ Métricas de tempo de resposta
- ✅ Health check endpoint

---

## 🗺️ Roadmap

### v1.1 (Próxima)
- [ ] Integração completa com Comic Vine API
- [ ] Sincronização automática de metadados
- [ ] Importação de capas e thumbnails
- [ ] Cache com Redis

### v1.2
- [ ] Sistema de notificações
- [ ] Feed de atividades
- [ ] Ranking de colecionadores
- [ ] Desafios de leitura

### v2.0
- [ ] App mobile (React Native)
- [ ] Marketplace de HQs físicas
- [ ] Recomendações com IA
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

---

⭐ **Se este projeto te ajudou, deixe uma estrela!**
```
