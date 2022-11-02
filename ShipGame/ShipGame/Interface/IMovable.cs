using ShipGame.Class;

namespace ShipGame.Interface
{
    public interface IMovable
    {
        Vector Position { get; set; }
        Vector Velocity { get; }
    }
}
