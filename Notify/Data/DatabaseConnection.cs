using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Notify.Classes;

namespace Notify.Data;

public class DatabaseConnection
{
    private readonly string _connectionString;
    public DatabaseConnection()
    {
        var dbDirectory = Path.Combine(AppContext.BaseDirectory, "data");
        Directory.CreateDirectory(dbDirectory);
        
        var dbPath = Path.Combine(dbDirectory, "music.db");
        _connectionString = $"Data Source={dbPath}";
        
        Console.WriteLine($"Data base {dbPath}");
        
        var sqliteConnection = CreateDatabaseConnection();
        sqliteConnection.Open();
        
        using (sqliteConnection)
        {
            sqliteConnection.Execute
            (
                @"CREATE TABLE IF NOT EXISTS Songs
                (
                    SongId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    SongFilePath TEXT NOT NULL UNIQUE,
                    SongName TEXT NOT NULL,
                    ArtistName TEXT NOT NULL,
                    AlbumName TEXT NOT NULL,
                    Duration INTEGER NOT NULL,
                    AlbumCover BLOB,
                    AlbumId INTEGER NOT NULL,
                    ArtistId INTEGER NOT NULL,
                    FOREIGN KEY(AlbumId) REFERENCES Albums(AlbumId),
                    FOREIGN KEY(ArtistId) REFERENCES Artists(ArtistId)
                );
               CREATE TABLE IF NOT EXISTS Artists
                (
                    ArtistId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    ArtistName TEXT NOT NULL UNIQUE,
                    ArtistCover BLOB,
                    Genre TEXT
                );
                CREATE TABLE IF NOT EXISTS Albums
                (
                    AlbumId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    AlbumName TEXT NOT NULL UNIQUE,
                    AlbumArtistName TEXT NOT NULL,
                    AlbumCover BLOB,
                    ArtistId INTEGER,
                    FOREIGN KEY(ArtistId) REFERENCES Artists(ArtistId)
                );
            ");
        }
    }
    
    private SqliteConnection CreateDatabaseConnection()
    {
        return new SqliteConnection(_connectionString);
    }

    public IEnumerable<Song> GetSongs()
    {
        using var connection = new SqliteConnection(_connectionString);
        return connection.Query<Song>("SELECT * FROM Songs");
    }

    public void AddSong(Song song)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(@"INSERT OR IGNORE INTO Songs (SongFilePath, SongName, ArtistName, AlbumName, Duration, AlbumCover, AlbumId, ArtistId)
                                 VALUES (@SongFilePath, @SongName, @ArtistName, @AlbumName, @Duration, @AlbumCover, @AlbumId, @ArtistId)", song);
        
        song.SongId = connection.ExecuteScalar<int>("SELECT last_insert_rowid();");
    }

    public int AddAlbum(Album album)
    {
        using var connection = new SqliteConnection(_connectionString);
        
        int? id = connection.QueryFirstOrDefault<int?>("SELECT AlbumId FROM Albums WHERE AlbumName = @AlbumName", album);
        
        if (id.HasValue)
            return id.Value;
        
        connection.Execute(@"INSERT OR IGNORE INTO Albums (AlbumName, AlbumArtistName, AlbumCover, ArtistId) VALUES (@AlbumName, @AlbumArtistName, @AlbumCover, @ArtistId)", album);
        
        return connection.ExecuteScalar<int>("SELECT last_insert_rowid();");
    }

    public int AddArtist(Artist artist)
    {
        using var connection = new SqliteConnection(_connectionString);
        
        int? id = connection.QueryFirstOrDefault<int?>("SELECT ArtistId FROM Artists WHERE ArtistName = @ArtistName", artist);

        if (id.HasValue)
            return id.Value;
        
        connection.Execute(@"INSERT OR IGNORE INTO Artists (ArtistName, ArtistCover, Genre) VALUES (@ArtistName, @ArtistCover, @Genre)", artist);
        
        return connection.ExecuteScalar<int>("SELECT last_insert_rowid();");
    }

    public IEnumerable<Artist> SearchArtist(string artistName)
    {
        using var connection = new SqliteConnection(_connectionString);
        return connection.Query<Artist>(@"SELECT * FROM Artists WHERE ArtistName LIKE @ArtistName", new { ArtistName = "%" + artistName + "%"});
    }

    public IEnumerable<Song> SearchSong(string songName)
    {
        using var connection = new SqliteConnection(_connectionString);
        return connection.Query<Song>(@"SELECT * FROM Songs WHERE SongName LIKE @SongName", new {SongName = "%" + songName + "%"});
    }

    public IEnumerable<Album> SearchAlbum(string albumName)
    {
        using var connection = new SqliteConnection(_connectionString);
        return connection.Query<Album>(@"SELECT * FROM Albums WHERE AlbumName LIKE @AlbumName", new  {AlbumName = "%" + albumName + "%"});
    }

    public IEnumerable<Song> SearchSongByAlbum(int albumId)
    {
        using var connection = new SqliteConnection(_connectionString);
        return connection.Query<Song>(@"SELECT * FROM Songs WHERE AlbumId = @AlbumId", new {AlbumId = albumId});
    }
}