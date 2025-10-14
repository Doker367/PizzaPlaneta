using System.Text.Json.Serialization;

namespace ManyBoxApi.Models
{
    public class DhlDimensionsDecimal
    {
        [JsonPropertyName("length")]
        public decimal Length { get; set; }
        [JsonPropertyName("width")]
        public decimal Width { get; set; }
        [JsonPropertyName("height")]
        public decimal Height { get; set; }
    }
}
