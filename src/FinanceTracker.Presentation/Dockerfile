# =========================
# Dev target (local hot reload)
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS dev
WORKDIR /src

ENV NUGET_PACKAGES=/root/.nuget/packages

# Copy everything (dev wants full tree for dotnet watch)
COPY . .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

CMD ["dotnet", "watch", "run", "--project", "FinanceTracker.Presentation/FinanceTracker.Presentation.csproj", "--urls", "http://0.0.0.0:8080", "--non-interactive"]

# =========================
# Debug target (stable process for attach)
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS debug
WORKDIR /src

ENV NUGET_PACKAGES=/root/.nuget/packages

COPY . .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

CMD ["dotnet", "run", "--project", "FinanceTracker.Presentation/FinanceTracker.Presentation.csproj", "--urls", "http://0.0.0.0:8080"]

# =========================
# Build target (CI/CD + prod publish)
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

ENV NUGET_PACKAGES=/root/.nuget/packages

# Copy all csproj files for restore caching
COPY FinanceTracker.Domain/FinanceTracker.Domain.csproj FinanceTracker.Domain/
COPY FinanceTracker.Application/FinanceTracker.Application.csproj FinanceTracker.Application/
COPY FinanceTracker.Infrastructure/FinanceTracker.Infrastructure.csproj FinanceTracker.Infrastructure/
COPY FinanceTracker.Presentation/FinanceTracker.Presentation.csproj FinanceTracker.Presentation/

RUN dotnet restore FinanceTracker.Presentation/FinanceTracker.Presentation.csproj --nologo

# Copy remaining source
COPY . .

# Publish
RUN dotnet publish FinanceTracker.Presentation/FinanceTracker.Presentation.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false \
    --nologo

# =========================
# Prod target (runtime only)
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS prod
WORKDIR /app

COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "FinanceTracker.Presentation.dll"]
