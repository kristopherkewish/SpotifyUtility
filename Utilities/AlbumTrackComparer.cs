using SpotifyUtility.Contracts.Activities;

namespace SpotifyUtilities.Utilities;

public class AlbumTrackComparer : IEqualityComparer<AlbumTracks>
{
    public bool Equals(AlbumTracks x, AlbumTracks y)
    {
        return NormaliseTrackName(x.Name) == NormaliseTrackName(y.Name);
    }

    public int GetHashCode(AlbumTracks obj)
    {
        return NormaliseTrackName(obj.Name).GetHashCode();
    }

    private static string NormaliseTrackName(string name)
    {
        return name.ToLower().Trim();
    }
}
