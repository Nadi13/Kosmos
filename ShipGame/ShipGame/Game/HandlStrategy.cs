using ICommand = ShipGame.Move.ICommand;
using Hwdtech;
using ShipGame.Move;

namespace ShipGame.Game
{
    public class HandlStrategy: IStrategy
    {
        public object RunStrategy(params object[] args)
        {
            Type exception = args[0].GetType();
            var command = (ICommand)args[1];

            var dictExceptionHandlers = IoC.Resolve<IDictionary<Type, Dictionary<ICommand, IStrategy>>>("Dictionary.Handler.Exception");

            if (!dictExceptionHandlers.ContainsKey(exception) || !dictExceptionHandlers[exception].ContainsKey(command))
            {
                var commandData = new Dictionary<string, object>();
                commandData["NoStrategyForCommand"] = command;
                var ex = new Exception();
                ex.Data["Unknown"] = commandData;
                throw ex;
            }

            else
            {
                var handledStrategy = dictExceptionHandlers[exception][command];
                var handledCommand = handledStrategy.RunStrategy();
                return handledCommand;
            }
        }
    }
}
