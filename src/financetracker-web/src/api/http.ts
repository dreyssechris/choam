// src/api/http.ts
// Build a robust base URL for API calls.
// - Dev (Docker Compose): Vite proxy rewrites /api → api:8080
// - Prod (K8s): /financetracker/api → Traefik → api:8080
const BASE = import.meta.env.VITE_API_BASE_URL ?? "/financetracker/api";

// Safe join to avoid double slashes
function join(base: string, path: string) {
  const b = base.replace(/\/+$/, "");
  const p = path.replace(/^\/+/, "");
  return `${b}/${p}`;
}

export async function api<T>(path: string, options?: RequestInit): Promise<T> {
  const url = join(BASE, path);

  const res = await fetch(url, {
    headers: { "Content-Type": "application/json" },
    ...options,
  });

  if (!res.ok) {
    // Optional: better error surface for debugging
    const text = await res.text().catch(() => "");
    throw new Error(text || `HTTP ${res.status} for ${url}`);
  }

  if (res.status === 204 || res.status === 205) {
    return undefined as unknown as T;
  }

  return res.json() as Promise<T>;
}