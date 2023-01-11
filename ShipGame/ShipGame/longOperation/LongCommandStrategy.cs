using Hwdtech;
using ShipGame.Move;

namespace ShipGame.longOperation
{
    public class LongCommandStrategy : IStrategy
    {
        public object RunStrategy(params object[] args)
        {
            IUObject obj = (IUObject)args[1];
            var namedip = (string)args[0];
            var create_com = IoC.Resolve<Move.ICommand>("Create.Command", namedip, obj);
            var longcom = IoC.Resolve<Move.ICommand>("Commands.Paste", create_com);
            return IoC.Resolve<Move.ICommand>("Queue.Push", longcom);
        }
    }
}
