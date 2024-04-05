using Moq;
using Hwdtech;
using Hwdtech.Ioc;
using ShipGame.GameState;
using ShipGame.Move;

namespace Tests.TestGameState;
public class TestObject : IUObject
{
    public IDictionary<string, object> scope { get; set; }
    public object this[string key] { get { return new object(); } set { } }
    public TestObject(Dictionary<string, object> props)
    {
        scope = props;
    }
    public object GetProperty(string key)
    {
        return scope[key];
    }

    public void SetProperty(string key, object value)
    {
        scope[key] = value;
    }
}
public class GetItem : IStrategy
{
    public object RunStrategy(params object[] args)
    {
        IoC.Resolve<Dictionary<string, IUObject>>("General.Objects").TryGetValue((string)args[0], out IUObject? obj);

        if (obj != null)
        {
            return obj;
        }
        throw new Exception();
    }
}

public class GameInitTests

{
    public GameInitTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
        int playerid = 0;
        int objid = 0;
        Dictionary<string, IUObject> gameObjects = new Dictionary<string, IUObject>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "General.Objects", (Func<object[], Dictionary<string, IUObject>>) (args => gameObjects)).Execute();
        Dictionary<string, object> initProps = new Dictionary<string, object>
        {
            ["numberOfPlayers"] = 2,
        };
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.InitProperties", (Func<object[], Dictionary<string, object>>) (args => initProps)).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "General.AddNewPlayer", (Func<object[], string>) (args => Convert.ToString(playerid++))).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "General.Objects.Empty", (Func<object[], IUObject>) (args => new TestObject(new Dictionary<string, object>()))).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "General.Objects.EmptyId", (Func<object[], string>) (args => Convert.ToString(objid++))).Execute(); 
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "General.GetItem", (Func<object[], IUObject>) (args => (IUObject) new GetItem().RunStrategy(args[0]))).Execute();
    }
    
    [Test]
    public void setFuelTest()
    {
        TestObject obj = new TestObject(new Dictionary<string, object>());
        new SetFuel().RunStrategy(obj, 10);
        Assert.True((double) obj.GetProperty("fuel") == 10);
    }
    [Test]
    public void createShipsTests()
    {
        var gameObjects = IoC.Resolve<Dictionary<string, IUObject>>("General.Objects");
        new CreateEmptyShips().RunStrategy();
        Assert.True(gameObjects.Count() == 6);
        Assert.True((string) gameObjects["0"].GetProperty("player") == "0");
        Assert.True((string) gameObjects["1"].GetProperty("player") == "0");
        Assert.True((string) gameObjects["2"].GetProperty("player") == "0");
        Assert.True((string) gameObjects["3"].GetProperty("player") == "1");
        Assert.True((string) gameObjects["4"].GetProperty("player") == "1");
        Assert.True((string) gameObjects["5"].GetProperty("player") == "1");
    }
    [Test]
    public void placeObjectsTests()
    {
        var gameObjects = IoC.Resolve<Dictionary<string, IUObject>>("General.Objects");
        new CreateEmptyShips().RunStrategy();
        var friendlyShips = new IUObject[] {
            IoC.Resolve<IUObject>("General.GetItem", "0"),
            IoC.Resolve<IUObject>("General.GetItem", "1"),
            IoC.Resolve<IUObject>("General.GetItem", "2"),
        };
        new PlaceObjects().RunStrategy(friendlyShips, "Placements.Vertical", 10, 5);
        Assert.True((Vector) gameObjects["0"].GetProperty("position") == new Vector(5, 0));
        Assert.True((Vector) gameObjects["1"].GetProperty("position") == new Vector(5, 10));
        Assert.True((Vector) gameObjects["2"].GetProperty("position") == new Vector(5, 20));

        var enemyShips = new IUObject[] {
            IoC.Resolve<IUObject>("General.GetItem", "3"),
            IoC.Resolve<IUObject>("General.GetItem", "4"),
            IoC.Resolve<IUObject>("General.GetItem", "5"),
        };
        new PlaceObjects().RunStrategy(enemyShips, "Placements.PairLike", 15, 5, -5);
        Assert.True((Vector) gameObjects["3"].GetProperty("position") == new Vector(-5, 0));
        Assert.True((Vector) gameObjects["4"].GetProperty("position") == new Vector(0, 0));
        Assert.True((Vector) gameObjects["5"].GetProperty("position") == new Vector(-5, 15));

        new CreateEmptyShips().RunStrategy();
    }
}
