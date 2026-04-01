namespace SpotifyUtility.Contracts.Activities;

public record FetchTracksByAlbumInput(
    string AlbumId, 
    int Offset
);