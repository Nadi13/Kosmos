using ICommand = ShipGame.Move.ICommand;
using Hwdtech;

namespace ShipGame.Server
{
    public class ServerThread
    {
        internal bool stop = false;
        private IReceiver queue;
        private Thread thread;
        private Action strategy;
        private IReceiver externalQueue;
        public ServerThread(IReceiver queue, IReceiver externalQueue)
        {
            this.queue = queue;
            this.externalQueue = externalQueue;

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
            ICommand cmd = this.queue.Receive();
            ICommand cmd2 = this.externalQueue.Receive();
            try
            {
                cmd2?.Execute();
            }
            catch (Exception e)
            {
                var exceptionCommand = IoC.Resolve<ICommand >("HandleException", e, cmd2);
                exceptionCommand.Execute();
            }
            try
            {
                cmd?.Execute();
            }
            catch (Exception e)
            {
                var exceptionCommand = IoC.Resolve<ICommand >("HandleException", e, cmd);
                exceptionCommand.Execute();
            }
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
    }
}
