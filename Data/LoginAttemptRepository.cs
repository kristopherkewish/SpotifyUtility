using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using SpotifyUtilities.Configuration;
using SpotifyUtilities.Contracts;

namespace SpotifyUtilities.Data;

/// <summary>
/// Repository for managing login attempt records in Cosmos DB.
/// </summary>
public class LoginAttemptRepository : ILoginAttemptRepository
{
    private readonly Container _container;

    public LoginAttemptRepository(IOptions<CosmosDbOptions> options)
    {
        var config = options.Value;
        var client = new CosmosClient(config.ConnectionString);
        var database = client.GetDatabase(config.DatabaseName);
        _container = database.GetContainer(config.LoginAttemptsContainer);
    }

    /// <summary>
    /// Creates a new login attempt record with the specified code verifier.
    /// </summary>
    /// <param name="codeVerifier">The PKCE code verifier to store.</param>
    /// <returns>The ID of the created login attempt.</returns>
    public async Task<string> CreateLoginAttemptAsync(string codeVerifier)
    {
        string id = Guid.NewGuid().ToString();
        var loginAttempt = new LoginAttempt(id, codeVerifier);
        await _container.CreateItemAsync(loginAttempt, new PartitionKey(id));
        return id;
    }

    /// <summary>
    /// Retrieves a login attempt by its ID.
    /// </summary>
    /// <param name="id">The ID of the login attempt to retrieve.</param>
    /// <returns>The login attempt if found, null otherwise.</returns>
    public async Task<LoginAttempt?> GetLoginAttemptAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<LoginAttempt>(
                id,
                new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    /// <summary>
    /// Deletes a login attempt by its ID.
    /// </summary>
    /// <param name="id">The ID of the login attempt to delete.</param>
    /// <returns>True if the item was deleted, false if it was not found.</returns>
    public async Task<bool> DeleteLoginAttemptAsync(string id)
    {
        try
        {
            await _container.DeleteItemAsync<LoginAttempt>(
                id,
                new PartitionKey(id));
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}
