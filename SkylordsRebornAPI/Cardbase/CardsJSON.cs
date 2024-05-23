// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
// Entered "https://smj.cards/api/cards" in a web browser
// copied the contents into https://json2csharp.com/
// Copied the class output to here

using Newtonsoft.Json;
using SkylordsRebornAPI.Cardbase.Cards;
using System;
using System.Collections.Generic;

namespace SkylordsRebornAPI.Cardbase
{
    public class Ability
    {
        public string abilityIdentifier { get; set; }
        public string abilityName { get; set; }
        public int abilityType { get; set; }
        public int abilityAffinity { get; set; }
        public List<bool> abilityAvailability { get; set; }
        public bool abilityHidden { get; set; }
        public List<int> abilityStars { get; set; }
        public string abilityDescription { get; set; }
        public List<List<string>> abilityDescriptionValues { get; set; }
        public List<int> abilityCost { get; set; }
    }

    public class AbilityAffinity
    {
        [JsonProperty("0")]
        public string _0 { get; set; }

        [JsonProperty("1")]
        public string _1 { get; set; }

        [JsonProperty("2")]
        public string _2 { get; set; }

        [JsonProperty("3")]
        public string _3 { get; set; }

        [JsonProperty("-1")]
        public string _minus1 { get; set; }
    }

    public class AbilityType
    {
        [JsonProperty("0")]
        public string _0 { get; set; }

        [JsonProperty("1")]
        public string _1 { get; set; }

        [JsonProperty("2")]
        public string _2 { get; set; }

        [JsonProperty("3")]
        public string _3 { get; set; }

        [JsonProperty("4")]
        public string _4 { get; set; }

        [JsonProperty("-1")]
        public string _minus1 { get; set; }
    }

    public class Affinity
    {
        [JsonProperty("0")]
        public string _0 { get; set; }

        [JsonProperty("1")]
        public string _1 { get; set; }

        [JsonProperty("2")]
        public string _2 { get; set; }

        [JsonProperty("3")]
        public string _3 { get; set; }

        [JsonProperty("-1")]
        public string _minus1 { get; set; }
    }

    public class AttackType
    {
        [JsonProperty("0")]
        public string _0 { get; set; }

        [JsonProperty("1")]
        public string _1 { get; set; }

        [JsonProperty("-1")]
        public string _minus1 { get; set; }
    }

    public class Boosters
    {
        [JsonProperty("0")]
        public string _0 { get; set; }

        [JsonProperty("1")]
        public string _1 { get; set; }

        [JsonProperty("2")]
        public string _2 { get; set; }

        [JsonProperty("3")]
        public string _3 { get; set; }

        [JsonProperty("4")]
        public string _4 { get; set; }

        [JsonProperty("5")]
        public string _5 { get; set; }

        [JsonProperty("6")]
        public string _6 { get; set; }

        [JsonProperty("7")]
        public string _7 { get; set; }

        [JsonProperty("8")]
        public string _8 { get; set; }

        [JsonProperty("9")]
        public string _9 { get; set; }

        [JsonProperty("-3")]
        public string _minus3 { get; set; }

        [JsonProperty("-2")]
        public string _minus2 { get; set; }

        [JsonProperty("-1")]
        public string _minus1 { get; set; }
    }

    public class BuildingClass
    {
        [JsonProperty("~")]
        public string building { get; set; }

    }

    public class Color
    {
        [JsonProperty("0")]
        public string _0 { get; set; }

        [JsonProperty("1")]
        public string _1 { get; set; }

        [JsonProperty("2")]
        public string _2 { get; set; }

        [JsonProperty("3")]
        public string _3 { get; set; }

        [JsonProperty("4")]
        public string _4 { get; set; }

        [JsonProperty("5")]
        public string _5 { get; set; }

        [JsonProperty("6")]
        public string _6 { get; set; }

        [JsonProperty("7")]
        public string _7 { get; set; }

        [JsonProperty("8")]
        public string _8 { get; set; }

        [JsonProperty("9")]
        public string _9 { get; set; }

        [JsonProperty("-1")]
        public string _minus1 { get; set; }
    }

