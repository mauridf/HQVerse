# Estágio de build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar arquivos de projeto e restaurar dependências
COPY ["src/HQVerse.API/HQVerse.API.csproj", "src/HQVerse.API/"]
COPY ["src/HQVerse.Domain/HQVerse.Domain.csproj", "src/HQVerse.Domain/"]
COPY ["src/HQVerse.Application/HQVerse.Application.csproj", "src/HQVerse.Application/"]
COPY ["src/HQVerse.Infrastructure/HQVerse.Infrastructure.csproj", "src/HQVerse.Infrastructure/"]
COPY ["src/HQVerse.CrossCutting/HQVerse.CrossCutting.csproj", "src/HQVerse.CrossCutting/"]

RUN dotnet restore "src/HQVerse.API/HQVerse.API.csproj"

# Copiar todo o código fonte
COPY . .

# Publicar a aplicação
WORKDIR "/src/src/HQVerse.API"
RUN dotnet publish "HQVerse.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio final
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "HQVerse.API.dll"]