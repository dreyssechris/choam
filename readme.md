# finance-tracker

Personal finance tracking application built with .NET 10 and Vue 3.

## Tech Stack

**Backend:** ASP.NET Core 10 (Clean Architecture) with PostgreSQL 16
**Frontend:** Vue 3 + Quasar 2 + TypeScript
**Infrastructure:** K3s (Kubernetes) on Raspberry Pi, Flux CD (GitOps), GitHub Actions CI/CD

## Architecture

```
src/
├── FinanceTracker.Domain/          # Entities, value objects
├── FinanceTracker.Application/     # Use cases, interfaces
├── FinanceTracker.Infrastructure/  # EF Core, repositories
├── FinanceTracker.Presentation/    # ASP.NET Core API
└── financetracker-web/             # Vue 3 + Quasar SPA
```

## Development

### Prerequisites
- Docker Desktop
- VS Code with Dev Containers extension

### Quick Start
1. Clone the repo
2. Copy `.env.example` to `.env` and set credentials
3. Open in VS Code → "Reopen in Container"
4. Services start automatically (API on 8080, Web on 5173)

### Local Development (without Dev Container)
```bash
# Backend
dotnet restore FinanceTracker.slnx
dotnet run --project src/FinanceTracker.Presentation

# Frontend
cd src/financetracker-web
npm install
npm run dev
```

## Deployment

Deployed on Raspberry Pi 4 via K3s with Flux CD for GitOps.

### CI/CD Workflow
1. Create feature branch, develop locally
2. Open PR to `main` → CI runs (lint, type-check, build)
3. Merge to `main` → CD builds ARM64 images, pushes to ghcr.io, updates dev manifests
4. Flux CD auto-deploys to `dev.chrispicloud.dev/financetracker`
5. Create git tag (`v1.x.x`) → CD deploys to `chrispicloud.dev/financetracker`

K8s manifests and infrastructure docs are in the [homelab](https://github.com/dreyssechris/homelab) repo.

## Documentation

See the [Wiki](https://github.com/dreyssechris/finance-tracker/wiki) for detailed documentation on architecture, development, and the roadmap.

## License

Private project.
