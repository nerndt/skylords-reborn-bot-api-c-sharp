using Newtonsoft.Json;

namespace SkylordsRebornAPI.Leaderboards
{
    public class MatchInfo
    {
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("map")] public long Map { get; set; }

        [JsonProperty("time")] public long Time { get; set; }

        [JsonProperty("difficulty")] public long Difficulty { get; set; }

        [JsonProperty("experience")] public long Experience { get; set; }
    }

    public class PVPMatchInfo
    {
        [JsonProperty("players", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Players { get; set; }

        [JsonProperty("baseElo")] public long BaseElo { get; set; }

        [JsonProperty("rating")] public double Rating { get; set; }

        [JsonProperty("activity")] public double Activity { get; set; }

        [JsonProperty("wins", NullValueHandling = NullValueHandling.Ignore)]
        public long? Wins { get; set; }

        [JsonProperty("losses", NullValueHandling = NullValueHandling.Ignore)]
        public long? Losses { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("totalMatches", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalMatches { get; set; }

        [JsonProperty("winsLimited", NullValueHandling = NullValueHandling.Ignore)]
        public long? WinsLimited { get; set; }

        [JsonProperty("losesLimited", NullValueHandling = NullValueHandling.Ignore)]
        public long? LosesLimited { get; set; }
    }
}