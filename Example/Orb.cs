using Api;
using System.Drawing;

namespace Bots
{
    // Track the information for each orb in the game
    public class Orb
    {
        public Orb(EntityId id)
        {
            Id = id;
            Pos = Position2DExt.Zero();
            Power = 0f;
            Color = OrbColor.White;
        }

        public EntityId Id { get; set; }
        public EntityId? Owner { get; set; }

        public Position2D Pos;

        public float Power = 0f;

        public OrbColor Color;
    }
}