    public class SMJCard
    {
        public string _id { get; set; }
        public string cardSlug { get; set; }
        public string cardName { get; set; }
        public string cardNameSimple { get; set; }
        public string cardNameImage { get; set; }
        public List<int> officialCardIds { get; set; } // First position is one used to Import Card in game
        public string description { get; set; }
        public int edition { get; set; } // 0 Twilight, 1 Renegade, 2 Lost Souls, 3 Amii, 4 Rebirth
        public int color { get; set; } // 0 Neutral, Fire, Shadow, Nature, Frost, Bandits, Stonekin, Twilight, Lost Souls, Amii
        public int promo { get; set; } // 0 No, 1 Yes
        public int rarity { get; set; } // COmmon, Uncommon, Rare, Ultra Rare
        public int orbsTotal { get; set; } // 1, 2, 3, 4
        public int orbsNeutral { get; set; } // 0, 1, 2, 3, 4
        public int orbsFire { get; set; } // 0, 1, 2, 3, 4
        public int orbsShadow { get; set; } // 0, 1, 2, 3, 4
        public int orbsNature { get; set; } // 0, 1, 2, 3, 4
        public int orbsFrost { get; set; } // 0, 1, 2, 3, 4
        public int orbsFireShadow { get; set; } // 0, 1, 2, 3, 4
        public int orbsNatureFrost { get; set; } // 0, 1, 2, 3, 4
        public int orbsFireNature { get; set; } // 0, 1, 2, 3, 4
        public int orbsShadowFrost { get; set; } // 0, 1, 2, 3, 4
        public int orbsShadowNature { get; set; } // 0, 1, 2, 3, 4
        public int orbsFireFrost { get; set; } // 0, 1, 2, 3, 4
        public int affinity { get; set; } // None, Fire, Nature, Shadow, Frost
        public int type { get; set; } // 0 Unit, 1 Building, 2 Spell
        public string unitModel { get; set; } // Non-Units, Amazon, Atrocity, Balrog, Behemoth, Bird, Bow, Canine, Claw, Dancer, Dragon, Fighter, Floater, Guardian, Head, Insect, Mage, Minion, Quadraped, Raptor, Rider, Rifle, Roughneck, Ruffian, Serpent, Ship, Skyelf, Spear, Titan, Wagon, Worm, Unique
        public string category { get; set; }
        public string unitSpecies { get; set; } // Non-Units, Amii, Ancient, Artifact, Beast, Demon, Dragonkin, Elemental, Elf, Forestkin, Giant, Human, Kobold, Ogre, Orc, Primordial, Spirit, Undead, Special
        public string unitClass { get; set; } // Non-Units, Archer, Commander, Corrupter, Crusader, Destroyer, Dominator, Gladiatore, Marauder, Soldier, Supporter, Wizard, Special
        public string buildingClass { get; set; } // Non-Buildings, Artillary, Barrier, Device, Fortress, Hut, Shrine, Statue, Tower
        public string spellClass { get; set; } // Non-Spells, Arcane, Enchantment, Spell
        public int gender { get; set; } // Non-Units, Unspecified, Male, Female
        public int movementType { get; set; } // Non-Units, Ground, Flying
        public int attackType { get; set; } // Non-Units, Melee, Ranged
        public int offenseType { get; set; } // Non-Units, Small, Medium, Large, Extra Large, Special
        public int defenseType { get; set; } // Non-Units, Small, Medium, Large, Extra Large
        public int maxCharges { get; set; } // 4, 8, 12, 16, 20, 24
        public int squadSize { get; set; } // Spells, Buildings, 1, 2, 4, 6
        public int starterCard { get; set; } // Non-Starter, Starter
        public List<int> powerCost { get; set; }
        public List<int> damage { get; set; }
        public List<int> health { get; set; }
        public List<int> boosters { get; set; } // None, Mini, General, Fire, Shadow, Nature, Frost, Bandits, Stonekin, Twilight, Lost Souls, Amii, Fire/Frost
        public List<string> upgradeMaps { get; set; } // None, Encounter With Twilight, Siege of Hope, Defending Hope, The Soultree, The Treasure Fleet, Behind Enemy Lines, Mo, Ocean, Oracle, Crusade, Sunbridge, Nightmare Shard, Nightmare's End, The Insane God, Slave master, Convoy, Bad Harvest, King of the Giants, Titan, The Dwarven Riddle, The guns of Lyr, Blight, Raven's End, Empire,  
        public List<Ability> abilities { get; set; }
    }

