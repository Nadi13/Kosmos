using Hwdtech;
using ShipGame.Move;


namespace ShipGame.MessageProcessing
{
    public class GetQueue : IStrategy
    {
        public object RunStrategy(params object[] args)
        {
            string gameid = (string)args[0];

            if (!IoC.Resolve<IDictionary<string, Queue<ShipGame.Move.ICommand>>>("GameDictionary").TryGetValue(gameid, out Queue<ShipGame.Move.ICommand>? queue))
            {
                throw new Exception();
            }
            else
            {
                return queue;
            }
        }
    }
}
