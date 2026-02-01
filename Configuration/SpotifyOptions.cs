using System.ComponentModel.DataAnnotations;

namespace SpotifyUtilities.Configuration;

/// <summary>
/// Configuration options for Spotify API integration.
/// </summary>
public class SpotifyOptions
{
    public const string SectionName = "Spotify";

    /// <summary>
    /// The Spotify application client ID.
    /// </summary>
    [Required]
    public required string ClientId { get; set; }

    /// <summary>
    /// The redirect URI for OAuth callback.
    /// </summary>
    [Required]
    public required string RedirectUri { get; set; }

    /// <summary>
    /// The scopes to request during authorization.
    /// </summary>
    public string Scope { get; set; } = "playlist-modify-public";
}
