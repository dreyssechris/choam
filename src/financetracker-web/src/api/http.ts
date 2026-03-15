// src/api/http.ts
// Build a robust base URL for API calls.
// - Dev: from VITE_API_BASE_URL (e.g., http://localhost:5184)
// - Prod with subpath: e.g., /financetracker/api
const BASE = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5184";

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