# ============================================================
# Metin2Bausia.Web — Blazor Server admin aplikace
# ============================================================

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Zkopírovat projekty
COPY src/Metin2Bausia.Web/Metin2Bausia.Web.csproj src/Metin2Bausia.Web/
COPY src/SharedServices/SharedServices.csproj src/SharedServices/

# Restore
RUN dotnet restore src/Metin2Bausia.Web/Metin2Bausia.Web.csproj

# Zkopírovat zbytek
COPY src/ src/

# Publish
RUN dotnet publish src/Metin2Bausia.Web/Metin2Bausia.Web.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

HEALTHCHECK --interval=30s --timeout=10s --retries=3 \
  CMD wget -qO- http://localhost:8080/ || exit 1

EXPOSE 8080
ENTRYPOINT ["dotnet", "Metin2Bausia.Web.dll"]
