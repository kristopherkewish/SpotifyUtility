using SpotifyUtilities.Contracts;

namespace SpotifyUtilities.Services;

public interface ISpotifyAccessTokenService
{
    Task<SpotifyAccessToken> ExchangeAuthorizationCodeAsync(SpotifyAccessTokenRequest tokenRequest);
}