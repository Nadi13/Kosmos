using Hwdtech;
namespace ShipGame.Move;


public class GetItem : IStrategy
{
    public object RunStrategy(params object[] args)
    {
        IoC.Resolve<Dictionary<string, IUObject>>("General.Objects").TryGetValue((string) args[0], out IUObject? obj);

        if (obj != null)
        {
            return obj;
        }
        throw new Exception();
    }
}
