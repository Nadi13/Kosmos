using ShipGame.Move;

namespace ShipGame.GameState
{
    public class SetFuelCommand : ICommand
    {
        private IUObject obj;
        private double fuelLevel;

        public SetFuelCommand(IUObject obj, double fuelLevel)
        {
            this.obj = obj;
            this.fuelLevel = fuelLevel;
        }
        public void Execute()
        {
            obj.SetProperty("fuel", fuelLevel);
        }
    }
}
