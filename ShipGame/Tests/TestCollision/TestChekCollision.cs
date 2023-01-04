using Hwdtech;
using Moq;
using Hwdtech.Ioc;
using ShipGame.Collision;
using ShipGame.Move;

namespace Tests.TestCollision
{
    public class TestChekCollision
    {
        Mock<IStrategy> DecisionStrategy = new Mock<IStrategy>();
        StrategyDelta DeltaStrategy = new StrategyDelta();
        StrategyProperties GetPropertyStrategy = new StrategyProperties();
        public TestChekCollision()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetProperty", (object[] args) => GetPropertyStrategy.RunStrategy(args)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Delta", (object[] args) => DeltaStrategy.RunStrategy(args)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "DecisionTree", (object[] args) => DecisionStrategy.Object.RunStrategy(args)).Execute();
        }

        private static void GetPropertyVelocity1(Mock<IUObject>mock1, List<int> list)
        {
            mock1.Setup(x => x.GetProperty("Velocity")).Returns(list);
        }
        private static void GetPropertyPosition1(Mock<IUObject> mock1, List<int> list)
        {
            mock1.Setup(x => x.GetProperty("Position")).Returns(list);
        }
        private static void GetPropertyVelocity2(Mock<IUObject> mock2, List<int> list)
        {
            mock2.Setup(x => x.GetProperty("Velocity")).Returns(list);
        }
        private static void GetPropertyPosition2(Mock<IUObject> mock2, List<int> list)
        {
            mock2.Setup(x => x.GetProperty("Position")).Returns(list);
        }

        [Test]
        public void PositiveCollision()
        {
            var mock1 = new Mock<IUObject>();
            var mock2 = new Mock<IUObject>();
            GetPropertyVelocity1(mock1, new List<int> { 2, 1 });
            GetPropertyPosition1(mock1, new List<int> { 1, 1 });
            GetPropertyVelocity2(mock2, new List<int> { 0, 1 });
            GetPropertyPosition2(mock2, new List<int> { 0, 1 });

            DecisionStrategy.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(true).Verifiable();
            CheckCollision collision = new CheckCollision(mock1.Object, mock2.Object);

            Assert.Throws<Exception>(() => collision.Execute());
            DecisionStrategy.Verify();
        }
       
        [Test]
        public void NegativeCollision()
        {
            var mock1 = new Mock<IUObject>();
            var mock2 = new Mock<IUObject>();
            GetPropertyVelocity1(mock1, new List<int> { 2, 1 });
            GetPropertyPosition1(mock1, new List<int> { 1, 1 });
            GetPropertyVelocity2(mock2, new List<int> { 0, 1 });
            GetPropertyPosition2(mock2, new List<int> { 0, 1 });

            DecisionStrategy.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(false).Verifiable();
            CheckCollision collision = new CheckCollision(mock1.Object, mock2.Object);

            collision.Execute();
            DecisionStrategy.Verify();
        }
    }
}
