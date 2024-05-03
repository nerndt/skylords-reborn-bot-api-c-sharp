using Api;

namespace Bots
{
    public class DeckOfficialCardIds
    {
        public string Name; // Name of the Deck
        public int[] Ids; // List of 20 Ofiicial Card Ids for this deck
    }

    public static class PvPCardDecks
    {
        #region PvP Decks

        #region PvP Shadow

        public static readonly Deck TaintedDarkness = new()
        {
            Name = "TaintedDarkness",
            CoverCardIndex = 0,
            Cards = new Api.CardId[20]
{
                CardIdCreator.New(Api.CardTemplate.Dreadcharger, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Forsaken, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.NoxTrooper, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Motivate, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.NastySurprise, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.LifeWeaving, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Executor, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DarkelfAssassins, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Nightcrawler, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.ShadowMage, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.KnightOfChaosAFrost, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.CorpseExplosion, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.NetherWarpAFrost, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Harvester, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.AuraofCorruption, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.CultistMaster, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.AshbonePyro, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SatanaelAFire, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.BloodHealing, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Voidstorm, Upgrade.U3),
}
        };

        public static readonly DeckOfficialCardIds TaintedDarknessCardIds = new()
        {
            Name = "TaintedDarkness",
            Ids = new int[20]
{
                (int)Api.CardTemplate.Dreadcharger,
                (int)Api.CardTemplate.Forsaken,
                (int)Api.CardTemplate.NoxTrooper,
                (int)Api.CardTemplate.Motivate,
                (int)Api.CardTemplate.NastySurprise,
                (int)Api.CardTemplate.LifeWeaving,
                (int)Api.CardTemplate.Executor,
                (int)Api.CardTemplate.DarkelfAssassins,
                (int)Api.CardTemplate.Nightcrawler,
                (int)Api.CardTemplate.ShadowMage,
                (int)Api.CardTemplate.KnightOfChaosAFrost,
                (int)Api.CardTemplate.CorpseExplosion,
                (int)Api.CardTemplate.NetherWarpAFrost,
                (int)Api.CardTemplate.Harvester,
                (int)Api.CardTemplate.AuraofCorruption,
                (int)Api.CardTemplate.CultistMaster,
                (int)Api.CardTemplate.AshbonePyro,
                (int)Api.CardTemplate.SatanaelAFire,
                (int)Api.CardTemplate.BloodHealing,
                (int)Api.CardTemplate.Voidstorm,
}
        };

        public static readonly Deck GiftedDarkness = new()
        {
            Name = "GiftedDarkness",
            CoverCardIndex = 0,
            Cards = new Api.CardId[20]
{
                CardIdCreator.New(Api.CardTemplate.Dreadcharger, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Forsaken, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.NoxTrooper, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Motivate, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.NastySurprise, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.LifeWeaving, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DarkelfAssassins, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Nightcrawler, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.AmiiPaladins, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.AmiiPhantom, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Burrower, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.ShadowPhoenix, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.AuraofCorruption, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Tranquility, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.EnsnaringRoots, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Hurricane, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.CurseofOink, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SurgeOfLight, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Brannoc, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.AshbonePyro, Upgrade.U3),
}
        };

        public static readonly DeckOfficialCardIds GiftedDarknessCardIds = new()
        {
            Name = "GiftedDarkness",
            Ids = new int[20]
{
                (int)Api.CardTemplate.Dreadcharger,
                (int)Api.CardTemplate.Forsaken,
                (int)Api.CardTemplate.NoxTrooper,
                (int)Api.CardTemplate.Motivate,
                (int)Api.CardTemplate.NastySurprise,
                (int)Api.CardTemplate.LifeWeaving,
                (int)Api.CardTemplate.DarkelfAssassins,
                (int)Api.CardTemplate.Nightcrawler,
                (int)Api.CardTemplate.AmiiPaladins,
                (int)Api.CardTemplate.AmiiPhantom,
                (int)Api.CardTemplate.Burrower,
                (int)Api.CardTemplate.ShadowPhoenix,
                (int)Api.CardTemplate.AuraofCorruption,
                (int)Api.CardTemplate.Tranquility,
                (int)Api.CardTemplate.EnsnaringRoots,
                (int)Api.CardTemplate.Hurricane,
                (int)Api.CardTemplate.CurseofOink,
                (int)Api.CardTemplate.SurgeOfLight,
                (int)Api.CardTemplate.Brannoc,
                (int)Api.CardTemplate.AshbonePyro,
}
        };

