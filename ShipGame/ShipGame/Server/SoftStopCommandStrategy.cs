using Hwdtech;
using ShipGame.Move;

namespace ShipGame.Server
{
    public class SoftStopCommandStrategy : IStrategy
    {
        public object RunStrategy(params object[] args)
        {
            var id = args[0];
            var ST = IoC.Resolve<ServerThread>("ServerThreadGetByID", id);
            if (args.Length > 1)
            {
                Action act1 = (Action)args[1];
                Action newStrategy = new Action(() =>
                {
                    if (!ST.QueueIsEmpty())
                    {
                        ST.HandleCommand();
                    }
                    else
                    {
                        new StopCommand(ST).Execute();
                        act1();
                    }
                });
                 var softStopCommand = new UpdateBehavior(ST, newStrategy);
                 return softStopCommand;
            }
            else
            {
                Action newStrategy = new Action(() =>
                {
                    if (!ST.QueueIsEmpty())
                    {
                        ST.HandleCommand();
                    }
                    else
                    {
                        new StopCommand(ST).Execute();
                    }
                });
                var softStopCommand = new UpdateBehavior(ST, newStrategy);
                return softStopCommand;
            }
        }
    }
}
