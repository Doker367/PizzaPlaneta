using System.Text.Json.Serialization;

namespace ManyBox.Models.Dhl
{
    // Modelo único para dimensiones, igual que el backend
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
