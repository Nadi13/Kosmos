using Hwdtech;
using ShipGame.Move;


namespace ShipGame.GameState;

public class PlaceObjects : IStrategy
{
    public object RunStrategy(params object[] args)
    {
        IUObject[] objects = (IUObject[]) args[0];
        
            string placementMethod = (string) args[1];
            if (placementMethod == "Placements.PairLike")
            {
                int verticalOffset = (int) args[2];
                int horizontalDif = (int) args[3];
                int horizontalPos = (int) args[4];
                for (int i = 0; i < objects.Count(); i++)
                {
                    Vector pos = new Vector(horizontalPos + horizontalDif * (i % 2), verticalOffset * (i / 2 ));
                    objects[i].SetProperty("position", pos);
                }
            }
            else // вертикальное расположение
            {
                int vericalOffset = (int)args[2];
                int horizontalPos = (int)args[3];
                for (int i = 0; i < objects.Count(); i++)
                {
                    Vector pos = new Vector(horizontalPos, vericalOffset * i);
                    objects[i].SetProperty("position", pos);
                }
            }
        return new object();
    }
}
