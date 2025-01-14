using Hwdtech;

namespace GRpc.Server
{
    public class Command: ShipGame.Move.ICommand
    {
        string gameId; 
        Dictionary<string, string> message;

        public Command(string idGame, Dictionary<string, string> map)
        {
            gameId = idGame;
            message = map;
        }
        public void Execute()
        {
            ShipGame.Move.ICommand command = IoC.Resolve<ShipGame.Move.ICommand>("messageToICommand", message);
            var sendCommand = IoC.Resolve<ShipGame.Move.ICommand>("GameQueueReceiveCommand", gameId, command);
            sendCommand.Execute();
        }
    }
}
