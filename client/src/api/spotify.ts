/** Base URL for the Azure Functions API.
 *  When running locally the Vite dev server proxies /api → http://localhost:7071,
 *  so the default empty string is correct for local development.
 *  Set VITE_API_BASE_URL in .env.local to point at a deployed backend.
 */
const API_BASE = import.meta.env.VITE_API_BASE_URL ?? "";

/**
 * Calls the backend SpotifyLogin function and returns the Spotify
 * authorization URL to redirect the user to.
 */
export async function initiateSpotifyLogin(): Promise<string> {
  const res = await fetch(`${API_BASE}/api/SpotifyLogin`, {
    method: "POST",
  });

  if (!res.ok) {
    throw new Error(`Login initiation failed: ${res.status} ${res.statusText}`);
  }

  const authUrl = await res.text();
  return authUrl;
}
