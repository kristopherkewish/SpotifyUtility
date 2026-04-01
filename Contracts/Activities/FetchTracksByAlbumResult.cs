namespace SpotifyUtility.Contracts.Activities;

public record FetchTracksByAlbumResult(
    List<AlbumTracks> AlbumTracks,
    int Total
);

public record AlbumTracks(
    string TrackId,
    string Name
);