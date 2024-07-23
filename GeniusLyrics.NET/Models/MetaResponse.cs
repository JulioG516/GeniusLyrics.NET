using System.Text.Json.Serialization;

namespace GeniusLyrics.NET.Models;

public class MetaResponse
{
    [JsonPropertyName("status")]
    public int Status { get; set; }
}