        public static readonly Deck BlessedDarkness = new()
        {
            Name = "BlessedDarkness",
            CoverCardIndex = 0,
            Cards = new Api.CardId[20]
{
                CardIdCreator.New(Api.CardTemplate.Dreadcharger, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Forsaken, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.NoxTrooper, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Motivate, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.NastySurprise, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.LifeWeaving, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.FrostBiteAShadow, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Executor, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.GlacierShell, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.KoboldTrick, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DarkelfAssassins, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Nightcrawler, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.StormsingerANature, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.AuraofCorruption, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.LostReaverAFire, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Coldsnap, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SilverwindLancers, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.AshbonePyro, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.TimelessOne, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.LostGrigoriAFire, Upgrade.U3),
}
        };

        public static readonly DeckOfficialCardIds BlessedDarknessCardIds = new()
        {
            Name = "BlessedDarkness",
            Ids = new int[20]
{
                (int)Api.CardTemplate.Dreadcharger,
                (int)Api.CardTemplate.Forsaken,
                (int)Api.CardTemplate.NoxTrooper,
                (int)Api.CardTemplate.Motivate,
                (int)Api.CardTemplate.NastySurprise,
                (int)Api.CardTemplate.LifeWeaving,
                (int)Api.CardTemplate.FrostBiteAShadow,
                (int)Api.CardTemplate.Executor,
                (int)Api.CardTemplate.GlacierShell,
                (int)Api.CardTemplate.KoboldTrick,
                (int)Api.CardTemplate.DarkelfAssassins,
                (int)Api.CardTemplate.Nightcrawler,
                (int)Api.CardTemplate.StormsingerANature,
                (int)Api.CardTemplate.AuraofCorruption,
                (int)Api.CardTemplate.LostReaverAFire,
                (int)Api.CardTemplate.Coldsnap,
                (int)Api.CardTemplate.SilverwindLancers,
                (int)Api.CardTemplate.AshbonePyro,
                (int)Api.CardTemplate.TimelessOne,
                (int)Api.CardTemplate.LostGrigoriAFire,
}
        };

        public static readonly Deck InfusedDarkness = new()
        {
            Name = "InfusedDarkness",
            CoverCardIndex = 0,
            Cards = new Api.CardId[20]
{
                CardIdCreator.New(Api.CardTemplate.Dreadcharger, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Forsaken, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.NoxTrooper, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Motivate, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.NastySurprise, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.LifeWeaving, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.BanditStalkerANature, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.BanditSniper, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.BanditSpearmenAFrost, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DarkelfAssassins, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.WindhunterAShadow, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.BanditMinefield, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.LavaField, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.RallyingBanner, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Eruption, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Ravage, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.AuraofCorruption, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DisenchantAShadow, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.BanditLancerANature, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SoulhunterAFrost, Upgrade.U3),
}
        };

        public static readonly DeckOfficialCardIds InfusedDarknessCardIds = new()
        {
            Name = "InfusedDarkness",
            Ids = new int[20]
{
                (int)Api.CardTemplate.Dreadcharger,
                (int)Api.CardTemplate.Forsaken,
                (int)Api.CardTemplate.NoxTrooper,
                (int)Api.CardTemplate.Motivate,
                (int)Api.CardTemplate.NastySurprise,
                (int)Api.CardTemplate.LifeWeaving,
                (int)Api.CardTemplate.BanditStalkerANature,
                (int)Api.CardTemplate.BanditSniper,
                (int)Api.CardTemplate.BanditSpearmenAFrost,
                (int)Api.CardTemplate.DarkelfAssassins,
                (int)Api.CardTemplate.WindhunterAShadow,
                (int)Api.CardTemplate.BanditMinefield,
                (int)Api.CardTemplate.LavaField,
                (int)Api.CardTemplate.RallyingBanner,
                (int)Api.CardTemplate.Eruption,
                (int)Api.CardTemplate.Ravage,
                (int)Api.CardTemplate.AuraofCorruption,
                (int)Api.CardTemplate.DisenchantAShadow,
                (int)Api.CardTemplate.BanditLancerANature,
                (int)Api.CardTemplate.SoulhunterAFrost,
}
        };

