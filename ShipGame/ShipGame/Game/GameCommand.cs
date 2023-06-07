using System.Diagnostics;
using Hwdtech;
using ICommand = ShipGame.Move.ICommand;
namespace ShipGame.Game
{
    public class GameCommand: ICommand
    {
        Queue<ICommand> queue;
        string gameId;
        Stopwatch stopwatch = new Stopwatch();

        public GameCommand(string gameId, Queue<ICommand> queue)
        {
            this.gameId = gameId;
            this.queue = queue;
        }
        public void Execute()
        {
            var quantumOfTime = IoC.Resolve<TimeSpan>("QuantumForGame");
            stopwatch.Restart();
            while (stopwatch.Elapsed < quantumOfTime)
            {
                if (queue.Count() == 0)
                    break;
                var command = queue.Dequeue();
                try
                {
                    command!.Execute();
                }
                catch (Exception exception)
                {
                    IoC.Resolve<ICommand>("HandleException", exception, command!).Execute();
                }
            }
            stopwatch.Stop();
        }
    }
}
