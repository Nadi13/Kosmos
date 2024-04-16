using Hwdtech;
using ShipGame.Move;


namespace ShipGame.GameState;

public class PlaceObjects : IStrategy
{
    public object RunStrategy(params object[] args)
    {
        string placementMethod = (string) args[0];
            if (placementMethod == "Placements.PairLike")
            {
                IUObject[] left_objects = (IUObject[])args[1];
                IUObject[] right_objects = (IUObject[])args[2];
                int verticalOffset = (int) args[3];
                int horizontalDif = (int) args[4];
                int horizontalPos = (int) args[5];
                int verticalPos = (int) args[6];
                

                var objects = left_objects.Zip(right_objects, (l, r) => new {left = l, right = r});
                
                foreach (var pair in objects)
                {
                    new SetPositionCommand(pair.left, new Vector(horizontalPos, verticalPos)).Execute();
                    new SetPositionCommand(pair.right, new Vector(horizontalPos + horizontalDif, verticalPos)).Execute();
                    verticalPos += verticalOffset;
                }
            }
            else 
            {
                IUObject[] objects = (IUObject[])args[1];
                int vericalOffset = (int)args[2];
                int horizontalPos = (int)args[3];
                int verticalPos = (int)args[4];
 
                foreach (IUObject obj in objects)
                {
                    new SetPositionCommand(obj, new Vector(horizontalPos, verticalPos)).Execute();
                    verticalPos += vericalOffset;
                }
            }
        return new object();
    }
}
