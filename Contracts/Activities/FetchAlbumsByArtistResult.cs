namespace SpotifyUtility.Contracts.Activities;

public record FetchAlbumsByArtistResult(
    string[] AlbumIds
);