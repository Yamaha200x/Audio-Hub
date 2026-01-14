using TagLib;
using Audio_Hub.Droid.Services;

namespace Audio_Hub.Droid.Platforms.Android;

/// <summary>
/// Android implementation using TagLibSharp library.
/// Supports: MP3, M4A, FLAC, WAV, OGG, WMA, and more.
/// </summary>
public class AudioMetadataService : IAudioMetadataService
{
    public AudioMetadata GetMetadata(string filePath)
    {
        try
        {
            var file = TagLib.File.Create(filePath);
            var fileInfo = new FileInfo(filePath);

            return new AudioMetadata
            {
                Title = file.Tag.Title ?? Path.GetFileNameWithoutExtension(filePath),
                Artist = file.Tag.FirstPerformer ?? file.Tag.FirstAlbumArtist ?? "Unknown Artist",
                Album = file.Tag.Album ?? "Unknown Album",
                Genre = file.Tag.FirstGenre,
                Year = file.Tag.Year,
                Duration = file.Properties.Duration,
                TrackNumber = (int?)file.Tag.Track,
                FileSize = fileInfo.Length,
                FilePath = filePath,
                AlbumArt = file.Tag.Pictures.FirstOrDefault()?.Data.Data
            };
        }
        catch (Exception ex)
        {
            global::Android.Util.Log.Error("AudioMetadata", $"Error reading {filePath}: {ex.Message}");
            
            // Return minimal metadata on error
            return new AudioMetadata
            {
                Title = Path.GetFileNameWithoutExtension(filePath),
                Artist = "Unknown",
                Album = "Unknown",
                FilePath = filePath
            };
        }
    }

    public async Task<AudioMetadata> GetMetadataAsync(string filePath)
    {
        return await Task.Run(() => GetMetadata(filePath));
    }

    public byte[]? GetAlbumArt(string filePath)
    {
        try
        {
            var file = TagLib.File.Create(filePath);
            return file.Tag.Pictures.FirstOrDefault()?.Data.Data;
        }
        catch
        {
            return null;
        }
    }
}
