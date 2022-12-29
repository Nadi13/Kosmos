using Hwdtech;
namespace ShipGame.Move
{
    public class StartMoveCommand: ICommand
    {
        IMoveCommandStartable startable;

        public StartMoveCommand(IMoveCommandStartable obj)
        {
            startable = obj;
        }
        public void Execute()
        {
            startable.Properties.ToList().ForEach(o => IoC.Resolve<ICommand>("Сommon.SetProperty", startable.Target, o.Key, o.Value).Execute());
            ICommand MoveCommand = IoC.Resolve<ICommand>("Long.Move", startable.Target);
            IoC.Resolve<ICommand>("Сommon.SetProperty",startable.Target,"Commands.Movement",MoveCommand).Execute();
            IoC.Resolve<ICommand>("Queue.Push",MoveCommand).Execute();
        }
    }
}
