﻿using Hwdtech.Ioc;
using Hwdtech;
using System.Collections.Concurrent;
using Moq;
using ShipGame.Game;
using ICommand = ShipGame.Move.ICommand;
using System.Diagnostics;
using ShipGame.Move;
using ShipGame.Server;
using SpaceBattle.ServerStrategies;

namespace Tests.TestGameCommand
{
    public class TestsGameCommand
    {
        public TestsGameCommand()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            var initialScope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", initialScope).Execute();

            var threadDict = new ConcurrentDictionary<string, ServerThread>();
            var senderDict = new ConcurrentDictionary<string, ISender>();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SenderDictionary", (object[] _) => { return senderDict; }).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ReturnInitialScope", (object[] _) => { return initialScope; }).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadDictionary", (object[] _) => { return threadDict; }).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadIDSenderMapping", (object[] _) => senderDict).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SenderAdapterGetByID", (object[] id) => senderDict[(string)id[0]]).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ServerThreadGetByID", (object[] id) => threadDict[(string)id[0]]).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SenderGetByID", (object[] id) => senderDict[(string)id[0]]).Execute();
            

            var createWithStartThreadStrategy = new CreateWithStartThreadStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateWithStartThread", (object[] args) => createWithStartThreadStrategy.RunStrategy(args)).Execute();

            var sendCommandStrategy = new SendCommandStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SendCommand", (object[] args) => sendCommandStrategy.RunStrategy(args)).Execute();

            var threadgamedict = new ConcurrentDictionary<string, string>();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Storage.ThreadByGameID", (object[] args) => threadgamedict).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Storage.GetThreadByGameID", (object[] args) =>
            {
                var dict = IoC.Resolve<ConcurrentDictionary<string, string>>("Storage.ThreadByGameID");
                return dict[(string)args[0]];
            }
            ).Execute();
        }
        [Test]
        public void GameCommandWithoutExceptionSuccessfulTest()
        {
            var senderDict = IoC.Resolve<ConcurrentDictionary<string, ISender>>("SenderDictionary");
            var threadGameDict = IoC.Resolve<ConcurrentDictionary<string, string>>("Storage.ThreadByGameID");
            var scopeGameDict = new ConcurrentDictionary<string, object>();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Storage.ScopeByGameID", (object[] args) => scopeGameDict).Execute();
            var sendCommandByThreadID = new SendCommandByThreadIDStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SendCommandByThreadID", (object[] args) => sendCommandByThreadID.RunStrategy(args)).Execute();
            var scopesByGameid = IoC.Resolve<ConcurrentDictionary<string, object>>("Storage.ScopeByGameID");
            var threadScope = IoC.Resolve<object>("ReturnInitialScope");
            var mre1 = new ManualResetEvent(false);
            var gameScope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
            scopesByGameid.TryAdd("game1", gameScope);
            var handleExceptionStrategy = new HandlStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "HandleException", (object[] args) => handleExceptionStrategy.RunStrategy(args)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadScope.Current", (object[] args) =>
            {
                var scopes = IoC.Resolve<ConcurrentDictionary<string, object>>("Storage.ScopeByGameID");
                return scopes[(string)args[0]];
            }).Execute();

            var quantum = new TimeSpan(0, 0, 0, 0, 100);
            var quantumStrategy = new Mock<IStrategy>();
            quantumStrategy.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(quantum);

            var th1 = IoC.Resolve<ServerThread>("CreateWithStartThread", "thread1");
            IoC.Resolve<ICommand>("SendCommandByThreadID", "thread1", new ActionCommand(() => {
                IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", gameScope).Execute();
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "QuantumForGame", (object[] args) => quantumStrategy.Object.RunStrategy()).Execute();
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Storage.ScopeByGameID", (object[] args) => scopeGameDict).Execute();
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SendCommandByThreadIDStrategy", (object[] args) => sendCommandByThreadID.RunStrategy(args)).Execute();
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "HandleException", (object[] args) => handleExceptionStrategy.RunStrategy(args)).Execute();
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Storage.GetThreadByGameID", (object[] args) =>
                {
                    return threadGameDict[(string)args[0]];
                }
                ).Execute();
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadScope.Current", (object[] args) =>
                {
                    return scopesByGameid[(string)args[0]];
                }
                ).Execute();
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SenderAdapterGetByID", (object[] id) => senderDict[(string)id[0]]).Execute();
                IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", threadScope).Execute();
                mre1.Set();
            })).Execute();
            mre1.WaitOne();
            var queue = new Queue<ICommand>();
            var repeatGameCommand = new SetScopeCommand("game1", gameScope, queue);
            var games = IoC.Resolve<ConcurrentDictionary<string, string>>("Storage.ThreadByGameID");
            games.TryAdd("game1", "thread1");
            var mre2 = new ManualResetEvent(false);
            IoC.Resolve<ICommand>("SendCommandByThreadID", "thread1", repeatGameCommand).Execute();
            IoC.Resolve<ICommand>("SendCommandByThreadID", "thread1", new ActionCommand(() => { mre2.Set(); })).Execute();
            Assert.False(th1.QueueIsEmpty());
            mre2.WaitOne();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            repeatGameCommand.Execute();
            stopwatch.Stop();
            Assert.True(stopwatch.ElapsedMilliseconds < 150);
        }
    }
}
