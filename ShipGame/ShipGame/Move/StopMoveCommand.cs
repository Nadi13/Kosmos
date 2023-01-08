using Hwdtech;
namespace ShipGame.Move
{
    public class StopMoveCommand : ICommand
    {
        IMoveCommandEndable endable;

        public StopMoveCommand(IMoveCommandEndable obj)
        {
            endable = obj;
        }
        public void Execute()
        {
            ICommand StopCommand = IoC.Resolve<ICommand>("Empty.Command");
            IoC.Resolve<ICommand>("Delete.Property", endable.Item).Execute();
            IoC.Resolve<ICommand>("Insert.Command", endable.Obj, endable.MoveCommand, StopCommand).Execute();
        }
    }
}
