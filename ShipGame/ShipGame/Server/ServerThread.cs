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
        public ServerThread(IReceiver queue)
        {
            this.queue = queue;
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
            try
            {
                cmd.Execute();
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
