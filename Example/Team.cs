using Api;

namespace Bots
{
    // Track the information for each player in the game
    public class Team
    {
        public EntityId? myId;

        public Dictionary<EntityId, Player> Players = new Dictionary<EntityId, Player>();

        public float Power = 0f;
    }
}
