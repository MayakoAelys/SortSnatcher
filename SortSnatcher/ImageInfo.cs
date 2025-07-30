using System.Text.Json.Serialization;

namespace SortSnatcher
{
    internal class ImageInfo
    {
        [JsonPropertyName("tags")]
        public string[] Tags { get; set; } = [];

        [JsonPropertyName("rating")]
        public required string Rating { get; set; }
    }
}
