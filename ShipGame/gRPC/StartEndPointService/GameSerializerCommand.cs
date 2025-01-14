namespace gRPC.StartEndPointService;
using System;
using System.Collections.Generic;
using System.Linq;
using Hwdtech;
using ICommand = ShipGame.Move.ICommand;

public class GameSerializerCommand: ICommand
{
    string gameId;
    string newServerId;
    string serializedString = "";

    public GameSerializerCommand(string gameId, string newServerId)
    {
      this.gameId = gameId;
      this.newServerId = newServerId;
    }

    public void Execute()
    {
        StopGame();

        var gameData = GetGameData();

        serializedString = SerializeGameData(gameData);
         
        SendSerializedData();
    }


    private void StopGame()
    {
       IoC.Resolve<ICommand>("TerminateGameCommand").Execute();
    }

    private (List<string> options, Dictionary<string, object> objects, Queue<ICommand> queue, TimeSpan timespan) GetGameData()
    {
      var gameOptions = IoC.Resolve<List<string>>("Game.Options.GetAll", gameId);
      var gameObjects = IoC.Resolve<Dictionary<string, object>>("Game.Objects.GetAll", gameId);
      var gameQueue = IoC.Resolve<Queue<ICommand>>("Game.Queue.Get", gameId);
      var timespan = IoC.Resolve<TimeSpan>("Game.Get.Timespan", gameId);

      return (gameOptions, gameObjects, gameQueue, timespan);
    }


    private string SerializeGameData((List<string> options, Dictionary<string, object> objects, Queue<ICommand> queue, TimeSpan timespan) gameData)
    {
          
        string serializedOptions = string.Join("", gameData.options.Select(option => IoC.Resolve<string>("SerializeOption", option)));
        string serializedObjects = string.Join(";", gameData.objects.Select(entry => $"{entry.Key} : {IoC.Resolve<string>("SerializeObject", entry.Value)}"));
        string serializedCommands = string.Join("", gameData.queue.ToArray().Select(cmd => IoC.Resolve<string>("SerializeCommand", cmd)));

        return $"{serializedOptions} | {serializedObjects} | {serializedCommands} | {gameData.timespan}";
     }

    private void SendSerializedData()
    {
      IoC.Resolve<ICommand>("EndPointClientCall", newServerId, serializedString).Execute();
    }
}
