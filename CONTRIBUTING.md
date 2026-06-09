# 🤝 Contribuindo com HQVerse

Obrigado pelo interesse em contribuir! 🎉

## Como Contribuir

### 1. Reportar Bugs
Abra uma issue com:
- Descrição clara do bug
- Passos para reproduzir
- Comportamento esperado vs atual
- Screenshots (se aplicável)

### 2. Sugerir Features
- Verifique se já não existe uma issue similar
- Descreva a feature e seu valor para o projeto

### 3. Pull Requests

#### Setup Local
```bash
git clone https://github.com/SEU_USUARIO/HQVerse.git
cd HQVerse
dotnet restore
dotnet build
```

#### Fluxo de Trabalho
1. Crie uma branch: `git checkout -b feature/nome-da-feature`
2. Faça suas alterações
3. Execute os testes: `dotnet test`
4. Commit seguindo Conventional Commits
5. Push e abra um PR

#### Padrões de Código
- Siga os princípios SOLID
- Mantenha a arquitetura DDD
- Documente endpoints com XML comments
- Adicione testes para novas funcionalidades
- Use nomes descritivos em inglês

#### Conventional Commits
```
feat: adiciona endpoint de busca por ISBN
fix: corrige validação de email no registro
docs: atualiza README com novos endpoints
test: adiciona testes para ComicIssueService
refactor: extrai lógica de validação para serviço
```

## Obrigado! 🦸
```