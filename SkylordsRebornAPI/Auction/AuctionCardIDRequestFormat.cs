using Newtonsoft.Json;

namespace SkylordsRebornAPI.Auction
{
    public class AuctionCardIDRequestFormat
    {
        [JsonProperty("cardId")] public long CardId { get; set; }

        [JsonProperty("cardName")] public string CardName { get; set; }

        [JsonProperty("rarity")] public string Rarity { get; set; }

        [JsonProperty("expansion")] public string Expansion { get; set; }

        [JsonProperty("promo")] public string Promo { get; set; }

        [JsonProperty("obtainable")] public string Obtainable { get; set; }

        [JsonProperty("fireOrbs")] public long FireOrbs { get; set; }

        [JsonProperty("frostOrbs")] public long FrostOrbs { get; set; }

        [JsonProperty("natureOrbs")] public long NatureOrbs { get; set; }

        [JsonProperty("shadowOrbs")] public long ShadowOrbs { get; set; }

        [JsonProperty("neutralOrbs")] public long NeutralOrbs { get; set; }

        [JsonProperty("cardType")] public string CardType { get; set; }
    }
}