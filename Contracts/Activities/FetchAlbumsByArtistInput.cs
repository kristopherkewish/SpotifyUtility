namespace SpotifyUtility.Contracts.Activities;

public record FetchAlbumsByArtistInput(
    string ArtistId, 
    int Offset
);