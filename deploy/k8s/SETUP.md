# K3s + Flux CD Setup auf Raspberry Pi

## Voraussetzungen
- Raspberry Pi 4 (4GB RAM, 64-bit OS)
- SSH-Zugang zum Pi
- GitHub Repository: `dreyssechris/finance-tracker`
- Cloudflare Tunnel bereits konfiguriert

---

## 1. K3s installieren

```bash
# Auf dem Pi via SSH:
curl -sfL https://get.k3s.io | sh -

# Warten bis K3s bereit ist
sudo k3s kubectl get nodes

# kubectl ohne sudo nutzbar machen
mkdir -p ~/.kube
sudo cp /etc/rancher/k3s/k3s.yaml ~/.kube/config
sudo chown $(id -u):$(id -g) ~/.kube/config
export KUBECONFIG=~/.kube/config
echo 'export KUBECONFIG=~/.kube/config' >> ~/.bashrc
```

## 2. Namespaces erstellen

```bash
kubectl create namespace financetracker-dev
kubectl create namespace financetracker-prod
```

## 3. ghcr.io Registry-Secret anlegen

Ein GitHub Personal Access Token (PAT) mit `read:packages` Scope erstellen:
https://github.com/settings/tokens/new?scopes=read:packages

```bash
# In beiden Namespaces:
for NS in financetracker-dev financetracker-prod; do
  kubectl -n $NS create secret docker-registry ghcr-credentials \
    --docker-server=ghcr.io \
    --docker-username=dreyssechris \
    --docker-password=<GITHUB_PAT> \
    --docker-email=<EMAIL>
done
```

## 4. App-Secrets anlegen

```bash
# Dev Secrets
kubectl -n financetracker-dev create secret generic postgres-credentials \
  --from-literal=POSTGRES_USER=ft_dbadmin \
  --from-literal=POSTGRES_PASSWORD=<DEV_PASSWORD> \
  --from-literal=POSTGRES_DB=financedb_dev

kubectl -n financetracker-dev create secret generic app-secrets \
  --from-literal=CONNECTION_STRING="Host=postgres;Port=5432;Database=financedb_dev;Username=ft_dbadmin;Password=<DEV_PASSWORD>"

# Prod Secrets
kubectl -n financetracker-prod create secret generic postgres-credentials \
  --from-literal=POSTGRES_USER=ft_dbadmin \
  --from-literal=POSTGRES_PASSWORD=<PROD_PASSWORD> \
  --from-literal=POSTGRES_DB=financedb_prod

kubectl -n financetracker-prod create secret generic app-secrets \
  --from-literal=CONNECTION_STRING="Host=postgres;Port=5432;Database=financedb_prod;Username=ft_dbadmin;Password=<PROD_PASSWORD>"
```

## 5. Flux CD installieren

```bash
# Flux CLI installieren
curl -s https://fluxcd.io/install.sh | sudo bash

# GitHub PAT mit repo Scope erstellen (für Flux)
export GITHUB_TOKEN=<GITHUB_PAT_WITH_REPO_SCOPE>
export GITHUB_USER=dreyssechris

# Flux Bootstrap (verbindet Flux mit dem GitHub Repo)
flux bootstrap github \
  --owner=$GITHUB_USER \
  --repository=FinanceTracker \
  --branch=main \
  --path=deploy/k8s/flux \
  --personal
```

## 6. Flux Kustomizations erstellen

```bash
# Dev environment
flux create kustomization financetracker-dev \
  --source=flux-system \
  --path="deploy/k8s/overlays/dev" \
  --prune=true \
  --interval=1m \
  --target-namespace=financetracker-dev

# Prod environment
flux create kustomization financetracker-prod \
  --source=flux-system \
  --path="deploy/k8s/overlays/prod" \
  --prune=true \
  --interval=1m \
  --target-namespace=financetracker-prod
```

## 7. Cloudflare Tunnel umkonfigurieren

Die Cloudflare Tunnel Config (`~/.cloudflared/config.yml`) aktualisieren:

```yaml
tunnel: <TUNNEL_ID>
credentials-file: /home/pi/.cloudflared/<TUNNEL_ID>.json

ingress:
  - hostname: dev.chrispicloud.dev
    service: http://localhost:80
  - hostname: chrispicloud.dev
    service: http://localhost:80
  - service: http_status:404
```

Traefik (K3s built-in) lauscht auf Port 80 und routet basierend auf Ingress-Regeln.

```bash
# Cloudflared als systemd Service neustarten
sudo systemctl restart cloudflared
```

## 8. ImagePullSecrets den Deployments zuweisen

Die `imagePullSecrets` müssen in den base-Manifests referenziert werden.
Dies ist bereits in `api.yaml` und `web.yaml` konfiguriert.

## 9. Verifizierung

```bash
# Flux Status prüfen
flux get kustomizations

# Pods prüfen
kubectl get pods -n financetracker-dev
kubectl get pods -n financetracker-prod

# Ingress prüfen
kubectl get ingress -n financetracker-dev
kubectl get ingress -n financetracker-prod

# Logs prüfen
kubectl logs -n financetracker-dev -l app=api
kubectl logs -n financetracker-dev -l app=web

# RAM-Verbrauch prüfen
kubectl top nodes
kubectl top pods -A
```

## 10. Bestehende Docker Services stoppen

Nachdem K3s läuft und alles funktioniert:

```bash
# Portainer und Docker Compose Services stoppen
docker compose -f deploy/compose/compose.base.yml -f deploy/compose/compose.remote-dev.yml down

# Optional: Docker selbst stoppen um RAM zu sparen
sudo systemctl stop docker
sudo systemctl disable docker
```

---

## CI/CD Workflow

1. Feature-Branch erstellen, lokal entwickeln
2. PR auf `main` erstellen → CI läuft (lint, build, type-check)
3. PR mergen → CD-Dev baut ARM64 Images, pusht zu ghcr.io, updated dev Manifests
4. Flux erkennt Änderung → deployed auf `dev.chrispicloud.dev/financetracker`
5. Testen auf Dev
6. Git Tag erstellen (`git tag v1.0.0 && git push --tags`) → CD-Prod baut + deployed auf `chrispicloud.dev/financetracker`
