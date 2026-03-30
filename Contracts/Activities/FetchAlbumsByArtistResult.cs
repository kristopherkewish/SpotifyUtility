namespace SpotifyUtility.Contracts.Activities;

public record FetchAlbumsByArtistResult(
    List<SpotifyAlbum> ArtistAlbums,
    int Total
);