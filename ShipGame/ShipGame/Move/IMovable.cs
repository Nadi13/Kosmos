using ShipGame.Move;

namespace ShipGame.Move
{
    public interface IMovable
    {
        Vector Position { get; set; }
        Vector Velocity { get; }
    }
}
