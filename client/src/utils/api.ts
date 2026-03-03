// utils/apiClient.ts
import * as z from 'zod';
import { getDefaultStore } from 'jotai/vanilla';
/**
 * Internal state for token refresh (replaces global `isRefreshing`)
 */
type RefreshState = {
  promise: Promise<boolean>;
};

// Map to track active refreshes per user ID
const refreshCache = new Map<string, RefreshState>();

async function refreshToken(userId?: string): Promise<boolean> {
  if (!userId) return false;

  // Check cache first — avoid duplicate refreshes for same user
  const existing = refreshCache.get(userId);

  if (existing) {
    console.log(`[RefreshToken] Reusing pending refresh for user: ${userId}`);
    return existing.promise;
  }

  const promise = fetch('/api/auth/refresh-token', {
    method: 'POST',
    credentials: 'include',
    headers: { 'Content-Type': 'application/json' }
  })
    .then(async (response) => {
      if (!response.ok) {
        const errorText = await response.text().catch(() => 'Unknown refresh failure');
        console.warn(`[RefreshToken] Failed (${response.status}):`, errorText);
        return false;
      }

      return true;
    })
    .catch((error) => {
      console.error(`[RefreshToken] Network error for user ${userId}:`, error);
      return false;
    });

  // Cache the promise
  const state = { promise };
  refreshCache.set(userId, state);

  // Cleanup after completion (success or failure)
  promise.finally(() => {
    refreshCache.delete(userId);
  });

  return promise;
}
let refreshPromise: Promise<boolean> | null = null;

async function getRefreshPromise() {
  if (!refreshPromise) {
    refreshPromise = refreshToken().finally(() => {
      refreshPromise = null;
    });
  }
  return refreshPromise;
}

export class ApiError extends Error {
  status: number;
  constructor(status: number, message: string) {
    super(message);
    this.status = status;
  }
}

export async function api<T>(
  url: string,
  options?: {
    init?: RequestInit;
    skipRetry?: boolean;
  }
): Promise<T> {
  const { init = {}, skipRetry = false } = options ?? {};

  // Ensure headers is a plain object for safe property assignment
  const headers: Record<string, string> = {
    ...(init.headers instanceof Headers
      ? Object.fromEntries(init.headers.entries())
      : (Array.isArray(init.headers)
          ? Object.fromEntries(init.headers)
          : init.headers) as Record<string, string>),
  };

  if ((headers as any)['authorization']) {
    throw new Error('Authorization header not allowed — use httpOnly cookies');
  }

  if (init.body && !(init.body instanceof FormData)) {
    headers['Content-Type'] = 'application/json';
  }

  let body: BodyInit | undefined = undefined;

  if (init.body) {
    body =
      typeof init.body === 'string' || init.body instanceof FormData
        ? init.body
        : JSON.stringify(init.body);
  }

  const response = await fetch(url, {
    ...init,
    method: init.method ?? 'GET',
    headers,
    credentials: 'include',
    body,
  });

  if (!response.ok) {
    if (response.status === 401 && !skipRetry) {
      const refreshed = await getRefreshPromise();
      if (refreshed) {
        return api<T>(url, { ...options, skipRetry: true });
      }
      throw new ApiError(401, 'Session expired. Please log in again.');
    }

    let errorMessage = `HTTP ${response.status}`;

    try {
      const contentType = response.headers.get('content-type');
      if (contentType?.includes('application/json')) {
        const data = await response.json();
        errorMessage =
          data?.message ||
          data?.errors?.[0]?.msg ||
          JSON.stringify(data);
      } else {
        errorMessage += `: ${await response.text()}`;
      }
    } catch {}

    throw new ApiError(response.status, errorMessage);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  const contentType = response.headers.get('content-type');

  if (contentType?.includes('application/json')) {
    return (await response.json()) as T;
  }

  return (await response.text()) as unknown as T;
}
