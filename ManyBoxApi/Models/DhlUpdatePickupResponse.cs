
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManyBoxApi.Models
{
    public class DhlUpdatePickupResponse
    {
        [JsonPropertyName("dispatchConfirmationNumber")]
        public string DispatchConfirmationNumber { get; set; }

        [JsonPropertyName("readyByTime")]
        public string ReadyByTime { get; set; }

        [JsonPropertyName("nextPickupDate")]
        public string NextPickupDate { get; set; }

        [JsonPropertyName("warnings")]
        public List<string>? Warnings { get; set; }
    }
}
