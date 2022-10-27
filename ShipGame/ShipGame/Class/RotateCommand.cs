using ShipGame.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
