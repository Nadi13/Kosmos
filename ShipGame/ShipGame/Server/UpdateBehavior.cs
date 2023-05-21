using ICommand = ShipGame.Move.ICommand;
namespace ShipGame.Server
{
    public class UpdateBehavior: ICommand
    {
        ServerThread updateBehaviorThread;
        Action action;
        public UpdateBehavior(ServerThread updateBehaviorThread, Action action)
        {
            this.updateBehaviorThread = updateBehaviorThread;
            this.action = action;
        }
        public void Execute()
        {
            if (updateBehaviorThread.Equals(Thread.CurrentThread))
            {
                updateBehaviorThread.UpdateBehavior(action);
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
