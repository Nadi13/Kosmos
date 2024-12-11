using Hwdtech;
using Hwdtech.Ioc;
using Moq;
using ShipGame.Server;

namespace Tests
{
    public class UObjectTest
    {
        public UObjectTest()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
            Dictionary<int, bool> transactionManager = new();
            int currentTransactionID = 0;
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "TransactionManager.GetManager", (object[] args) => transactionManager).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "TransactionManager.GetTransactionStatus", (object[] args) => {
                transactionManager.TryGetValue((int)args[0], out bool output);
                return (object)output;
            }).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "TransactionManager.GetCurrentTransactionID", (object[] args) => (object)currentTransactionID).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "TransactionManager.SetCurrentTransactionID", (object[] args) => {currentTransactionID = (int)args[0];
            return (object)currentTransactionID;}).Execute();

        }

        [Test]
        public void PositiveTest_UObject_CommitedGetValue()
        {
            Dictionary<int, bool> transactionManager = IoC.Resolve<Dictionary<int, bool>>("TransactionManager.GetManager");
            transactionManager[0] = true;
            Dictionary<string, ITransactionStatus> properties = new();
            var uobject = new UObject(properties);
            uobject.SetProperty("MoveVelocity", 69);
            Assert.True(Equals(69, uobject.GetProperty("MoveVelocity")));
        }
        [Test]
        public void NegativeTest_UObject_AbortedGetValue()
        {
            Dictionary<int, bool> transactionManager = IoC.Resolve<Dictionary<int, bool>>("TransactionManager.GetManager");
            Dictionary<string, ITransactionStatus> properties = new();
            transactionManager[0] = true;
            var uobject = new UObject(properties);
            uobject.SetProperty("MoveVelocity", 69);
            var currentTrunsactionNumber = IoC.Resolve<int>("TransactionManager.SetCurrentTransactionID", 1);
            uobject.SetProperty("MoveVelocity", 34);
            transactionManager[currentTrunsactionNumber] = false;
            Assert.True(Equals(69, uobject.GetProperty("MoveVelocity")));
        }
        [Test]
        public void PositiveTest_UObject_CommitedSetValue()
        {
            Dictionary<int, bool> transactionManager = IoC.Resolve<Dictionary<int, bool>>("TransactionManager.GetManager");
            Dictionary<string, ITransactionStatus> properties = new();
            transactionManager[0] = true;
            var uobject = new UObject(properties);
            uobject.SetProperty("MoveVelocity", 69);
            var currentTrunsactionNumber = IoC.Resolve<int>("TransactionManager.SetCurrentTransactionID", 1);
            uobject.SetProperty("MoveVelocity", 34);
            transactionManager[currentTrunsactionNumber] = true;
            Assert.True(Equals(34, uobject.GetProperty("MoveVelocity")));
        }
        [Test]
        public void NegativeTest_UObject_AbortedSetValue()
        {
            Dictionary<int, bool> transactionManager = IoC.Resolve<Dictionary<int, bool>>("TransactionManager.GetManager");
            transactionManager[0] = false;
            transactionManager[1] = true;
            Dictionary<string, ITransactionStatus> properties = new();
            var uobject = new UObject(properties);
                uobject.SetProperty("MoveVelocity", 69);
            IoC.Resolve<int>("TransactionManager.SetCurrentTransactionID", 1);
            uobject.SetProperty("MoveVelocity", 34);

            Assert.True(Equals(34, uobject.GetProperty("MoveVelocity")));
        }
        [Test]
        public void NegativeTest_UObject_NoKey_On_GetValue()
        {
            Dictionary<int, bool> transactionManager = IoC.Resolve<Dictionary<int, bool>>("TransactionManager.GetManager");
            Dictionary<string, ITransactionStatus> properties = new();
            var uobject = new UObject(properties);
            Assert.Throws<KeyNotFoundException>(()=>uobject.GetProperty("MoveVelocity"));
        }
    }
}