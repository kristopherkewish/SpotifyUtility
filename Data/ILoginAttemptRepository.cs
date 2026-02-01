using SpotifyUtilities.Contracts;

namespace SpotifyUtilities.Data;

/// <summary>
/// Interface for managing login attempt records.
/// </summary>
public interface ILoginAttemptRepository
{
    /// <summary>
    /// Creates a new login attempt record with the specified code verifier.
    /// </summary>
    /// <param name="codeVerifier">The PKCE code verifier to store.</param>
    /// <returns>The ID of the created login attempt.</returns>
    Task<string> CreateLoginAttemptAsync(string codeVerifier);

    /// <summary>
    /// Retrieves a login attempt by its ID.
    /// </summary>
    /// <param name="loginAttemptId">The ID of the login attempt to retrieve.</param>
    /// <returns>The login attempt if found, null otherwise.</returns>
    Task<LoginAttempt?> GetLoginAttemptAsync(string loginAttemptId);

    /// <summary>
    /// Deletes a login attempt by its ID.
    /// </summary>
    /// <param name="loginAttemptId">The ID of the login attempt to delete.</param>
    /// <returns>True if the item was deleted, false if it was not found.</returns>
    Task<bool> DeleteLoginAttemptAsync(string loginAttemptId);
}
