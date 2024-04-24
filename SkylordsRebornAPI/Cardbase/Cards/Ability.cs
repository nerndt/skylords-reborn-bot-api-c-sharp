namespace SkylordsRebornAPI.Cardbase.Cards
{
    public class Ability
    {
        public string Name { get; set; }
        public AbilityType Type { get; set; }
        public int Power { get; set; }
        public int Order { get; set; }
        public string Description { get; set; }
        public int Era { get; set; }
    }
}