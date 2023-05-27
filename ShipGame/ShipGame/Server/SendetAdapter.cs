using System.Collections.Concurrent;

namespace ShipGame.Server
{
    public class SenderAdapter : ISender
    {
        BlockingCollection<ShipGame.Move.ICommand> queue;
        public SenderAdapter(BlockingCollection<ShipGame.Move.ICommand> queue)
        {
            this.queue = queue;
        }
        public void Send(ShipGame.Move.ICommand command)
        {
            queue.Add(command);
        }
    }
}
