using System.Text.Json;

namespace Dac.Neo.Data.Configurations;

public static class JsonExtensions  //this even needed now?!? toReview**
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // overriden  by [JsonPropertyName] annotation....
    };

    public static T? FromJson<T>(this string json) =>
        JsonSerializer.Deserialize<T>(json, _jsonOptions);

    public static string ToJson<T>(this T obj) =>
        JsonSerializer.Serialize<T>(obj, _jsonOptions);
}
