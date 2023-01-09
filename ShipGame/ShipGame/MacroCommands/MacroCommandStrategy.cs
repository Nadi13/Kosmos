using Hwdtech;
using ShipGame.Move;

namespace ShipGame.MacroCommands
{
    public class MacroCommandStrategy: IStrategy
    {
        public object RunStrategy(params object[] args)
        {
            var obj1 = (IUObject)args[0];
            var name = (string)args[1];
            IEnumerable<string> NamesOfCommand = IoC.Resolve<IEnumerable<string>>("Config.MacroCommand." + name);
            IEnumerable<ShipGame.Move.ICommand> commands = new List<ShipGame.Move.ICommand>();
            var inameofcommand = NamesOfCommand.GetEnumerator();
            while (inameofcommand.MoveNext())
            {
                commands.Append(IoC.Resolve<ShipGame.Move.ICommand>(inameofcommand.Current, obj1));
            }
            return IoC.Resolve<ShipGame.Move.ICommand>("SimpleMacroCommand", commands);
        }
    }
}
