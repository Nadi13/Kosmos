using Hwdtech;
using ICommand = ShipGame.Move.ICommand;

namespace ShipGame.MessageProcessing
{
    public class InterpretCommand : ICommand
    {
        IMessage message;

        public InterpretCommand(IMessage msg)
        {
            message = msg;
        }

        public void Execute()
        {
            var cmd = IoC.Resolve<ICommand>("CreateCommand", message);

            IoC.Resolve<ICommand>("PushCommand", message.Gameid, cmd).Execute();
        }
    }
}

