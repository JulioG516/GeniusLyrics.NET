# GeniusLyrics.NET

![NuGet Version](https://img.shields.io/nuget/v/GeniusLyrics.NET)


a library that allows developers to interact with the Genius API. You can retrieve song information, scrape lyrics, and retrieve album art from the Genius platform. 🎶

## Usage 

### Get one specific song and lyrics with title and artist name
``` 
var genius =  new GeniusClient(yourApiKey);

var brooklynBaby = await genius.GetSong("Brooklyn Baby", "Lana Del Rey", true);

Console.WriteLine(brooklynBaby.FullTitle + "\n" + brooklynBaby.Lyrics);

// How many times baby appears at brooklyn baby ?
Console.WriteLine($"Baby appears: {Regex.Matches(brooklynBaby.Lyrics, "baby").Count}");
```


### Get one song with the id
```
var songById = await genius.GetSongById(5138841);

Console.WriteLine(songById.FullTitle + "\n" + songById.Lyrics);
```

### Get url of an album art 
```
var albumArt = await genius.GetAlbumArt("rushing back", "flume");

// https://images.genius.com/e9031955f1346e634e1e3c701ba3a37e.1000x1000x1.jpg
```
