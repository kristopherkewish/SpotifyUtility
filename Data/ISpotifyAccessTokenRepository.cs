using SpotifyUtilities.Contracts;

namespace SpotifyUtilities.Data;

/// <summary>
/// Interface for managing Spotify access token records.
/// </summary>
public interface ISpotifyAccessTokenRepository
{
    /// <summary>
    /// Creates or updates a Spotify access token record.
    /// </summary>
    /// <param name="token">The access token to store.</param>
    Task SaveAccessTokenAsync(SpotifyAccessToken token);

    /// <summary>
    /// Retrieves a Spotify access token by its value.
    /// </summary>
    /// <param name="accessToken">The access token string to retrieve.</param>
    /// <returns>The access token record if found, null otherwise.</returns>
    Task<SpotifyAccessToken?> GetAccessTokenAsync(string accessToken);

    /// <summary>
    /// Retrieves the most recently added Spotify access token.
    /// </summary>
    /// <returns>The most recent access token record if any exist, null otherwise.</returns>
    Task<SpotifyAccessToken?> GetMostRecentAccessTokenAsync();

    /// <summary>
    /// Deletes a Spotify access token by its value.
    /// </summary>
    /// <param name="accessToken">The access token string to delete.</param>
    /// <returns>True if the item was deleted, false if it was not found.</returns>
    Task<bool> DeleteAccessTokenAsync(string accessToken);
}
