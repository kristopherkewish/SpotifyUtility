using Newtonsoft.Json;

namespace SpotifyUtilities.Contracts;

public record ArtistAlbum(
    [property: JsonProperty("id")] string JobId,
    string ArtistId,
    List<AlbumEntry> Albums);

public record AlbumEntry(
    string AlbumId,
    int ReleaseYear
);
