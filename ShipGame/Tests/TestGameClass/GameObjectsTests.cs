using Hwdtech;
using Hwdtech.Ioc;
using Moq;

using ShipGame.Move;
using ShipGame.Server;
using ICommand = ShipGame.Move.ICommand;

namespace Tests.TestGameClass;
public class GameObjectsTests
{
    public GameObjectsTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
    }
    [Test]
    public void getItemTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Commands.GameCommand", (object[] args) => new ActionCommand(
            () =>
            {
                IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", args[0]).Execute();
            }
        )).Execute();

        ICommand gameCommand = (ICommand) new CreateNewGame().RunStrategy();
        gameCommand.Execute();

        var mockObj = new Mock<IUObject>();

        IoC.Resolve<Dictionary<string, IUObject>>("General.Objects").Add("0", mockObj.Object);

        var resolvedObj = IoC.Resolve<IUObject>("General.GetItem", "0");

        Assert.Equal(mockObj.Object, resolvedObj);
    }
    [Test]
    public void removeItemTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Commands.GameCommand", (object[] args) => new ActionCommand(
            () =>
            {
                IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", args[0]).Execute();
            }
        )).Execute();

        ICommand gameCommand = (ICommand) new CreateNewGame().RunStrategy();
        gameCommand.Execute();

        var mockObj = new Mock<IUObject>();

        IoC.Resolve<Dictionary<string, IUObject>>("General.Objects").Add("0", mockObj.Object);
        Assert.True( IoC.Resolve<Dictionary<string, IUObject>>("General.Objects").Count() == 1);

        IoC.Resolve<ICommand>("General.RemoveItem", "0").Execute();
        Assert.True( IoC.Resolve<Dictionary<string, IUObject>>("General.Objects").Count() == 0);
    }
    [Test]
    public void getItemTestObjectNotExists()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Commands.GameCommand", (object[] args) => new ActionCommand(
            () =>
            {
                IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", args[0]).Execute();
            }
        )).Execute();

        ICommand gameCommand = (ICommand) new CreateNewGame().RunStrategy();
        gameCommand.Execute();

        var mockObj = new Mock<IUObject>();

        IoC.Resolve<Dictionary<string, IUObject>>("General.Objects").Add("0", mockObj.Object);

        IoC.Resolve<ICommand>("General.RemoveItem", "0").Execute();
        Assert.True( IoC.Resolve<Dictionary<string, IUObject>>("General.Objects").Count() == 0);

        Assert.Throws<Exception>(
            () =>
            {
                IoC.Resolve<IUObject>("General.GetItem", "0");
            }
        );
    }
    
}
