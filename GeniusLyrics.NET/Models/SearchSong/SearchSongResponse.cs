﻿using System.Text.Json.Serialization;

namespace GeniusLyrics.NET.Models.SearchSong;

public class SearchSongResponse
{
    [JsonPropertyName("meta")]
    public MetaResponse Meta { get; set; }

    [JsonPropertyName("response")]
    public Response Response { get; set; }
}

public class Response
{
    [JsonPropertyName("hits")]
    public List<HitResponse> Hits { get; set; }
} 

public class HitResponse
{
    [JsonPropertyName("index")] public string Index { get; set; }
    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("result")] public Song Result { get; set; }
}

public class Song
{
    //TODO: copiar todas as propriedades do return
    
    [JsonPropertyName("id")] 
    public int Id { get; set; }

    [JsonPropertyName("full_title")] 
    public string FullTitle { get; set; }

    [JsonPropertyName("song_art_image_url")]
    public string AlbumArt { get; set; }

    [JsonPropertyName("url")] 
    public string Url { get; set; }
}