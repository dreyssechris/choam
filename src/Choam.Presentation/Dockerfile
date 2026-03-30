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

CMD ["dotnet", "watch", "run", "--project", "Choam.Presentation/Choam.Presentation.csproj", "--urls", "http://0.0.0.0:8080", "--non-interactive"]

# =========================
# Debug target (stable process for attach)
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS debug
WORKDIR /src

ENV NUGET_PACKAGES=/root/.nuget/packages

COPY . .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

CMD ["dotnet", "run", "--project", "Choam.Presentation/Choam.Presentation.csproj", "--urls", "http://0.0.0.0:8080"]

# =========================
# Build target (CI/CD + prod publish)
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

ENV NUGET_PACKAGES=/root/.nuget/packages

# Copy all csproj files for restore caching
COPY Choam.Domain/Choam.Domain.csproj Choam.Domain/
COPY Choam.Application/Choam.Application.csproj Choam.Application/
COPY Choam.Infrastructure/Choam.Infrastructure.csproj Choam.Infrastructure/
COPY Choam.Presentation/Choam.Presentation.csproj Choam.Presentation/

RUN dotnet restore Choam.Presentation/Choam.Presentation.csproj --nologo

# Copy remaining source
COPY . .

# Publish
RUN dotnet publish Choam.Presentation/Choam.Presentation.csproj \
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

ENTRYPOINT ["dotnet", "Choam.Presentation.dll"]
