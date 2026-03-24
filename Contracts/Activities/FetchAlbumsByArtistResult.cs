namespace SpotifyUtility.Contracts.Activities;

public record FetchAlbumsByArtistResult(
    List<string> AlbumIds,
    int Total
);