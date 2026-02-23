import { useEffect, useRef, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { exchangeAuthorizationCode } from "../api/spotify";
import styles from "./CallbackPage.module.css";

type Status = "loading" | "success" | "error";

export default function CallbackPage() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const [status, setStatus] = useState<Status>("loading");
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  // Guard against React StrictMode double-invocation in development.
  const exchangedRef = useRef(false);

  useEffect(() => {
    if (exchangedRef.current) return;
    exchangedRef.current = true;

    const code = searchParams.get("code");
    const state = searchParams.get("state");
    const error = searchParams.get("error");

    if (error) {
      setErrorMessage(`Spotify authorization was denied: ${error}`);
      setStatus("error");
      return;
    }

    if (!code || !state) {
      setErrorMessage("Missing required parameters from Spotify callback.");
      setStatus("error");
      return;
    }

    exchangeAuthorizationCode(code, state)
      .then((redirectTo) => {
        setStatus("success");
        // Navigate to the location the backend instructed, or fall back to /dashboard.
        setTimeout(() => {
          navigate(redirectTo ?? "/dashboard");
        }, 1000);
      })
      .catch((err: unknown) => {
        setErrorMessage(
          err instanceof Error ? err.message : "An unexpected error occurred."
        );
        setStatus("error");
      });
  }, [searchParams, navigate]);

  return (
    <div className={styles.container}>
      <div className={styles.card}>
        {status === "loading" && (
          <>
            <div className={styles.spinner} aria-label="Loading" />
            <p className={styles.message}>Completing sign-in…</p>
          </>
        )}
        {status === "success" && (
          <>
            <div className={styles.successIcon} aria-hidden="true">✓</div>
            <p className={styles.message}>Signed in! Redirecting…</p>
          </>
        )}
        {status === "error" && (
          <>
            <div className={styles.errorIcon} aria-hidden="true">✕</div>
            <p className={styles.message}>Sign-in failed</p>
            <p className={styles.errorDetail}>{errorMessage}</p>
            <button
              className={styles.retryButton}
              onClick={() => navigate("/")}
            >
              Try again
            </button>
          </>
        )}
      </div>
    </div>
  );
}
