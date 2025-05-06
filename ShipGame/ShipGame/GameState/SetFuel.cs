using Hwdtech;
using ShipGame.Move;


namespace ShipGame.GameState;

public class SetFuel : IStrategy
{
    public object RunStrategy(params object[] args)
    {
        IUObject obj = (IUObject) args[0];
        double fuel = Convert.ToDouble(args[1]);
        return new SetFuelCommand(obj, fuel);
    }
}
