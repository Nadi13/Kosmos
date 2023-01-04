using Hwdtech;
using ShipGame.Move;

namespace ShipGame.Collision
{
    public class CheckCollision : ShipGame.Move.ICommand
    {
        IUObject obj1 
        { 
            get; 
        }
        IUObject obj2 
        { 
            get;
        }

        public CheckCollision(IUObject UObj1, IUObject UObj2)
        {
            obj1 = UObj1;
            obj2 = UObj2;
        }

        public void Execute()
        {
            var list = IoC.Resolve<List<int>>("Delta", obj1, obj2);
            bool collision = IoC.Resolve<bool>("DecisionTree", list);
            if (collision) throw new Exception();
        }
    }
}
