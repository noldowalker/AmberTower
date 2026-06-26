FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY backend/AmberTowerBackend/src/ApiGateway/ApiGateway.csproj backend/AmberTowerBackend/src/ApiGateway/
COPY backend/AmberTowerBackend/src/Contracts/AmberTower.Auth.Contracts/AmberTower.Auth.Contracts.csproj backend/AmberTowerBackend/src/Contracts/AmberTower.Auth.Contracts/
COPY backend/AmberTowerBackend/src/Contracts/AmberTower.Profile.Contracts/AmberTower.Profile.Contracts.csproj backend/AmberTowerBackend/src/Contracts/AmberTower.Profile.Contracts/
RUN dotnet restore backend/AmberTowerBackend/src/ApiGateway/ApiGateway.csproj

COPY backend/AmberTowerBackend/src/ApiGateway/ backend/AmberTowerBackend/src/ApiGateway/
COPY backend/AmberTowerBackend/src/Contracts/AmberTower.Auth.Contracts/ backend/AmberTowerBackend/src/Contracts/AmberTower.Auth.Contracts/
COPY backend/AmberTowerBackend/src/Contracts/AmberTower.Profile.Contracts/ backend/AmberTowerBackend/src/Contracts/AmberTower.Profile.Contracts/
RUN dotnet publish backend/AmberTowerBackend/src/ApiGateway/ApiGateway.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

HEALTHCHECK --interval=10s --timeout=3s --start-period=5s --retries=3 \
    CMD curl --fail http://127.0.0.1:8080/health || exit 1

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ApiGateway.dll"]
