using Api;

namespace Example
{
    // Track the information for each player in the game
    public class Team
    {
        public Dictionary<EntityId, Player> Players = new Dictionary<EntityId, Player>();

        public float Power = 0f;
    }
}