        #endregion PvP Shadow

        #region PvP Nature

        public static readonly Deck TaintedFlora = new()
        {
            Name = "TaintedFlora",
            CoverCardIndex = 0,
            Cards = new Api.CardId[20]
{
                CardIdCreator.New(Api.CardTemplate.Swiftclaw, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DryadAFrost, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Windweavers, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Shaman, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.PrimalDefender, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.EnsnaringRoots, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Hurricane, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SurgeOfLight, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.NastySurprise, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DarkelfAssassins, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Nightcrawler, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.AmiiPaladins, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.AmiiPhantom, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Burrower, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.ShadowPhoenix, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.AuraofCorruption, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Tranquility, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.CurseofOink, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.CultistMaster, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.AshbonePyro, Upgrade.U3),
}
        };

        public static readonly DeckOfficialCardIds TaintedFloraCardIds = new()
        {
            Name = "TaintedFlora",
            Ids = new int[20]
{
                (int)Api.CardTemplate.Swiftclaw,
                (int)Api.CardTemplate.DryadAFrost,
                (int)Api.CardTemplate.Windweavers,
                (int)Api.CardTemplate.Shaman,
                (int)Api.CardTemplate.PrimalDefender,
                (int)Api.CardTemplate.EnsnaringRoots,
                (int)Api.CardTemplate.Hurricane,
                (int)Api.CardTemplate.SurgeOfLight,
                (int)Api.CardTemplate.NastySurprise,
                (int)Api.CardTemplate.DarkelfAssassins,
                (int)Api.CardTemplate.Nightcrawler,
                (int)Api.CardTemplate.AmiiPaladins,
                (int)Api.CardTemplate.AmiiPhantom,
                (int)Api.CardTemplate.Burrower,
                (int)Api.CardTemplate.ShadowPhoenix,
                (int)Api.CardTemplate.AuraofCorruption,
                (int)Api.CardTemplate.Tranquility,
                (int)Api.CardTemplate.CurseofOink,
                (int)Api.CardTemplate.CultistMaster,
                (int)Api.CardTemplate.AshbonePyro,
}
        };

        public static readonly Deck GiftedFlora = new()
        {
            Name = "GiftedFlora",
            CoverCardIndex = 0,
            Cards = new Api.CardId[20]
{
                CardIdCreator.New(Api.CardTemplate.Swiftclaw, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DryadAFrost, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Windweavers, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Shaman, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Spearmen, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.EnsnaringRoots, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Hurricane, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SurgeOfLight, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Burrower, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Ghostspears, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SpiritHuntersANature, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DeepOneANature, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.EnergyParasite, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.CurseofOink, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.CreepingParalysis, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Parasite, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.ParasiteSwarm, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Thunderstorm, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.FathomLord, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Mo, Upgrade.U3),
}
        };

        public static readonly DeckOfficialCardIds GiftedFloraCardIds = new()
        {
            Name = "GiftedFlora",
            Ids = new int[20]
{
                (int)Api.CardTemplate.Swiftclaw,
                (int)Api.CardTemplate.DryadAFrost,
                (int)Api.CardTemplate.Windweavers,
                (int)Api.CardTemplate.Shaman,
                (int)Api.CardTemplate.Spearmen,
                (int)Api.CardTemplate.EnsnaringRoots,
                (int)Api.CardTemplate.Hurricane,
                (int)Api.CardTemplate.SurgeOfLight,
                (int)Api.CardTemplate.Burrower,
                (int)Api.CardTemplate.Ghostspears,
                (int)Api.CardTemplate.SpiritHuntersANature,
                (int)Api.CardTemplate.DeepOneANature,
                (int)Api.CardTemplate.EnergyParasite,
                (int)Api.CardTemplate.CurseofOink,
                (int)Api.CardTemplate.CreepingParalysis,
                (int)Api.CardTemplate.Parasite,
                (int)Api.CardTemplate.ParasiteSwarm,
                (int)Api.CardTemplate.Thunderstorm,
                (int)Api.CardTemplate.FathomLord,
                (int)Api.CardTemplate.Mo,
}
        };

