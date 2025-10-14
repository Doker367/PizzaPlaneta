
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManyBoxApi.Models
{
    public class DhlProofOfDeliveryResponse
    {
        [JsonPropertyName("documents")]
        public List<DhlPodDocument> Documents { get; set; }
    }

    public class DhlPodDocument
    {
        [JsonPropertyName("encodingFormat")]
        public string EncodingFormat { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("typeCode")]
        public string TypeCode { get; set; }
    }
}
