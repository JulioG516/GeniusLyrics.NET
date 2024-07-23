using System.Text.RegularExpressions;
using GeniusLyrics.NET.Models;

namespace GeniusLyrics.NET;

public static class Utils
{
    public static void ValidateOptions(Options options)
    {
        if (options.ApiKey == null)
        {
            throw new ArgumentNullException(options.ApiKey, "API key it's missing in the options.");
        }

        if (options.ApiKey.Trim().Equals(""))
        {
            throw new ArgumentException("API Key in the options cannot be empty.");
        }
        
        if (options.Title == null)
        {
            throw new ArgumentNullException(options.Title, "Title it's missing in the options.");
        }

        if (options.Title.Trim().Equals(""))
        {
            throw new ArgumentException("Title in the options cannot be empty.");
        }
        
        if (options.Artist == null)
        {
            throw new ArgumentNullException(options.Artist, "Artist it's missing in the options.");
        }

        if (options.Artist.Trim().Equals(""))
        {
            throw new ArgumentException("Artist in the options cannot be empty.");
        }
    }

    public static string GetTitle(string title, string artist)
    {
        string combined = $"{title} {artist}".ToLower();

        // Remove content within parentheses
        combined = Regex.Replace(combined, @" *\([^)]*\) *", "");

        // Remove content within square brackets
        combined = Regex.Replace(combined, @" *\[[^\]]*\] *", "");

        // Remove "feat." or "ft."
        combined = Regex.Replace(combined, @"feat\.|ft\.", "");

        // Replace multiple spaces with a single space
        combined = Regex.Replace(combined, @"\s+", " ").Trim();

        return combined;
    }
}