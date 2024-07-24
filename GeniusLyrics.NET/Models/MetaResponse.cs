using System.Text.Json.Serialization;

namespace GeniusLyrics.NET.Models;

internal class MetaResponse
{
    [JsonPropertyName("status")]
    public int Status { get; set; }
}