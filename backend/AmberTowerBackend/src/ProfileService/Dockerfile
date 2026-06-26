FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY backend/AmberTowerBackend/src/ProfileService/ProfileService.csproj backend/AmberTowerBackend/src/ProfileService/
COPY backend/AmberTowerBackend/src/Contracts/AmberTower.Profile.Contracts/AmberTower.Profile.Contracts.csproj backend/AmberTowerBackend/src/Contracts/AmberTower.Profile.Contracts/

RUN dotnet restore backend/AmberTowerBackend/src/ProfileService/ProfileService.csproj

COPY backend/AmberTowerBackend/src/ProfileService/ backend/AmberTowerBackend/src/ProfileService/
COPY backend/AmberTowerBackend/src/Contracts/AmberTower.Profile.Contracts/ backend/AmberTowerBackend/src/Contracts/AmberTower.Profile.Contracts/

RUN dotnet publish backend/AmberTowerBackend/src/ProfileService/ProfileService.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8082
EXPOSE 8082
EXPOSE 8083

HEALTHCHECK --interval=10s --timeout=3s --start-period=10s --retries=5 \
  CMD curl --fail http://127.0.0.1:8082/health || exit 1

ENTRYPOINT ["dotnet", "ProfileService.dll"]
