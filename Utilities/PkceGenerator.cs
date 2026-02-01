using System.Security.Cryptography;
using System.Text;

namespace SpotifyUtilities.Utilities;

/// <summary>
/// Generates PKCE (Proof Key for Code Exchange) code verifiers and challenges
/// for OAuth 2.0 authorization flows.
/// </summary>
public static class PkceGenerator
{
    private const string AllowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    /// <summary>
    /// Generates a cryptographically random code verifier string.
    /// </summary>
    /// <param name="length">The length of the code verifier (recommended: 43-128 characters).</param>
    /// <returns>A random code verifier string.</returns>
    public static string GenerateCodeVerifier(int length = 64)
    {
        var randomBytes = RandomNumberGenerator.GetBytes(length);
        return new string([.. randomBytes.Select(x => AllowedCharacters[x % AllowedCharacters.Length])]);
    }

    /// <summary>
    /// Generates a code challenge from a code verifier using SHA256 and base64url encoding.
    /// </summary>
    /// <param name="codeVerifier">The code verifier to transform.</param>
    /// <returns>A base64url-encoded SHA256 hash of the code verifier.</returns>
    public static string GenerateCodeChallenge(string codeVerifier)
    {
        byte[] challengeBytes = SHA256.HashData(Encoding.UTF8.GetBytes(codeVerifier));
        return Convert.ToBase64String(challengeBytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
