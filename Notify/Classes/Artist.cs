namespace Notify.Classes;

public class Artist
{
    public int ArtistId { get; set; }
    public string ArtistName { get; set; }
    public byte[]? ArtistCover { get; set; }
    
    public string Genre { get; set; }

    public Artist()
    {
        
    }
    
    public Artist(string artistName, byte[] artistCover, string genre)
    {
        ArtistName = artistName;
        ArtistCover = artistCover;
        Genre = genre;
    }
}