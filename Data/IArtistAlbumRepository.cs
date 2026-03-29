namespace SpotifyUtilities.Data;

public interface IArtistAlbumRepository
{
    Task<string> CreateArtistAlbumAsync(string jobId, string artistId, List<string> albumIds);
}
