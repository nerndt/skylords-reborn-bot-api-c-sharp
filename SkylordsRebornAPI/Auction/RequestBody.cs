using Newtonsoft.Json;

namespace SkylordsRebornAPI.Auction
{
    public class RequestBody
    {
        [JsonProperty("input")] public string Input { get; set; }

        [JsonProperty("min")] public long Min { get; set; }

        [JsonProperty("max")] public long Max { get; set; }
    }
}