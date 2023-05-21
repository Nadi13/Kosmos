using Hwdtech;
using ShipGame.Move;
using ShipGame.Server;
using System.Collections.Concurrent;

namespace SpaceBattle.ServerStrategies
{
    public class CreateAndStartThreadStrategy : IStrategy
    {
        public object RunStrategy(params object[] args)
        {
            var senderDict = IoC.Resolve<ConcurrentDictionary<string, ISender>>("SenderDictionary");
            var sender = (ISender)args[1];
            senderDict.TryAdd((string)args[0], sender);
            if (args.Length > 3)
            {
                sender.Send(new ActionCommand((Action)args[3]));
            }
            var ST = new ServerThread((IReceiver)args[2]);
            ST.Start();
            var threadDict = IoC.Resolve<ConcurrentDictionary<string, ServerThread>>("ThreadDictionary");
            threadDict.TryAdd((string)args[0], ST);
            return ST;
        }
    }
}
