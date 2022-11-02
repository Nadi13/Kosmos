using ShipGame.Class;

namespace ShipGame.Interface
{
    public interface IRotatable
    {
        Fraction Angle { get; set; }
        Fraction AngleVelocity { get; }
    }
}