        public static readonly Deck BlessedFlora = new()
        {
            Name = "BlessedFlora",
            CoverCardIndex = 0,
            Cards = new Api.CardId[20]
{
                CardIdCreator.New(Api.CardTemplate.Swiftclaw, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DryadAFrost, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Windweavers, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Shaman, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Spearmen, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.EnsnaringRoots, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Hurricane, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SurgeOfLight, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.CurseofOink, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.FrostBiteAShadow, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.GlacierShell, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.KoboldTrick, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.StoneShardsAFire, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.StormsingerANature, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Burrower, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SpiritHuntersANature, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.RazorshardAFire, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SilverwindLancers, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.StoneWarriorAFrost, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.TimelessOne, Upgrade.U3),
}
        };

        public static readonly DeckOfficialCardIds BlessedFloraCardIds = new()
        {
            Name = "BlessedFlora",
            Ids = new int[20]
{
                (int)Api.CardTemplate.Swiftclaw,
                (int)Api.CardTemplate.DryadAFrost,
                (int)Api.CardTemplate.Windweavers,
                (int)Api.CardTemplate.Shaman,
                (int)Api.CardTemplate.Spearmen,
                (int)Api.CardTemplate.EnsnaringRoots,
                (int)Api.CardTemplate.Hurricane,
                (int)Api.CardTemplate.SurgeOfLight,
                (int)Api.CardTemplate.CurseofOink,
                (int)Api.CardTemplate.FrostBiteAShadow,
                (int)Api.CardTemplate.GlacierShell,
                (int)Api.CardTemplate.KoboldTrick,
                (int)Api.CardTemplate.StoneShardsAFire,
                (int)Api.CardTemplate.StormsingerANature,
                (int)Api.CardTemplate.Burrower,
                (int)Api.CardTemplate.SpiritHuntersANature,
                (int)Api.CardTemplate.RazorshardAFire,
                (int)Api.CardTemplate.SilverwindLancers,
                (int)Api.CardTemplate.StoneWarriorAFrost,
                (int)Api.CardTemplate.TimelessOne,
}
        };

        public static readonly Deck InfusedFlora = new()
        {
            Name = "InfusedFlora",
            CoverCardIndex = 0,
            Cards = new Api.CardId[20]
{
                CardIdCreator.New(Api.CardTemplate.Swiftclaw, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DryadAFrost, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Windweavers, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Shaman, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Spearmen, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.EnsnaringRoots, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Hurricane, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SurgeOfLight, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Eruption, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.TwilightMinionsANature, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Burrower, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.CurseofOink, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.GladiatrixANature, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.TwilightCrawlers, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.VilebloodAFire, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SkyfireDrake, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Ravage, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.LavaField, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.MutatingManiacAFire, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.NightshadePlantANature, Upgrade.U3),
}
        };

        public static readonly DeckOfficialCardIds InfusedFloraCardIds = new()
        {
            Name = "InfusedFlora",
            Ids = new int[20]
{
                (int)Api.CardTemplate.Swiftclaw,
                (int)Api.CardTemplate.DryadAFrost,
                (int)Api.CardTemplate.Windweavers,
                (int)Api.CardTemplate.Shaman,
                (int)Api.CardTemplate.Spearmen,
                (int)Api.CardTemplate.EnsnaringRoots,
                (int)Api.CardTemplate.Hurricane,
                (int)Api.CardTemplate.SurgeOfLight,
                (int)Api.CardTemplate.Eruption,
                (int)Api.CardTemplate.TwilightMinionsANature,
                (int)Api.CardTemplate.Burrower,
                (int)Api.CardTemplate.CurseofOink,
                (int)Api.CardTemplate.GladiatrixANature,
                (int)Api.CardTemplate.TwilightCrawlers,
                (int)Api.CardTemplate.VilebloodAFire,
                (int)Api.CardTemplate.SkyfireDrake,
                (int)Api.CardTemplate.Ravage,
                (int)Api.CardTemplate.LavaField,
                (int)Api.CardTemplate.MutatingManiacAFire,
                (int)Api.CardTemplate.NightshadePlantANature,
}
        };

