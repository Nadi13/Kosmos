using Hwdtech;
using ShipGame.Move;
using ShipGame.Server;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SpaceBattle.ServerStrategies
{
    public class CreateWithStartThreadStrategy : IStrategy
    {
        public object RunStrategy(params object[] args)
        {
            var senderDict = IoC.Resolve<ConcurrentDictionary<string, ISender>>("SenderDictionary");
            var threadDict = IoC.Resolve<ConcurrentDictionary<string, ServerThread>>("ThreadDictionary");
            var externalSenderDict = IoC.Resolve<ConcurrentDictionary<string, ISender>>("ExternalSenderDictionary");
            BlockingCollection<ShipGame.Move.ICommand> queue = new BlockingCollection<ShipGame.Move.ICommand>(100);
            BlockingCollection<ShipGame.Move.ICommand> queue2 = new BlockingCollection<ShipGame.Move.ICommand>();
            var sender = new SenderAdapter(queue);
            var sender2 = new SenderAdapter(queue2);
            if (args.Length > 1)
            {
                sender.Send(new ActionCommand((Action)args[1]));
            }
            var receiver = new ReceiverAdapter(queue);
            var receiver2 = new ReceiverAdapter(queue2);
            var ST = new ServerThread(receiver, receiver2);
            ST.Start();
            senderDict.TryAdd((string)args[0], sender);
            threadDict.TryAdd((string)args[0], ST);
            externalSenderDict.TryAdd((string)args[0], sender2);
            return ST;
        }
    }
}
