using System.Collections.Generic;
using Newtonsoft.Json;
using SkylordsRebornAPI.Cardbase.Shared;

namespace SkylordsRebornAPI.Cardbase
{
    public class Map
    {
        public string Name { get; set; }
        public string SubTitle { get; set; }
        public string Campaign { get; set; }
        public int Players { get; set; }
        public Edition Edition { get; set; }
        public string Description { get; set; }

        public List<string> Goals { get; set; }
        public List<string> Unlocks { get; set; }
        public List<string> Prerequisite { get; set; }

        public Media Image { get; set; }

        [JsonProperty("Map")] public Media MapInfo { get; set; }

        public List<string> Difficulties { get; set; } = new();
        // TODO add YoutubeVideo type once API works
        //public YoutubeVideo WalkThrough { get; set; }

        public List<Loot> Standard { get; set; }
        public List<Loot> Advanced { get; set; }
        public List<Loot> Expert { get; set; }

        public class Loot
        {
            public string CardName { get; set; }
            public int Era { get; set; }
        }
    }
}