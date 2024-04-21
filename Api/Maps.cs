﻿using System.Text.Json.Serialization;

namespace Api
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Maps
    {
        NotAMap = 0,
        Turan = 6,
        Simai = 7,
        SiegeOfHope = 8,
        Crusade = 9,
        BadHarvest = 10,
        Danduil = 14,
        Haladur = 15,
        Fyre = 16,
        Elyon = 17,
        Sunbridge = 18,
        TheDwarvenRiddle = 19,
        TheTreasureFleet = 20,
        TheInsaneGod = 21,
        TheSoulTree = 22,
        NightmareShard = 24,
        TheGunsOfLyr = 25,
        KingOfTheGiants = 26,
        Wazhai = 27,
        Gorgash = 28,
        Titans = 29,
        Lajesh = 30,
        Yshia = 31,
        BehindEnemyLines = 32,
        Uro = 33,
        Nadai = 34,
        SlaveMaster = 35,
        Ascension = 37,
        NightmaresEnd = 44,
        EncountersWithTwilight = 45,
        Convoy = 56,
        DefendingHope = 57,
        PassageToDarkness = 60,
        Introduction = 67,
        Yrmia = 71,
        Blight = 74,
        Koshan = 78,
        Zahadune = 79,
        Ocean = 84,
        Mo = 88,
        Random1v1 = 96,
        Random2v2 = 97,
        RavensEnd = 98,
        Oracle = 99,
        Empire = 100,
        RPvEOnePlayer = 101,
        RPvETwoPlayers = 102,
        RPvEFourPlayers = 103,
        CommunityMap1v1 = 104,
        CommunityMap2v2 = 105,
        CommunityMap1P = 106,
        CommunityMap2P = 107,
        CommunityMap4P = 108,
        Random3V3 = 109,
        CommunityMap3v3 = 110,
        HaladurSpectator = 1001,
        SimaiSpectator = 1002,
        ElyonSpectator = 1003,
        WazhaiSpectator = 1004,
        LajeshSpectator = 1005,
        UroSpectator = 1006,
        YrmiaSpectator = 1007,
        KoshanSpectator = 1010,
        ZahaduneSpectator = 1011,
        TuranSpectator = 1012,
        FyreSpectator = 1013,
        DanduilSpectator = 1014,
        GorgashSpectator = 1015,
        YshiaSpectator = 1016,
        NadaiSpectator = 1017,
    }
}
