using ICommand = ShipGame.Move.ICommand;
using Hwdtech;

namespace ShipGame.Server
{
    public class ServerThread
    {
        internal bool stop = false;
        private IReceiver queue;
        private IReceiver orderQueue;
        private Thread thread;
        private Action strategy;
        public ServerThread(IReceiver queue, IReceiver orderQueue)
        {
            this.queue = queue;
            this.orderQueue = orderQueue;
            strategy = new Action(() =>
            {
                HandleCommand();
            });
            this.thread = new Thread(() =>
            {
                while (!stop)
                {
                    strategy.Invoke();
                }
            });
        }
        public void Stop()
        {
            stop = true;
        }
        public void Start()
        {
            thread.Start();
        }
        internal void HandleCommand()
        {
             if (!orderQueue.IsEmpty()){
                ICommand order = orderQueue.Receive();
                tryExecute(order);
            }
            ICommand cmd = queue.Receive();
            tryExecute(cmd);
        }
        public void UpdateBehavior(Action newBeh)
        {
            strategy = newBeh;
        }
        public bool GetStop()
        {
            return this.stop;
        }
        public bool QueueIsEmpty()
        {
            return queue.IsEmpty();
        }
        public bool Equals(Thread thread)
        {
            return this.thread == thread;
        }
        internal void tryExecute(ICommand command)
        {
            try
            {
                command.Execute();
            }
            catch(Exception e)
            {
                var exceptionCommand = IoC.Resolve<ICommand>("HandleException", e, command);
                exceptionCommand.Execute();
            }
        }
    }
}