    // List<SkylordsRebornCard> myDeserializedClass = JsonConvert.DeserializeObject<List<SkylordsRebornCard>>(myJsonResponse);
    public class SkylordsRebornCard
    {
        /*
        Notes: From Maze on Discord application. If you need a tiny bit of information, I also created a random deck generator a while back: https://smj.cards/deck?random=true

       The correct official api docs can be found here: https://hub.backend.skylords.eu/api/docs/

       All others might be out of date, with the cardbase definitely being out of date.

       The endpoint that might be of interest for you is /api/auctions/cards, some of the data might not be correct though, as it disregards ' in names, doesn't have all hybrid orbs (FireNature).

       You can get all the card data through: https://hub.backend.skylords.eu/api/auctions/cards?id=all

       If you need a bit more in depth card data, you could use the api I provide. The data should be mostly up to date except for the abilities and can be found here: https://smj.cards/api/cards

       How to Upload Card images:
       I downloaded them from the wiki, or extracted them from the game's pack files. they are dynamically generated from the different parts, and I also expose them through an api:
       https://smj.cards/api/images/fullCard/00

       with the option to specify some stuff

       https://smj.cards/api/images/fullCard/00?upgrades=2&charges=1

       How to upload a deck into the game:

       Make a computer application that to generate a random deck and provide codes to import the deck ingame.
       There are a few different code versions that work ingame:

       Official Version M Code, Official Version A Code, Official Card IDs code

       with the array of official card ids being the simplest one

       How to import/export decks in the game:
       simply type /importdeck CODE  while CODE being in on of the above formats
       similarly /exportdeck will get the M version Code of you currently selected deck
       Example of deck import command in the Game Skylords Reborn:

       /importdeck [253,254,255,256,287,288,289,290,301,302,303,304,305,316,344,345,346,354,355,356]

*/

        public int cardId { get; set; }
        public string cardName { get; set; }
        public string rarity { get; set; }
        public string expansion { get; set; }
        public string promo { get; set; }
        public string obtainable { get; set; }
        public int fireOrbs { get; set; }
        public int frostOrbs { get; set; }
        public int natureOrbs { get; set; }
        public int shadowOrbs { get; set; }
        public int fireShadowOrbs { get; set; }
        public int fireFrostOrbs { get; set; }
        public int natureFrostOrbs { get; set; }
        public int shadownNatureOrbs { get; set; }
        public int shadowFrostOrbs { get; set; }
        public int neutralOrbs { get; set; }
        public string affinity { get; set; }
        public string cardType { get; set; }
    }

    public class DefenseType
    {
        [JsonProperty("0")]
        public string _0 { get; set; }

        [JsonProperty("1")]
        public string _1 { get; set; }

        [JsonProperty("2")]
        public string _2 { get; set; }

        [JsonProperty("3")]
        public string _3 { get; set; }

        [JsonProperty("-1")]
        public string _minus1 { get; set; }
    }

    // 0 Twilight, 1 Renegade, 2 Lost Souls, 3 Amii, 4 Rebirth
    public class Edition
    {
        [JsonProperty("0")]
        public string _0 { get; set; }

        [JsonProperty("1")]
        public string _1 { get; set; }

        [JsonProperty("2")]
        public string _2 { get; set; }

        [JsonProperty("3")]
        public string _3 { get; set; }

        [JsonProperty("4")]
        public string _4 { get; set; }
    }

    public class Enums
    {
        public Edition edition { get; set; }
        public Color color { get; set; }
        public Promo promo { get; set; }
        public Rarity rarity { get; set; }
        public Affinity affinity { get; set; }
        public Type type { get; set; }
        public Gender gender { get; set; }
        public MovementType movementType { get; set; }
        public AttackType attackType { get; set; }
        public OffenseType offenseType { get; set; }
        public DefenseType defenseType { get; set; }
        public SquadSize squadSize { get; set; }
        public StarterCard starterCard { get; set; }
        public Boosters boosters { get; set; }
        public UpgradeMaps upgradeMaps { get; set; }
        public AbilityType abilityType { get; set; }
        public AbilityAffinity abilityAffinity { get; set; }
        public UnitModel unitModel { get; set; }
        public UnitSpecies unitSpecies { get; set; }
        public UnitClass unitClass { get; set; }
        public BuildingClass buildingClass { get; set; }
        public SpellClass spellClass { get; set; }
        public MaxCharges maxCharges { get; set; }
    }

    public class Gender
    {
        [JsonProperty("0")]
        public string _0 { get; set; }

