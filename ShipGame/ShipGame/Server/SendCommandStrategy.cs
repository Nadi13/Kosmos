using ShipGame.Move;

namespace ShipGame.Server
{
    public class SendCommandStrategy: IStrategy
    {
        public object RunStrategy(params object[] args)
        {
            var sendCommand = new SendCommand((ISender)args[0], (ICommand)args[1]);
            return sendCommand;
        }
    }
}