        #endregion PvP Nature

        #region PvP Frost

        public static readonly Deck TaintedIce = new()
        {
            Name = "TaintedIce",
            CoverCardIndex = 0,
            Cards = new Api.CardId[20]
{
                CardIdCreator.New(Api.CardTemplate.MasterArchers, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.IceGuardian, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.FrostMage, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.FrostBiteAShadow, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.IceBarrier, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.HomeSoil, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.GlacierShell, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.KoboldTrick, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DarkelfAssassins, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Nightcrawler, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.StormsingerANature, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.LostReaverAFire, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.NastySurprise, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.LifeWeaving, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Coldsnap, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.AuraofCorruption, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.TimelessOne, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SilverwindLancers, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.AshbonePyro, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.LostGrigoriAFire, Upgrade.U3),
}
        };

        public static readonly DeckOfficialCardIds TaintedIceCardIds = new()
        {
            Name = "TaintedIce",
            Ids = new int[20]
{
                (int)Api.CardTemplate.MasterArchers,
                (int)Api.CardTemplate.IceGuardian,
                (int)Api.CardTemplate.FrostMage,
                (int)Api.CardTemplate.FrostBiteAShadow,
                (int)Api.CardTemplate.IceBarrier,
                (int)Api.CardTemplate.HomeSoil,
                (int)Api.CardTemplate.GlacierShell,
                (int)Api.CardTemplate.KoboldTrick,
                (int)Api.CardTemplate.DarkelfAssassins,
                (int)Api.CardTemplate.Nightcrawler,
                (int)Api.CardTemplate.StormsingerANature,
                (int)Api.CardTemplate.LostReaverAFire,
                (int)Api.CardTemplate.NastySurprise,
                (int)Api.CardTemplate.LifeWeaving,
                (int)Api.CardTemplate.Coldsnap,
                (int)Api.CardTemplate.AuraofCorruption,
                (int)Api.CardTemplate.TimelessOne,
                (int)Api.CardTemplate.SilverwindLancers,
                (int)Api.CardTemplate.AshbonePyro,
                (int)Api.CardTemplate.LostGrigoriAFire,
}
        };

        public static readonly Deck GiftedIce = new()
        {
            Name = "GiftedIce",
            CoverCardIndex = 0,
            Cards = new Api.CardId[20]
{
                CardIdCreator.New(Api.CardTemplate.MasterArchers, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.IceGuardian, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.FrostMage, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.FrostBiteAShadow, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.IceBarrier, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.HomeSoil, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.LightbladeAShadow, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.GlacierShell, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.KoboldTrick, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.EnsnaringRoots, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Hurricane, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SurgeOfLight, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.CurseofOink, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Burrower, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.RazorshardAFire, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.StoneShardsAFire, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.StormsingerANature, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SpiritHuntersANature, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.CrystalFiendANature, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Mountaineer, Upgrade.U3),
}
        };

        public static readonly DeckOfficialCardIds GiftedIceCardIds = new()
        {
            Name = "GiftedIce",
            Ids = new int[20]
{
                (int)Api.CardTemplate.MasterArchers,
                (int)Api.CardTemplate.IceGuardian,
                (int)Api.CardTemplate.FrostMage,
                (int)Api.CardTemplate.FrostBiteAShadow,
                (int)Api.CardTemplate.IceBarrier,
                (int)Api.CardTemplate.HomeSoil,
                (int)Api.CardTemplate.LightbladeAShadow,
                (int)Api.CardTemplate.GlacierShell,
                (int)Api.CardTemplate.KoboldTrick,
                (int)Api.CardTemplate.EnsnaringRoots,
                (int)Api.CardTemplate.Hurricane,
                (int)Api.CardTemplate.SurgeOfLight,
                (int)Api.CardTemplate.CurseofOink,
                (int)Api.CardTemplate.Burrower,
                (int)Api.CardTemplate.RazorshardAFire,
                (int)Api.CardTemplate.StoneShardsAFire,
                (int)Api.CardTemplate.StormsingerANature,
                (int)Api.CardTemplate.SpiritHuntersANature,
                (int)Api.CardTemplate.CrystalFiendANature,
                (int)Api.CardTemplate.Mountaineer,
}
        };

