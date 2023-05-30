using Hwdtech;
using ShipGame.Move;
using ICommand = ShipGame.Move.ICommand;

namespace ShipGame.Server
{
    public class SendCommandByThreadIDStrategy : IStrategy
    {
        public object RunStrategy(params object[] args)
        {
            var sender = IoC.Resolve<ISender>("SenderGetByID", args[0]);
            var sendCommand = new SendCommand(sender, (ICommand)args[1]);
            return sendCommand;
        }
    }
}

