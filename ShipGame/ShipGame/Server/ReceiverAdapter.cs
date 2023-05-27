using System.Collections.Concurrent;

namespace ShipGame.Server
{
    public class ReceiverAdapter : IReceiver
    {
        BlockingCollection<ShipGame.Move.ICommand> commands;
        public ReceiverAdapter(BlockingCollection<ShipGame.Move.ICommand> q)
        {
            this.commands = q;
        }
        public ShipGame.Move.ICommand Receive()
        {
            return commands.Take();
        }
        public bool IsEmpty()
        {
            return commands.Count() == 0;
        }
    }
}
