using Hwdtech;
namespace ShipGame.Move;


public class CreateNewGame : IStrategy
{
    int quantum;
    public CreateNewGame(int _quantum = 500)
    {
        quantum = _quantum;
    }
    public object RunStrategy(params object[] args)
    {
        Queue<ICommand> queue = new Queue<ICommand>();
        object scope = new InitGameScope().RunStrategy(quantum);
        return IoC.Resolve<ICommand>("Commands.GameCommand", scope, queue);
    }
}
