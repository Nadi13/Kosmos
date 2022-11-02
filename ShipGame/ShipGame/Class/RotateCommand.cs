using ShipGame.Interface;

namespace ShipGame.Class
{
    public class RotateCommand : ICommand
    {
        private IRotatable rotatable;
        public RotateCommand(IRotatable rotatable)
        {
            this.rotatable = rotatable;
        }
        public void Execute()
        {
            rotatable.Angle = Fraction.Sum(rotatable.Angle, rotatable.AngleVelocity);
        }
    }
}
