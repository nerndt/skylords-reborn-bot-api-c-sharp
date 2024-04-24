using SkylordsRebornAPI.Cardbase;

namespace SkylordsRebornAPI
{
    public class Instances
    {
        public static CardService CardService { get; } = new();
        public static MapService MapService { get; } = new();
        public static AuctionService AuctionService { get; } = new();
        public static LeaderboardsService LeaderboardsService { get; } = new();
    }
}