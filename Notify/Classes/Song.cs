using System;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;

namespace Notify.Classes;

public class Song
{
    public int SongId { get; set; }
    public int ArtistId { get; set; }
    public int AlbumId { get; set; }
    public string SongFilePath { get; set; }
    public string SongName { get; set; }
    public string AlbumName { get; set; }
    public string ArtistName { get; set; }
    public int Duration { get; set; }
    public byte[]? AlbumCover { get; set; }

    public Song()
    {
        
    }
    
    public Song(string? filePath, string songName, string albumName, string artistName, int duration, byte[]? albumCover, int artistId, int albumId)
    {
        SongFilePath = filePath;
        SongName = songName;
        AlbumName = albumName;
        ArtistName = artistName;
        Duration = duration;
        AlbumCover = albumCover;
        ArtistId = artistId;
        AlbumId = albumId;
    }
    
}