        public static readonly Deck BlessedIce = new()
        {
            Name = "BlessedIce",
            CoverCardIndex = 0,
            Cards = new Api.CardId[20]
{
                CardIdCreator.New(Api.CardTemplate.MasterArchers, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.IceGuardian, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.FrostMage, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.FrostBiteAShadow, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.IceBarrier, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.HomeSoil, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.GlacierShell, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.KoboldTrick, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.IcefangRaptorAFrost, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.MountainRowdyAShadow, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.StormsingerANature, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.WhiteRangers, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.WarEagle, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Coldsnap, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.AreaIceShield, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.TimelessOne, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.TimelessOne, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SilverwindLancers, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Tremor, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.ShieldBuilding, Upgrade.U3),
}
        };

        public static readonly DeckOfficialCardIds BlessedIceCardIds = new()
        {
            Name = "BlessedIce",
            Ids = new int[20]
{
                (int)Api.CardTemplate.MasterArchers,
                (int)Api.CardTemplate.IceGuardian,
                (int)Api.CardTemplate.FrostMage,
                (int)Api.CardTemplate.FrostBiteAShadow,
                (int)Api.CardTemplate.IceBarrier,
                (int)Api.CardTemplate.HomeSoil,
                (int)Api.CardTemplate.GlacierShell,
                (int)Api.CardTemplate.KoboldTrick,
                (int)Api.CardTemplate.IcefangRaptorAFrost,
                (int)Api.CardTemplate.MountainRowdyAShadow,
                (int)Api.CardTemplate.StormsingerANature,
                (int)Api.CardTemplate.WhiteRangers,
                (int)Api.CardTemplate.WarEagle,
                (int)Api.CardTemplate.Coldsnap,
                (int)Api.CardTemplate.AreaIceShield,
                (int)Api.CardTemplate.TimelessOne,
                (int)Api.CardTemplate.TimelessOne,
                (int)Api.CardTemplate.SilverwindLancers,
                (int)Api.CardTemplate.Tremor,
                (int)Api.CardTemplate.ShieldBuilding,
}
        };

        public static readonly Deck InfusedIce = new()
        {
            Name = "InfusedIce",
            CoverCardIndex = 0,
            Cards = new Api.CardId[20]
{
                CardIdCreator.New(Api.CardTemplate.MasterArchers, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.IceGuardian, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.FrostMage, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.FrostBiteAShadow, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.IceBarrier, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.HomeSoil, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.GlacierShell, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.KoboldTrick, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.StormsingerANature, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.IcefangRaptorAFrost, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.WarlockAFrost, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Eruption, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SkyfireDrake, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Ravage, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Coldsnap, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.LavaField, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SilverwindLancers, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.TimelessOne, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.CoreDredgeAFrost, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.LostGrigoriAFire, Upgrade.U3),
}
        };

        public static readonly DeckOfficialCardIds InfusedIceCardIds = new()
        {
            Name = "InfusedIce",
            Ids = new int[20]
{
                (int)Api.CardTemplate.MasterArchers,
                (int)Api.CardTemplate.IceGuardian,
                (int)Api.CardTemplate.FrostMage,
                (int)Api.CardTemplate.FrostBiteAShadow,
                (int)Api.CardTemplate.IceBarrier,
                (int)Api.CardTemplate.HomeSoil,
                (int)Api.CardTemplate.GlacierShell,
                (int)Api.CardTemplate.KoboldTrick,
                (int)Api.CardTemplate.StormsingerANature,
                (int)Api.CardTemplate.IcefangRaptorAFrost,
                (int)Api.CardTemplate.WarlockAFrost,
                (int)Api.CardTemplate.Eruption,
                (int)Api.CardTemplate.SkyfireDrake,
                (int)Api.CardTemplate.Ravage,
                (int)Api.CardTemplate.Coldsnap,
                (int)Api.CardTemplate.LavaField,
                (int)Api.CardTemplate.SilverwindLancers,
                (int)Api.CardTemplate.TimelessOne,
                (int)Api.CardTemplate.CoreDredgeAFrost,
                (int)Api.CardTemplate.LostGrigoriAFire,
}
        };

