using System.Text.Json;
using GeniusLyrics.NET.Models.SearchSong;

namespace GeniusLyrics.NET;

/// <summary>
/// Genius API client to fetch data as album art, song info and lyrics.
/// </summary>
public class GeniusClient
{
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://api.genius.com/")
    };

    private string ApiKey { get; }

    /// <summary>
    /// Initializes a new instance of the GeniusClient class with the specified API key.
    /// </summary>
    /// <param name="apiKey">The Genius API key to authenticate requests.</param>
    /// <exception cref="ArgumentNullException">Thrown if the API key is null or empty.</exception>
    public GeniusClient(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new ArgumentNullException(nameof(apiKey), "Api key cannot be null.");
        }

        ApiKey = apiKey;

        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
    }


    /// <summary>
    /// Returns a list of songs based on the song title and artist name.
    /// </summary>
    /// <param name="title">The song title to search for.</param>
    /// <param name="artist">The artist's name to search for.</param>
    /// <param name="optimizeQuery">Indicates whether to optimize the query by removing unnecessary elements from the title and artist.</param>
    /// <returns>List of Song objects or null if not found.</returns>
    public async Task<List<Song>?> SearchSong(string title, string artist, bool optimizeQuery = false)
    {
        if (string.IsNullOrEmpty(title))
        {
            throw new ArgumentNullException(nameof(title), "Title cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(artist))
        {
            throw new ArgumentNullException(nameof(artist), "Artist value cannot be null or empty.");
        }

        var songQuery = optimizeQuery
            ? Utils.GetTitle(title, artist)
            : $"{title} {artist}";

        var responseMessage = await _httpClient.GetAsync($"search?q={songQuery}");

        responseMessage.EnsureSuccessStatusCode();

        var jsonResponse = await responseMessage.Content.ReadAsStringAsync();

        var songResponse = JsonSerializer.Deserialize<SearchSongResponse>(jsonResponse);

        if (songResponse?.Response.Hits is null)
            return null;

        var songs = songResponse.Response.Hits.Select(s => s.Result).ToList();
        return songs;
    }


    /// <summary>
    /// Retrieves a single song based on the song title and artist name.
    /// </summary>
    /// <param name="title">The song title to search for.</param>
    /// <param name="artist">The artist's name to search for.</param>
    /// <param name="optimizeQuery">Indicates whether to optimize the query by removing unnecessary elements from the title and artist.</param>
    /// <returns>A single Song object or null if not found.</returns>
    public async Task<Song?> GetSong(string title, string artist, bool optimizeQuery = false)
    {
        if (string.IsNullOrEmpty(title))
        {
            throw new ArgumentNullException(nameof(title), "Title cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(artist))
        {
            throw new ArgumentNullException(nameof(artist), "Artist value cannot be null or empty.");
        }

        var results = await SearchSong(title, artist, optimizeQuery);
        if (results is null || !results.Any())
            return null;

        var lyrics = await Utils.ExtractLyrics(results[0].Url);

        var song = new Song
        {
            Id = results[0].Id,
            FullTitle = results[0].FullTitle,
            AlbumArt = results[0].AlbumArt,
            Url = results[0].Url,
            Lyrics = lyrics,
        };

        return song;
    }

    /// <summary>
    /// Retrieves a single song based on the song id.
    /// </summary>
    /// <param name="id">The song id to search for.</param>
    /// <returns>A single Song object or null if not found.</returns>
    /// <exception cref="ArgumentException">When id it's negative or 0.</exception>
    public async Task<Song?> GetSongById(ulong id)
    {
        if (id <= 0)
            throw new ArgumentException("Id cant be negative values.");

        var responseMessage = await _httpClient.GetAsync($"songs/{id}");

        responseMessage.EnsureSuccessStatusCode();

        var jsonResponse = await responseMessage.Content.ReadAsStringAsync();

        var songResponse = JsonSerializer.Deserialize<SearchSongResponse>(jsonResponse);

        if (songResponse is null || songResponse.Response.Song is null)
            return null;

        var song = songResponse.Response.Song;

        var lyrics = await Utils.ExtractLyrics(song.Url);

        song.Lyrics = lyrics;

        return song;
    }

    /// <summary>
    /// Retrieves the album art URL for a given song title and artist name.
    /// </summary>
    /// <param name="title">The song title to search for.</param>
    /// <param name="artist">The artist's name to search for.</param>
    /// <param name="optimizeQuery">Indicates whether to optimize the query by removing unnecessary elements from the title and artist.</param>
    /// <returns>URL of the album art as a string, or null if not found.</returns>
    public async Task<string?> GetAlbumArt(string title, string artist, bool optimizeQuery = false)
    {
        if (string.IsNullOrEmpty(title))
        {
            throw new ArgumentNullException(nameof(title), "Title cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(artist))
        {
            throw new ArgumentNullException(nameof(artist), "Artist value cannot be null or empty.");
        }
        
        var results = await SearchSong(title, artist, optimizeQuery);
        if (results is null || !results.Any())
            return null;

        var albumArt = results[0].AlbumArt;
        return albumArt;
    }
}