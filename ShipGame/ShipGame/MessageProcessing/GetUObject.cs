using Hwdtech;
using ShipGame.Move;

namespace ShipGame.MessageProcessing
{
    public class GetUObject : IStrategy
    {
        public object RunStrategy(params object[] args)
        {
            string objectid = (string)args[0]; ;

            if (!IoC.Resolve<IDictionary<string, IUObject>>("UObjectDictionary").TryGetValue(objectid, out IUObject? uObject))
            {
                throw new Exception();
            }
            else
            {
                return uObject;
            }
        }
    }
}

