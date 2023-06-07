using Hwdtech.Ioc;
using Hwdtech;
using ShipGame.Builder;
using ShipGame.Rotate;

namespace Tests.TestBuilderAdapter
{
    public class AdapterGeneratorTest
    {
        public AdapterGeneratorTest()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

            var adapterGenerator = new AdapterBuilder();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "BuilderForAdaptationDtype1ToDtype2", (object[] args) => adapterGenerator).Execute();
        }
        string exampleIRotatble = @"public class IRotatable_adapter : ShipGame.Rotate.IRotatable {
    private System.Collections.Generic.IDictionary<string, object> target;
    public IRotatable_adapter(System.Collections.Generic.IDictionary<string, object> target) {
        this.target = target;
    }
    public ShipGame.Rotate.Fraction Angle {
        get => IoC.Resolve<ShipGame.Rotate.Fraction>(""UObjectGetValue"", target, Angle);
        set => IoC.Resolve<ICommand>(""UObjectSetValue"", target, Angle, propertyValue).Execute();
    }
    public ShipGame.Rotate.Fraction AngleVelocity {
        get => IoC.Resolve<ShipGame.Rotate.Fraction>(""UObjectGetValue"", target, AngleVelocity);
    }
}";
        [Test]
        public void sameStringCodeTest()
        {
            var adapterGeneratorStrategy = new BuilderStrategy();
            var template = (string)adapterGeneratorStrategy.RunStrategy(typeof(IRotatable), typeof(object));
            Assert.True(exampleIRotatble.Replace("\r\n", "").Replace("    ", "").Replace("\n", "")==template);
        }
    }
}
