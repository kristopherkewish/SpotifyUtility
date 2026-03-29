using Newtonsoft.Json;

namespace SpotifyUtilities.Contracts;

public record ArtistAlbum(
    [property: JsonProperty("id")] string JobId,
    string ArtistId,
    List<string> AlbumIds);
