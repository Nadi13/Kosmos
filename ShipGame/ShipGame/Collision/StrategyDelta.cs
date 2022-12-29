using Hwdtech;
using ShipGame.Move;

namespace ShipGame.Collision
{
    public class StrategyDelta: IStrategy
    {
        public object RunStrategy(params object[] args)
        {
            var object1 = (IUObject)args[0];
            var object2 = (IUObject)args[1];
            int size = (int)args[2];
            var name = new List<string> { "Position", "Velocity" };
            var list_delta = new List<int>();
            foreach(string x in name)
            {
                var Obj1List = IoC.Resolve<List<int>>("GetProperty", object1, x);
                var Obj2List = IoC.Resolve<List<int>>("GetProperty", object2, x);
                for(int i=0; i<size; i++)
                {
                    list_delta.Add(Obj1List[i] - Obj2List[i]);
                }
            }
            return new List<int>(list_delta);
        }
    }
}