        #endregion PvP Frost

        #region PvP Fire

        public static readonly Deck TaintedFlame = new()
        {
            Name = "TaintedFlame",
            CoverCardIndex = 0,
            Cards = new Api.CardId[20]
{
                CardIdCreator.New(Api.CardTemplate.Scavenger, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Sunstriders, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Thugs, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Eruption, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.FireswornAFrost, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Sunderer, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.LifeWeaving, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DarkelfAssassins, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.BanditStalkerANature, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.BanditSniper, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.BanditSpearmenAFrost, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.RallyingBanner, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.WindhunterAShadow, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Ravage, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.BanditMinefield, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DisenchantAShadow, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.LavaField, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.AuraofCorruption, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.BanditLancerANature, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SoulhunterAFrost, Upgrade.U3),
}
        };

        public static readonly DeckOfficialCardIds TaintedFlameCardIds = new()
        {
            Name = "TaintedFlame",
            Ids = new int[20]
{
                (int)Api.CardTemplate.Scavenger,
                (int)Api.CardTemplate.Sunstriders,
                (int)Api.CardTemplate.Thugs,
                (int)Api.CardTemplate.Eruption,
                (int)Api.CardTemplate.FireswornAFrost,
                (int)Api.CardTemplate.Sunderer,
                (int)Api.CardTemplate.LifeWeaving,
                (int)Api.CardTemplate.DarkelfAssassins,
                (int)Api.CardTemplate.BanditStalkerANature,
                (int)Api.CardTemplate.BanditSniper,
                (int)Api.CardTemplate.BanditSpearmenAFrost,
                (int)Api.CardTemplate.RallyingBanner,
                (int)Api.CardTemplate.WindhunterAShadow,
                (int)Api.CardTemplate.Ravage,
                (int)Api.CardTemplate.BanditMinefield,
                (int)Api.CardTemplate.DisenchantAShadow,
                (int)Api.CardTemplate.LavaField,
                (int)Api.CardTemplate.AuraofCorruption,
                (int)Api.CardTemplate.BanditLancerANature,
                (int)Api.CardTemplate.SoulhunterAFrost,
}
        };

        public static readonly Deck GiftedFlame = new()
        {
            Name = "GiftedFlame",
            CoverCardIndex = 0,
            Cards = new Api.CardId[20]
{
                CardIdCreator.New(Api.CardTemplate.Scavenger, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Sunstriders, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Thugs, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Eruption, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.FireswornAFrost, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.TwilightMinionsANature, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Burrower, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.TwilightCrawlers, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SkyfireDrake, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.VilebloodAFire, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.GladiatrixANature, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.EnsnaringRoots, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Hurricane, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SurgeOfLight, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.CurseofOink, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DisenchantAShadow, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Ravage, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.LavaField, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.MutatingManiacAFire, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.NightshadePlantAShadow, Upgrade.U3),
}
        };

        public static readonly DeckOfficialCardIds GiftedFlameCardIds = new()
        {
            Name = "GiftedFlame",
            Ids = new int[20]
    {
                (int)Api.CardTemplate.Scavenger,
                (int)Api.CardTemplate.Sunstriders,
                (int)Api.CardTemplate.Thugs,
                (int)Api.CardTemplate.Eruption,
                (int)Api.CardTemplate.FireswornAFrost,
                (int)Api.CardTemplate.TwilightMinionsANature,
                (int)Api.CardTemplate.Burrower,
                (int)Api.CardTemplate.TwilightCrawlers,
                (int)Api.CardTemplate.SkyfireDrake,
                (int)Api.CardTemplate.VilebloodAFire,
                (int)Api.CardTemplate.GladiatrixANature,
                (int)Api.CardTemplate.EnsnaringRoots,
                (int)Api.CardTemplate.Hurricane,
                (int)Api.CardTemplate.SurgeOfLight,
                (int)Api.CardTemplate.CurseofOink,
                (int)Api.CardTemplate.DisenchantAShadow,
                (int)Api.CardTemplate.Ravage,
                (int)Api.CardTemplate.LavaField,
                (int)Api.CardTemplate.MutatingManiacAFire,
                (int)Api.CardTemplate.NightshadePlantAShadow,
    }
        };

