FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY backend/AmberTowerBackend/src/AuthService/AuthService.csproj backend/AmberTowerBackend/src/AuthService/
COPY backend/AmberTowerBackend/src/Contracts/AmberTower.Auth.Contracts/AmberTower.Auth.Contracts.csproj backend/AmberTowerBackend/src/Contracts/AmberTower.Auth.Contracts/
RUN dotnet restore backend/AmberTowerBackend/src/AuthService/AuthService.csproj

COPY backend/AmberTowerBackend/src/AuthService/ backend/AmberTowerBackend/src/AuthService/
COPY backend/AmberTowerBackend/src/Contracts/AmberTower.Auth.Contracts/ backend/AmberTowerBackend/src/Contracts/AmberTower.Auth.Contracts/
RUN dotnet publish backend/AmberTowerBackend/src/AuthService/AuthService.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

ENV Ports__Http=8080
ENV Ports__Grpc=8081

EXPOSE 8080
EXPOSE 8081

HEALTHCHECK --interval=10s --timeout=3s --start-period=10s --retries=5 \
    CMD curl --fail http://127.0.0.1:8080/health || exit 1

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AuthService.dll"]
