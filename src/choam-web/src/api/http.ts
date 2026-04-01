import { useAuth } from "../auth/useAuth";

const BASE = import.meta.env.VITE_API_BASE_URL ?? "/api";

function join(base: string, path: string) {
  const b = base.replace(/\/+$/, "");
  const p = path.replace(/^\/+/, "");
  return `${b}/${p}`;
}

export async function api<T>(path: string, options?: RequestInit): Promise<T> {
  const url = join(BASE, path);
  const { getAccessToken } = useAuth();
  const token = getAccessToken();

  const headers: Record<string, string> = {
    "Content-Type": "application/json",
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
  };

  const res = await fetch(url, {
    ...options,
    headers: { ...headers, ...(options?.headers as Record<string, string>) },
  });

  if (!res.ok) {
    const text = await res.text().catch(() => "");
    throw new Error(text || `HTTP ${res.status} for ${url}`);
  }

  if (res.status === 204 || res.status === 205) {
    return undefined as unknown as T;
  }

  return res.json() as Promise<T>;
}