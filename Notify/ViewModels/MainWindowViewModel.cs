using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Enumeration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Avalonia.Data;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using Microsoft.Data.Sqlite;
using Notify.Classes;
using Notify.Data;
using Notify.Views;
using TagLib.Ape;
using TagLib.Id3v2;
using File = System.IO.File;

namespace Notify.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly DatabaseConnection _dataBase;
    
    private readonly LibVLC _libVlc = new();
    
    public MediaPlayer MediaPlayer { get ;}
    
    public ObservableCollection<Song> Items { get; set; } = new();
    public int SelectedIdx {get; set;}
    public string searchBoxText {get; set;}
    
    private object? _selectedItem;
    
    private HttpClient _httpClient = new HttpClient();
    private string _url = "http://localhost:5272";
    

    public object SelectedItem
    {
        get => _selectedItem;
        set
        {
            _selectedItem = value;
            OnPropertyChanged();
        }
    }

    public MainWindowViewModel(DatabaseConnection db)
    {
        _dataBase = db;
        MediaPlayer = new MediaPlayer(_libVlc);
        SelectedIdx = -1;
    }
    
    [RelayCommand]
    private void PlayPauseClick()
    {
        Debug.WriteLine("Play/Pause Clicked");
        if(MediaPlayer.IsPlaying)
            MediaPlayer.Pause();
        else
            MediaPlayer.Play();
        
        var query = _dataBase.GetSongs();
        foreach (var song in query)
        {
            Console.WriteLine(song.SongName);
        }
    }

    [RelayCommand]
    private async Task TestAPICommand()
    {
        try
        {
            var songs = await _httpClient.GetFromJsonAsync<List<Song>>(_url + "/searchSong/" + searchBoxText);
            foreach (var song in songs)
            {
                Console.WriteLine(song.SongName);
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e.Message);
        }
       
        
    }
    
    [RelayCommand]
    private void BackClick()
    {
        Debug.WriteLine("Back Clicked");
        SelectedIdx--;
        var item = Items[SelectedIdx];
        Uri songUri = new Uri(item.SongFilePath);
        Play(songUri);
        SelectedItem = item;
        //Music.SelectedIndex--;
    }
    
    [RelayCommand]
    private void ForwardClick()
    {
        Debug.WriteLine("Forward Clicked");
        SelectedIdx++;
        var item = Items[SelectedIdx];
        Uri songUri = new Uri(item.SongFilePath);
        Play(songUri);
        SelectedItem = item;
        //Music.SelectedIndex++;
    }

    [RelayCommand]
    private void SearchClicked()
    {
        var songs = _dataBase.SearchSong(searchBoxText);
        var albums = _dataBase.SearchAlbum(searchBoxText);
        var artists = _dataBase.SearchArtist(searchBoxText);
        
        Items.Clear();
        //Console.WriteLine(searchBoxText + " " + artists + " " + albums + " " + songs);
        foreach (var song in songs)
        {
            Console.WriteLine("Song found: " +song.SongName + ", Song ID: " + song.SongId);
            Items.Add(song);
        }

        foreach (var album in albums)
        {
            Console.WriteLine("Album found: " + album.AlbumName + ", Album ID: " + album.AlbumId);
            var albumSongs = _dataBase.SearchSongByAlbum(album.AlbumId);
            foreach (var song in albumSongs)
            {
                Items.Add(song);
            }
        }

        foreach (var artist in artists)
        {
            Console.WriteLine("Artist found: " + artist.ArtistName + ", Artist ID: " + artist.ArtistId);
        }
        
    }
    public void Play(Uri uri)
    {
        using var media = new Media(_libVlc, uri);
        MediaPlayer.Play(media);
    }

    public void Stop()
    {
        MediaPlayer.Stop();
    }

    public async Task LoadFolderAsync(IStorageProvider storageProvider)
    {
        var folder = await storageProvider.TryGetFolderFromPathAsync(new Uri(
            "file:///C:\\Users\\Chevr\\OneDrive\\Documents\\Soulseek Downloads\\complete"));
        
        if (folder != null)
            await LoadSongsAsync(folder);
    }

    public async Task LoadSongsAsync(IStorageFolder folder)
    {
        if (folder != null)
        {
            await foreach (var item in folder.GetItemsAsync())
            {
                if (item is IStorageFolder subFolder)
                {
                   await LoadSongsAsync(subFolder);
                }
                if (item is IStorageFile file)
                {
                    var tags = TagLib.File.Create(file.TryGetLocalPath());

                    Bitmap? albumArt = null;
                    byte[]? artistArt = null;
                    byte[]? albumCover = null;
                    var timeSpan = tags.Properties.Duration;
                    var formattedTimeSpan = timeSpan.ToString(@"mm\:ss");
                    if (tags.Tag.Pictures.Length > 0)
                    {
                        var pic = tags.Tag.Pictures[0];
                        
                        using var stream = new MemoryStream(pic.Data.Data);

                        albumArt = new  Bitmap(stream);
                        
                        albumCover = pic.Data.Data;
                    }
                    
                    Artist artist = new Artist(tags.Tag.FirstPerformer, artistArt, tags.Tag.FirstGenre);
                    artist.ArtistId = _dataBase.AddArtist(artist);
                    Album album = new Album(tags.Tag.Album, tags.Tag.FirstPerformer, albumCover, artist.ArtistId);
                    album.AlbumId = _dataBase.AddAlbum(album);
                    Song song = new Song(file.TryGetLocalPath(),tags.Tag.Title, tags.Tag.Album, tags.Tag.FirstPerformer, (int)tags.Properties.Duration.TotalSeconds, albumCover, artist.ArtistId, album.AlbumId);
                    _dataBase.AddSong(song);
                    Items.Add(song);
                }
            }
        }
    }
    
    public string Greetings => "Hello world!";
}
