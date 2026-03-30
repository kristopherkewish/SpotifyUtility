namespace SpotifyUtility.Contracts.Activities;

public record FetchAlbumsByArtistResult(
    List<ArtistAlbum> ArtistAlbums,
    int Total
);

public record ArtistAlbum(
    string AlbumId,
    int ReleaseYear
);