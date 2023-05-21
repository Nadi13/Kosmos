using Hwdtech;
using ShipGame.Move;
using ShipGame.MacroCommands;
using ICommand = ShipGame.Move.ICommand;

namespace ShipGame.Server
{
    public class HardStopCommandStrategy: IStrategy
    {
        public object RunStrategy(params object[] args)
        {
            var id = args[0];
            var ST = IoC.Resolve<ServerThread>("ServerThreadGetByID", id);
            var hardStopCommand = new StopCommand(ST);
            if (args.Length > 1)
            {
                Action act = (Action)args[1];
                List<ICommand> commands = new List<ICommand>();
                var actCommand = new ActionCommand((Action)args[1]);
                commands.Add(hardStopCommand);
                commands.Add(actCommand);
                return new MacroCommand(commands);
            }
            else
            {
                return hardStopCommand;
            }
        }
    }
}
