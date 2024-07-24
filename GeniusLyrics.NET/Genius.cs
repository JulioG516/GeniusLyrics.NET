using System.Text.Json;
using GeniusLyrics.NET.Models;
using GeniusLyrics.NET.Models.SearchSong;

namespace GeniusLyrics.NET;

public static class Genius
{
    private static readonly string BaseUrl = "https://api.genius.com/";

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

    public static async Task<Song?> GetSong(Options options)
    {
        Utils.ValidateOptions(options);

        
        
        
        
        return null;
    }
}