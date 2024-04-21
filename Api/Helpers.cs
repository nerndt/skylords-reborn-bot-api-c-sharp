
namespace Api
{
    public static class PositionExtension
    {
        public static Position2D To2D(this Position position)
        {
            return new Position2D { X = position.X, Y = position.Z };
        }
    }
    public static class Position2DExt
    {
        public static Position2D Zero()
        {
            return new Position2D { X = 0, Y = 0 };
        }
    }
    public static class CardIdCreator
    {
        public static CardId New(CardTemplate ct, Upgrade u)
        {
            return new CardId((uint)ct + (uint)u);
        }
    }
}
