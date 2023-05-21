using ICommand = ShipGame.Move.ICommand;

namespace ShipGame.Server
{
    public class ActionCommand: ICommand
    {
        private Action action;
        public ActionCommand(Action action)
        {
            this.action = action;
        }
        public void Execute()
        {
            action();
        }
    }
}
