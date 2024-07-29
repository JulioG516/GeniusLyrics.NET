using System.Text.RegularExpressions;
using GeniusLyrics.NET.Models;
using HtmlAgilityPack;

namespace GeniusLyrics.NET;

internal static class Utils
{
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

    public static async Task<string?> ExtractLyrics(string url)
    {
        HtmlAgilityPack.HtmlWeb web = new();

        var htmlDoc = await web.LoadFromWebAsync(url);
        if (htmlDoc is null)
            return null;

        // var nodes = htmlDoc.DocumentNode.SelectNodes("//div[class='Lyrics']");
        // if (nodes == null)

        var nodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'Lyrics__Container')]");

        if (nodes == null) return null;
        var lyrics = "";
        foreach (var node in nodes)
        {
            var textOnly = node.SelectNodes(".//text()");

            if (textOnly == null)
                continue;

            foreach (var textNode in textOnly)
            {
                lyrics += HtmlEntity.DeEntitize(textNode.InnerText).Trim() + "\n";
            }
        }

        return lyrics.Trim();
    }
}