using Api;

namespace Example
{
    // Track the information for well in the game
    public class Well
    {
        public Well(EntityId id)
        {
            Id = id;
            Pos = Position2DExt.Zero();
        }
       
        public EntityId Id { get; set; }
        public EntityId? Owner { get; set; }

        public Position2D Pos;

        public float Power = 0f;
    }
}