        public static readonly Deck BlessedFlame = new()
        {
            Name = "BlessedFlame",
            CoverCardIndex = 0,
            Cards = new Api.CardId[20]
{
                CardIdCreator.New(Api.CardTemplate.Scavenger, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Sunstriders, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Wrecker, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Eruption, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.FireswornAFrost, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Sunderer, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.FrostSorceress, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SkyfireDrake, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Ravage, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.WarlockAFrost, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.StormsingerANature, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.IcefangRaptorAFrost, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.KoboldTrick, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.FrostBiteAShadow, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Coldsnap, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.LavaField, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.ShieldBuilding, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SilverwindLancers, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.TimelessOne, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.CoreDredgeAFrost, Upgrade.U3),
}
        };

        public static readonly DeckOfficialCardIds BlessedFlameCardIds = new()
        {
            Name = "TaintedFlame",
            Ids = new int[20]
{
                (int)Api.CardTemplate.Scavenger,
                (int)Api.CardTemplate.Sunstriders,
                (int)Api.CardTemplate.Wrecker,
                (int)Api.CardTemplate.Eruption,
                (int)Api.CardTemplate.FireswornAFrost,
                (int)Api.CardTemplate.Sunderer,
                (int)Api.CardTemplate.FrostSorceress,
                (int)Api.CardTemplate.SkyfireDrake,
                (int)Api.CardTemplate.Ravage,
                (int)Api.CardTemplate.WarlockAFrost,
                (int)Api.CardTemplate.StormsingerANature,
                (int)Api.CardTemplate.IcefangRaptorAFrost,
                (int)Api.CardTemplate.KoboldTrick,
                (int)Api.CardTemplate.FrostBiteAShadow,
                (int)Api.CardTemplate.Coldsnap,
                (int)Api.CardTemplate.LavaField,
                (int)Api.CardTemplate.ShieldBuilding,
                (int)Api.CardTemplate.SilverwindLancers,
                (int)Api.CardTemplate.TimelessOne,
                (int)Api.CardTemplate.CoreDredgeAFrost,
}
        };

        public static readonly Deck InfusedFlame = new()
        {
            Name = "InfusedFlame",
            CoverCardIndex = 0,
            Cards = new Api.CardId[20]
{
                CardIdCreator.New(Api.CardTemplate.Scavenger, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Sunstriders, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Thugs, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Eruption, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.FireswornAFrost, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Sunderer, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.RallyingBanner, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.BurningSpears, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Enforcer, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.GladiatrixANature, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Firedancer, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.ScytheFiends, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SkyfireDrake, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Ravage, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Wildfire, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.LavaField, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DisenchantANature, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.GiantSlayer, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Juggernaut, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Backlash, Upgrade.U3),
}
        };

        public static readonly DeckOfficialCardIds InfusedFlameCardIds = new()
        {
            Name = "InfusedFlame",
            Ids = new int[20]
{
                (int)Api.CardTemplate.Scavenger,
                (int)Api.CardTemplate.Sunstriders,
                (int)Api.CardTemplate.Thugs,
                (int)Api.CardTemplate.Eruption,
                (int)Api.CardTemplate.FireswornAFrost,
                (int)Api.CardTemplate.Sunderer,
                (int)Api.CardTemplate.RallyingBanner,
                (int)Api.CardTemplate.BurningSpears,
                (int)Api.CardTemplate.Enforcer,
                (int)Api.CardTemplate.GladiatrixANature,
                (int)Api.CardTemplate.Firedancer,
                (int)Api.CardTemplate.ScytheFiends,
                (int)Api.CardTemplate.SkyfireDrake,
                (int)Api.CardTemplate.Ravage,
                (int)Api.CardTemplate.Wildfire,
                (int)Api.CardTemplate.LavaField,
                (int)Api.CardTemplate.DisenchantANature,
                (int)Api.CardTemplate.GiantSlayer,
                (int)Api.CardTemplate.Juggernaut,
                (int)Api.CardTemplate.Backlash,
}
        };

        #endregion PvP Fire

        #endregion PvP Decks

    }
}