        [JsonProperty("1")]
        public string _1 { get; set; }

        [JsonProperty("2")]
        public string _2 { get; set; }

        [JsonProperty("-1")]
        public string _minus1 { get; set; }
    }

    public class MaxCharges
    {
        [JsonProperty("4")]
        public List<int> _4 { get; set; }

        [JsonProperty("8")]
        public List<int> _8 { get; set; }

        [JsonProperty("12")]
        public List<int> _12 { get; set; }

        [JsonProperty("16")]
        public List<int> _16 { get; set; }

        [JsonProperty("20")]
        public List<int> _20 { get; set; }

        [JsonProperty("24")]
        public List<int> _24 { get; set; }
    }

    public class MovementType
    {
        [JsonProperty("0")]
        public string _0 { get; set; }

        [JsonProperty("1")]
        public string _1 { get; set; }

        [JsonProperty("-1")]
        public string _minus1 { get; set; }
    }

    public class OffenseType
    {
        [JsonProperty("0")]
        public string _0 { get; set; }

        [JsonProperty("1")]
        public string _1 { get; set; }

        [JsonProperty("2")]
        public string _2 { get; set; }

        [JsonProperty("3")]
        public string _3 { get; set; }

        [JsonProperty("4")]
        public string _4 { get; set; }

        [JsonProperty("-1")]
        public string _minus1 { get; set; }
    }

    public class Promo
    {
        [JsonProperty("0")]
        public string _0 { get; set; }

        [JsonProperty("1")]
        public string _1 { get; set; }
    }

    public class Rarity
    {
        [JsonProperty("0")]
        public string _0 { get; set; }

        [JsonProperty("1")]
        public string _1 { get; set; }

        [JsonProperty("2")]
        public string _2 { get; set; }

        [JsonProperty("3")]
        public string _3 { get; set; }
    }

    public class SMJCards
    {
        public bool success { get; set; }
        public Enums enums { get; set; }
        public List<SMJCard> data { get; set; }
    }

    public class SpellClass
    {
        [JsonProperty("~")]
        public string spellClass { get; set; }

    }

    public class SquadSize
    {
        [JsonProperty("0")]
        public string _0 { get; set; }

        [JsonProperty("1")]
        public string _1 { get; set; }

        [JsonProperty("2")]
        public string _2 { get; set; }

        [JsonProperty("4")]
        public string _4 { get; set; }

        [JsonProperty("6")]
        public string _6 { get; set; }

        [JsonProperty("-1")]
        public string _minus1 { get; set; }
    }

    public class StarterCard
    {
        [JsonProperty("0")]
        public string _0 { get; set; }

        [JsonProperty("1")]
        public string _1 { get; set; }
    }

    public class Type
    {
        [JsonProperty("0")]
        public string _0 { get; set; }

        [JsonProperty("1")]
        public string _1 { get; set; }

        [JsonProperty("2")]
        public string _2 { get; set; }
    }

    public class UnitClass
    {
        [JsonProperty("~")]
        public string unitClass { get; set; }
    }

    public class UnitModel
    {
        [JsonProperty("~")]
        public string unitModel { get; set; }
    }

    public class UnitSpecies
    {
        [JsonProperty("~")]
        public string unitSpecies { get; set; }
    }

    public class UpgradeMaps
    {
        [JsonProperty("0-0")]
        public string _00 { get; set; }

        [JsonProperty("1-1")]
        public string _11 { get; set; }

        [JsonProperty("1-2")]
        public string _12 { get; set; }

        [JsonProperty("1-3")]
        public string _13 { get; set; }

        [JsonProperty("1-4")]
        public string _14 { get; set; }

        [JsonProperty("1-5")]
        public string _15 { get; set; }

        [JsonProperty("1-6")]
        public string _16 { get; set; }

        [JsonProperty("1-7")]
        public string _17 { get; set; }

        [JsonProperty("1-8")]
        public string _18 { get; set; }

        [JsonProperty("1-9")]
        public string _19 { get; set; }

        [JsonProperty("2-1")]
        public string _21 { get; set; }

        [JsonProperty("2-2")]
        public string _22 { get; set; }

        [JsonProperty("2-3")]
        public string _23 { get; set; }

        [JsonProperty("2-4")]
        public string _24 { get; set; }

        [JsonProperty("2-5")]
        public string _25 { get; set; }

