using System.Collections.Generic;
using SkylordsRebornAPI.Cardbase.Cards;
using SkylordsRebornAPI.Cardbase.Shared;

namespace SkylordsRebornAPI.Cardbase
{
    public class SMJCard0
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

        Example of a SMJCard string file:

                {
            "_id": "8p",
            "cardSlug": "abomination-b",
            "cardName": "Abomination [B]",
            "cardNameSimple": "Abomination",
            "cardNameImage": "Abomination",
            "officialCardIds": [
                1561
            ],
            "description": "Cost to transform into this unit: 165",
            "edition": 3,
            "color": 6,
            "promo": 0,
            "rarity": 2,
            "orbsTotal": 4,
            "orbsNeutral": 0,
            "orbsFire": 2,
            "orbsShadow": 0,
            "orbsNature": 2,
            "orbsFrost": 0,
            "orbsFireShadow": 0,
            "orbsNatureFrost": 0,
            "orbsFireNature": 0,
            "orbsShadowFrost": 0,
            "orbsShadowNature": 0,
            "orbsFireFrost": 0,
            "affinity": 3,
            "type": 0,
            "unitModel": "Behemoth",
            "category": "Beast Destroyer",
            "unitSpecies": "Beast",
            "unitClass": "Destroyer",
            "buildingClass": "~",
            "spellClass": "~",
            "gender": 1,
            "movementType": 0,
            "attackType": 0,
            "offenseType": 3,
            "defenseType": 3,
            "maxCharges": 4,
            "squadSize": 1,
            "starterCard": 0,
            "powerCost": [
                220,
                220,
                220,
                220
            ],
            "damage": [
                5270,
                5550,
                6100,
                6700
            ],
            "health": [
                5550,
                5550,
                5550,
                5550
            ],
            "boosters": [
                -2,
                -1,
                6
            ],
            "upgradeMaps": [
                "1-2",
                "1-2",
                "1-2"
            ],
            "abilities": [
                {
                    "abilityIdentifier": "Fury",
                    "abilityName": "Blessed Fury",
                    "abilityType": 1,
                    "abilityAffinity": 3,
                    "abilityAvailability": [
                        true,
                        true,
                        true,
                        true
                    ],
                    "abilityHidden": false,
                    "abilityStars": [
                        0,
                        1,
                        2,
                        3
                    ],
                    "abilityDescription": "Activate to release a furious roar that deals %s damage to enemies within a 25m radius around the unit, up to %s in total. Every hostile unit within that area will be stunned and unable to fight in close-combat for %s seconds. Reusable every 30 seconds.",
                    "abilityDescriptionValues": [
                        [
                            "840",
                            "900",
                            "900",
                            "1000"
                        ],
                        [
                            "2520",
                            "2700",
                            "2700",
                            "3000"
                        ],
                        [
                            "15",
                            "15",
                            "20",
                            "20"
                        ]
                    ],
                    "abilityCost": [
                        120,
                        120,
                        120,
                        120
                    ]
                },
                {
                    "abilityIdentifier": "Transformation",
                    "abilityName": "Transformation",
                    "abilityType": 1,
                    "abilityAffinity": -1,
                    "abilityAvailability": [
                        true,
                        true,
                        true,
                        true
                    ],
                    "abilityHidden": false,
                    "abilityStars": [
                        0,
                        0,
                        0,
                        0
                    ],
                    "abilityDescription": "The unit is infected with the Twilight Curse, activate to start the mutation process and choose a Twilight unit from the current deck to be transformed into after 1.2 seconds. The current deck must contain at least one other Twilight unit whose orb requirements are met! Has a 10 seconds cool-down after the card was played out or previously transformed.",
                    "abilityDescriptionValues": [],
                    "abilityCost": [
                        0,
                        0,
                        0,
                        0
                    ]
                }
            ]
        },

         */
        public string _id { get; set; }
        public string cardSlug { get; set; }
        public string cardName { get; set; }
        public string cardNameSimple { get; set; }
        public string cardNameImage { get; set; }
        public List<int> officialCardIds { get; set; }
        public string description { get; set; }
        public Edition edition { get; set; }
        public int color { get; set; }
        public int promo { get; set; }
        public Rarity rarity { get; set; }
        public int orbsTotal { get; set; }
        public int orbsNeutral { get; set; }
        public int orbsFire { get; set; }
        public int orbsShadow { get; set; }
        public int orbsNature { get; set; }
        public int orbsFrost { get; set; }
        public int orbsFireShadow { get; set; }
        public int orbsNatureFrost { get; set; }
        public int orbsFireNature { get; set; }
        public int orbsShadowFrost { get; set; }
        public int orbsShadowNature { get; set; }
        public int orbsFireFrost { get; set; }
        public int affinity { get; set; }
        public CardType type { get; set; }
        public string unitModel { get; set; }
        public string category { get; set; }
        public string unitSpecies { get; set; }
        public string unitClass { get; set; }
        public string buildingClass { get; set; }
        public string spellClass { get; set; }
        public int gender { get; set; }
        public int movementType { get; set; }
        public int attackType { get; set; }
        public OffenseType offenseType { get; set; }
        public DefenseType defenseType { get; set; }
        public int maxCharges { get; set; }
        public int squadSize { get; set; }
        public int starterCard { get; set; }
        public List<int> powerCost { get; set; }
        public List<int> damage { get; set; }
        public List<int> health { get; set; }
        public List<int> boosters { get; set; }
        public List<string> upgradeMaps { get; set; }
        public List<Ability> abilities { get; set; }
    }

    /*
        public string _id { get; set; }
        public string cardSlug { get; set; }
        public string cardName { get; set; }
        public string cardNameSimple { get; set; }
        public string cardNameImage { get; set; }
        public int[] officialCardIds { get; set; }

        public string description { get; set; }

        public Edition edition { get; set; }
        public int color { get; set; }

        public Rarity rarity { get; set; }
        
        public int Cost { get; set; }

        public CardType Type { get; set; }

        public Affinity Affinity { get; set; }

        public bool IsRanged { get; set; }
        public int Defense { get; set; }
        public int Offense { get; set; }
        public DefenseType DefenseType { get; set; }
        public OffenseType OffenseType { get; set; }
        public int UnitCount { get; set; }
        public int ChargeCount { get; set; }
        public string Category { get; set; }

        public List<Ability> Abilities { get; set; } = new();
        public List<Upgrade> Upgrades { get; set; } = new();

        public OrbInfo OrbInfo { get; set; }

        public string Extra { get; set; }
        public Media Image { get; set; }
    
    */
}