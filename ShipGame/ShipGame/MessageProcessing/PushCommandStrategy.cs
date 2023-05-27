using Hwdtech;
using ShipGame.Move;
using ICommand = ShipGame.Move.ICommand;

namespace ShipGame.MessageProcessing
{
    public class PushCommand : IStrategy
    {
        public object RunStrategy(params object[] args)
        {
            string id = (string)args[0];
            ICommand command = (ShipGame.Move.ICommand)args[1];

            var queue = IoC.Resolve<Queue<ICommand>>("GetQueue", id);

            return new ActionCommand(() => { queue.Enqueue(command); });
        }
    }
}

