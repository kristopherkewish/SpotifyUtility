namespace SpotifyUtility.Contracts.Activities;

public record PersistArtistAlbumsInput(
    string JobId,
    string ArtistId, 
    List<string> AlbumIds
);