namespace Notify.Classes;

public class Album
{
    public int AlbumId { get; set; }
    public string AlbumName { get; set; }
    public string AlbumArtistName { get; set; }
    public byte[]? AlbumCover { get; set; }
    
    public int ArtistId { get; set; }

    public Album()
    {
        
    }
    
    public Album(string  albumName, string albumArtistName, byte[]? albumCover, int artistId)
    {
        AlbumName = albumName;
        AlbumArtistName = albumArtistName;
        AlbumCover = albumCover;
        ArtistId = artistId;
    }
}