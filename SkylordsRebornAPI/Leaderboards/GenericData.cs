using Newtonsoft.Json;

namespace SkylordsRebornAPI.Leaderboards
{
    public class GenericData
    {
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("value")] public long Value { get; set; }
    }
}