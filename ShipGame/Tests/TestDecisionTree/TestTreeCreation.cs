using Hwdtech;
using Moq;
using Hwdtech.Ioc;
using ShipGame.DecisionTree;
using ShipGame.Move;

namespace Tests.TestDecisionTree
{
    public class TestTreeCreation
    {

        Mock<IStrategy> DecisionStrategy = new Mock<IStrategy>();
        public TestTreeCreation()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Create.Tree", (object[] args) => DecisionStrategy.Object.RunStrategy(args)).Execute();
        }


        [Test]
        public void PositiveDecisionTreeTest()
        {
            string path = "../../../InitialData.txt";
            DecisionStrategy.Setup(c => c.RunStrategy(It.IsAny<object[]>())).Returns(new Dictionary<int, object>()).Verifiable();
            var mock = new TreeCreation(path);
            mock.Execute();
            DecisionStrategy.Verify();
        }

        [Test]
        public void FileNotFoundException()
        {
            string path = "MyFile.txt";
            DecisionStrategy.Setup(c => c.RunStrategy(It.IsAny<object[]>())).Returns(new Dictionary<int, object>()).Verifiable();
            var mock = new TreeCreation(path);
            Assert.Throws<FileNotFoundException>(() => mock.Execute());
            DecisionStrategy.Verify();
        }
        
        [Test]
        public void Exception()
        {
            string path = "";
            DecisionStrategy.Setup(c => c.RunStrategy(It.IsAny<object[]>())).Returns(new Dictionary<int, object>()).Verifiable();
            var mock = new TreeCreation(path);
            Assert.Throws<Exception>(() => mock.Execute());
            DecisionStrategy.Verify();
        }
    }
}
