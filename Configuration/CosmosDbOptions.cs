using System.ComponentModel.DataAnnotations;

namespace SpotifyUtilities.Configuration;

/// <summary>
/// Configuration options for Cosmos DB.
/// </summary>
public class CosmosDbOptions
{
    public const string SectionName = "CosmosDb";

    /// <summary>
    /// The Cosmos DB connection string.
    /// </summary>
    [Required]
    public required string ConnectionString { get; set; }

    /// <summary>
    /// The database name.
    /// </summary>
    public string DatabaseName { get; set; } = "SpotifyUtility";

    /// <summary>
    /// The container name for login attempts.
    /// </summary>
    public string LoginAttemptsContainer { get; set; } = "LoginAttempts";
}