        [JsonProperty("2-6")]
        public string _26 { get; set; }

        [JsonProperty("2-7")]
        public string _27 { get; set; }

        [JsonProperty("4-1")]
        public string _41 { get; set; }

        [JsonProperty("4-2")]
        public string _42 { get; set; }

        [JsonProperty("4-3")]
        public string _43 { get; set; }

        [JsonProperty("4-4")]
        public string _44 { get; set; }

        [JsonProperty("4-5")]
        public string _45 { get; set; }

        [JsonProperty("4-6")]
        public string _46 { get; set; }

        [JsonProperty("4-7")]
        public string _47 { get; set; }

        [JsonProperty("4-8")]
        public string _48 { get; set; }
    }

    public static class Utils
    {
        private static readonly string[] CHARSETVERSION_M = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-".Split("");
        private static readonly int CHUNKLENGTH_VERSION_M = 2;

        public static string EncodeCardVersionM(int officialCardId)
        {
            string cardCode = "";

            while (officialCardId > 0)
            {
                cardCode = CHARSETVERSION_M[officialCardId % CHARSETVERSION_M.Length] + cardCode;
                officialCardId = (int)Math.Floor((double)officialCardId / CHARSETVERSION_M.Length);
            }

            while (cardCode.Length < CHUNKLENGTH_VERSION_M)
            {
                cardCode = CHARSETVERSION_M[0] + cardCode;
            }

            return cardCode;
        }

        public static SMJCard CopySkylordsRebornCard(SkylordsRebornCard SLRCard)
        {
            SMJCard smjCard = new SMJCard();
            // smjCard._id = ???
            smjCard.officialCardIds = new List<int> { SLRCard.cardId };
            smjCard.cardName = smjCard.cardNameSimple = SLRCard.cardName;
            smjCard.rarity = int.Parse(SLRCard.rarity);
            smjCard.promo = int.Parse(SLRCard.promo);
            smjCard.orbsFire = SLRCard.fireOrbs;
            smjCard.orbsShadow = SLRCard.shadowOrbs;
            smjCard.orbsNature = SLRCard.natureOrbs;
            smjCard.orbsFrost = SLRCard.frostOrbs;
            smjCard.orbsFireShadow = SLRCard.fireShadowOrbs;
            smjCard.orbsNatureFrost = SLRCard.natureFrostOrbs;
            // smjCard.orbsFireNature = SLRCard.fireOrbs;
            smjCard.orbsShadowFrost = SLRCard.shadowFrostOrbs;
            smjCard.orbsShadowNature = SLRCard.shadownNatureOrbs;
            smjCard.orbsFireFrost = SLRCard.fireFrostOrbs;
            smjCard.orbsNeutral = SLRCard.neutralOrbs;
            smjCard.affinity = int.Parse(SLRCard.affinity);
            smjCard.type = int.Parse(SLRCard.cardType);

            return smjCard;
        }

        public static SkylordsRebornCard CopySMJCard(SMJCard SMJCard)
        {
            SkylordsRebornCard SLRCard = new SkylordsRebornCard();
            SLRCard.cardId = SMJCard.officialCardIds[0];
            SLRCard.cardName = SMJCard.cardName;
            SLRCard.rarity = SMJCard.rarity.ToString();
            SLRCard.promo = SMJCard.promo.ToString();

            SLRCard.fireOrbs = SMJCard.orbsFire;
            SLRCard.shadowOrbs = SMJCard.orbsShadow;
            SLRCard.natureOrbs = SMJCard.orbsNature;
            SLRCard.frostOrbs = SMJCard.orbsFrost;
            SLRCard.fireShadowOrbs = SMJCard.orbsFireShadow;
            SLRCard.natureFrostOrbs = SMJCard.orbsNatureFrost;
            //SLRCard.fireNatureOrbs = SMJCard.orbsFireNature;
            SLRCard.shadowFrostOrbs = SMJCard.orbsShadowFrost;
            SLRCard.shadownNatureOrbs = SMJCard.orbsShadowNature;
            SLRCard.fireFrostOrbs = SMJCard.orbsFireFrost;
            SLRCard.neutralOrbs = SMJCard.orbsNeutral;
            SLRCard.affinity = SMJCard.affinity.ToString();
            SLRCard.cardType = SMJCard.type.ToString();

            return SLRCard;
        }

    }
}
