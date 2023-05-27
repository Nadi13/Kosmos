using ICommand = ShipGame.Move.ICommand;

namespace ShipGame.Server
{
    public class StopCommand: ICommand
    {
        ServerThread stoppingThread;
        public StopCommand(ServerThread stoppingThread)
        {
            this.stoppingThread = stoppingThread;
        }
        public void Execute()
        {
            if (stoppingThread.Equals(Thread.CurrentThread))
            {
                stoppingThread.Stop();
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
