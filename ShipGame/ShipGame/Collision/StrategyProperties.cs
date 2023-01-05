using ShipGame.Move;

namespace ShipGame.Collision
{
    public class StrategyProperties: IStrategy
    {
        public object RunStrategy(params object[] args)
        {
            IUObject uobject = (IUObject)args[0];
            string propertyName = (string)args[1];
            return uobject.GetProperty(propertyName);
        }
    }
}
