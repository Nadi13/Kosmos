using Hwdtech;
namespace ShipGame.Move;

public class DeleteGame : IStrategy
{
    public object RunStrategy(params object[] args)
    {
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.Outer")).Execute();
        return new object();
    }
}
