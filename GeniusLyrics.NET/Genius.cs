using System.Text.Json;
using GeniusLyrics.NET.Models;
using GeniusLyrics.NET.Models.SearchSong;

namespace GeniusLyrics.NET;

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

        if (!songResponse.Response.Hits.Any())
        {
            return null;
        }

        var songs = songResponse.Response.Hits.Select(s => s.Result).ToList();
        var lyrics = Utils.ExtractLyrics(songs[0].Url);
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