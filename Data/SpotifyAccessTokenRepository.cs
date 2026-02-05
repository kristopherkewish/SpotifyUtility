using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using SpotifyUtilities.Configuration;
using SpotifyUtilities.Contracts;

namespace SpotifyUtilities.Data;

/// <summary>
/// Repository for managing Spotify access token records in Cosmos DB.
/// </summary>
public class SpotifyAccessTokenRepository : ISpotifyAccessTokenRepository
{
    private readonly Container _container;

    public SpotifyAccessTokenRepository(IOptions<CosmosDbOptions> options)
    {
        var config = options.Value;
        var client = new CosmosClient(config.ConnectionString);
        var database = client.GetDatabase(config.DatabaseName);
        _container = database.GetContainer(config.AccessTokensContainer);
    }

    /// <summary>
    /// Creates or updates a Spotify access token record.
    /// </summary>
    /// <param name="token">The access token to store.</param>
    public async Task SaveAccessTokenAsync(SpotifyAccessToken token)
    {
        await _container.UpsertItemAsync(token, new PartitionKey(token.AccessToken));
    }

    /// <summary>
    /// Retrieves a Spotify access token by its value.
    /// </summary>
    /// <param name="accessToken">The access token string to retrieve.</param>
    /// <returns>The access token record if found, null otherwise.</returns>
    public async Task<SpotifyAccessToken?> GetAccessTokenAsync(string accessToken)
    {
        try
        {
            var response = await _container.ReadItemAsync<SpotifyAccessToken>(
                accessToken,
                new PartitionKey(accessToken));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    /// <summary>
    /// Deletes a Spotify access token by its value.
    /// </summary>
    /// <param name="accessToken">The access token string to delete.</param>
    /// <returns>True if the item was deleted, false if it was not found.</returns>
    public async Task<bool> DeleteAccessTokenAsync(string accessToken)
    {
        try
        {
            await _container.DeleteItemAsync<SpotifyAccessToken>(
                accessToken,
                new PartitionKey(accessToken));
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}
