using Api;

namespace Example
{
    // Track the information for each player in the game
    public class Player
    {
        public Player(EntityId id)
        {
            Id = id;
            PowerSlots = new List<PowerSlot>();
            Orbs = new List<Orbs>();
            Army = new List<EntityId>();
            StartPos = Position2DExt.Zero();
        }
       
        public EntityId Id { get; set; }
        public List<PowerSlot> PowerSlots { get; set; }
        public List<Orbs>  Orbs { get; set; }
        public List<EntityId> Army { get; set; }

        public Position2D StartPos;

        // List of players and locations

        public float Power = 0f;
    }
}
