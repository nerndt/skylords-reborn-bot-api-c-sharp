using System.Collections.Generic;
using SkylordsRebornAPI.Cardbase.Cards;
using SkylordsRebornAPI.Cardbase.Shared;

namespace SkylordsRebornAPI.Cardbase
{
    public class Card
    {
        public string Name { get; set; }

        public Rarity Rarity { get; set; }
        public int Cost { get; set; }

        public Edition Edition { get; set; }
        public CardType Type { get; set; }

        public int Color { get; set; }
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
    }
}