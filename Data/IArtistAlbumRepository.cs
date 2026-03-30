using SpotifyUtilities.Contracts;
namespace SpotifyUtilities.Data;

public interface IArtistAlbumRepository
{
    Task<string> CreateArtistAlbumAsync(ArtistAlbum artistAlbum);
}
