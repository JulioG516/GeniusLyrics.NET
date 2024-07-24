using System.Text.Json;
using GeniusLyrics.NET.Models;
using GeniusLyrics.NET.Models.SearchSong;

namespace GeniusLyrics.NET;

/// <summary>
/// Genius API client to fetch data as album art, song info and lyrics.
/// </summary>
public static class Genius
{
    private static readonly string BaseUrl = "https://api.genius.com/";

    /// <summary>
    /// Returns a list of songs based on the song title and artist name.
    /// </summary>
    /// <param name="options">API key = credentials for GeniusAPI, Title = Song Title, Artist = Artist Name</param>
    /// <returns>List of Song objects or null if not found.</returns>
    public static async Task<List<Song>?> SearchSong(Options options)
    {
        Utils.ValidateOptions(options);

        var songQuery = options.OptimizeQuery
            ? Utils.GetTitle(options.Title!, options.Artist!)
            : $"{options.Title} {options.Artist}";

        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(BaseUrl + $"search?q={songQuery}"),
            Method = HttpMethod.Get,
        };

        request.Headers.Add("Authorization", $"Bearer {options.ApiKey}");
        var responseMessage = await client.SendAsync(request);

        responseMessage.EnsureSuccessStatusCode();

        var jsonResponse = await responseMessage.Content.ReadAsStringAsync();

        var songResponse = JsonSerializer.Deserialize<SearchSongResponse>(jsonResponse);

        if (songResponse is null || !songResponse.Response.Hits.Any())
            return null;

        var songs = songResponse.Response.Hits.Select(s => s.Result).ToList();
        return songs;
    }

    /// <summary>
    /// Retrieves a single song based on the song title and artist name.
    /// </summary>
    /// <param name="options">API key = credentials for GeniusAPI, Title = Song Title, Artist = Artist Name</param>
    /// <returns>A single Song object or null if not found.</returns>
    public static async Task<Song?> GetSong(Options options)
    {
        Utils.ValidateOptions(options);

        var results = await SearchSong(options);
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
    /// <param name="apiKey"></param>
    /// <param name="id"></param>
    /// <returns>A single Song object or null if not found.</returns>
    /// <exception cref="ArgumentException">When apiKey it's empty or null or id have negative value.</exception>
    public static async Task<Song?> GetSongById(string apiKey, ulong id)
    {
        if (string.IsNullOrEmpty(apiKey))
            throw new ArgumentException("Api Key must have some value.");
        if (id <= 0)
            throw new ArgumentException("Id cant be negative values.");

        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(BaseUrl + $"songs/{id}"),
            Method = HttpMethod.Get,
        };

        request.Headers.Add("Authorization", $"Bearer {apiKey}");
        var responseMessage = await client.SendAsync(request);

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
    /// <param name="options">API key = credentials for GeniusAPI, Title = Song Title, Artist = Artist Name</param>
    /// <returns>URL of the album art as a string, or null if not found.</returns>
    public static async Task<string?> GetAlbumArt(Options options)
    {
        Utils.ValidateOptions(options);

        var results = await SearchSong(options);
        if (results is null || !results.Any())
            return null;

        var albumArt = results[0].AlbumArt;
        return albumArt;
    }
}