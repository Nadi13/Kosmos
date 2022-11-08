using ShipGame.Rotate;

namespace ShipGame.Rotate
{
    public interface IRotatable
    {
        Fraction Angle { get; set; }
        Fraction AngleVelocity { get; }
    }
}
