using Newtonsoft.Json;

namespace SkylordsRebornAPI.Auction
{
    public class AuctionEntry
    {
        [JsonProperty("auctionId")] public long AuctionId { get; set; }

        [JsonProperty("cardName")] public string CardName { get; set; }

        [JsonProperty("cardId")] public long CardId { get; set; }

        [JsonProperty("currentPrice")] public long CurrentPrice { get; set; }

        [JsonProperty("startingPrice")] public long StartingPrice { get; set; }

        [JsonProperty("buyoutPrice")] public long BuyoutPrice { get; set; }

        [JsonProperty("endingOn")] public string EndingOn { get; set; }
    }
}