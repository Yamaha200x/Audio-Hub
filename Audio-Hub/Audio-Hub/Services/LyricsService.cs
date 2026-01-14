using System.Net.Http.Json;
using System.Text.Json;
using Audio_Hub.Droid.Services;

namespace Audio_Hub.Droid.Services;

/// <summary>
/// LRCLIB integration - open source lyrics database.
/// No authentication required, no rate limits documented.
/// </summary>
public class LyricsService : ILyricsService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://lrclib.net/api";

    public LyricsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Lyrics?> GetLyricsAsync(string trackName, string artistName, string? albumName = null, int? duration = null)
    {
        try
        {
            var url = $"{BaseUrl}/get?track_name={Uri.EscapeDataString(trackName)}&artist_name={Uri.EscapeDataString(artistName)}";
            
            if (!string.IsNullOrEmpty(albumName))
                url += $"&album_name={Uri.EscapeDataString(albumName)}";
            
            if (duration.HasValue)
                url += $"&duration={duration.Value}";

            var response = await _httpClient.GetStringAsync(url);
            var json = JsonDocument.Parse(response);
            var root = json.RootElement;

            return new Lyrics
            {
                PlainLyrics = root.GetProperty("plainLyrics").GetString() ?? string.Empty,
                SyncedLyrics = root.TryGetProperty("syncedLyrics", out var synced) ? synced.GetString() : null,
                Language = root.TryGetProperty("lang", out var lang) ? lang.GetString() ?? "en" : "en",
                Duration = root.TryGetProperty("duration", out var dur) ? dur.GetInt32() : null
            };
        }
        catch (HttpRequestException ex)
        {
            // 404 = lyrics not found (normal), handle gracefully in UI
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<Lyrics?> GetLyricsByMetadataAsync(AudioMetadata metadata)
    {
        return await GetLyricsAsync(
            metadata.Title,
            metadata.Artist,
            metadata.Album,
            (int)metadata.Duration.TotalSeconds
        );
    }
}
