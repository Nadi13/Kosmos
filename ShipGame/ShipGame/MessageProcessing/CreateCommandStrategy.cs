using Hwdtech;
using ShipGame.Move;

namespace ShipGame.MessageProcessing
{
    public class CreateCommand : IStrategy
    {
        public object RunStrategy(params object[] args)
        {
            var message = (IMessage)args[0];

            var UObject = IoC.Resolve<IUObject>("GetUObject", message.UObjectid);

            message.Args.ToList().ForEach(x => UObject.SetProperty(x.Key, x.Value));
            return IoC.Resolve<Move.ICommand>("Command" + message.Cmd, UObject);
        }
    }
}

