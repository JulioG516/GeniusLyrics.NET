namespace GeniusLyrics.NET.Models;

public class Options
{
    public string? ApiKey { get; set; }
    public string? Title { get; set; }
    public string? Artist { get; set; }
    public bool OptimizeQuery { get; set; } = false;
}