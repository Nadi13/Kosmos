using ShipGame.Move;

namespace ShipGame.MacroCommands
{
    public class MacroCommand: ShipGame.Move.ICommand
    {
        IEnumerable<ShipGame.Move.ICommand> commands;
        public MacroCommand(IEnumerable<ShipGame.Move.ICommand> commands)
        {
            this.commands = commands;
        }

        public void Execute()
        {
            var icommand = commands.GetEnumerator();
            while(icommand.MoveNext())
            {
                icommand.Current.Execute();
            }
        }

    }
}
