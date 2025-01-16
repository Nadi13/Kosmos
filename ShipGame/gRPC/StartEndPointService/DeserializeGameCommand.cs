using Hwdtech;
using ShipGame.Game;
using ShipGame.Server;
using ICommand = ShipGame.Move.ICommand;

public class DeserializeGameCommand : ICommand
{
    string _threadId;
    string _serializedString;

    public DeserializeGameCommand(string threadId, string serializedGame)
    {
        _threadId = threadId;
        _serializedString = serializedGame;
    }

    public void Execute()
    {
        var deserializedData = ParseSerializedString(_serializedString);
        var gameScope = CreateGameScope(deserializedData.Options, deserializedData.Objects, deserializedData.Timespan);
        var gameCommands = deserializedData.Commands;

        string idForNewGame = IoC.Resolve<string>("ThreadScope.GameId.New");
        ICommand newGameCommand = new SetScopeCommand(idForNewGame, gameScope, gameCommands);
        ISender senderToCurrentThreadQueue = IoC.Resolve<ISender>("MySender");
        senderToCurrentThreadQueue.Send(newGameCommand);
    }

    private (List<string> Options, Dictionary<string, object> Objects, Queue<ICommand> Commands, TimeSpan Timespan) ParseSerializedString(string serializedString)
    {
       
        var parts = serializedString.Split('|');

        var options = ParseOptions(parts[0]);
        var gameObjects = ParseGameObjects(parts[1]);
        var commands = ParseCommands(parts[2]);
        var timeSpan = IoC.Resolve<TimeSpan>("DeserializeTimespan", parts[3]);
        return (options, gameObjects, commands, timeSpan);
    }
    
    private List<string> ParseOptions(string optionsData)
    {
         return optionsData.Split(',')
            .Where(option => option.Contains("Scope"))
            .ToList();
    }
    
    private Dictionary<string, object> ParseGameObjects(string propertiesData)
    {
      return propertiesData.Split(';')
            .Where(property => property.Contains(':'))
            .Select(property => {
                var parts = property.Split(" : ", StringSplitOptions.RemoveEmptyEntries);
                var key = parts[0];
                var value = IoC.Resolve<object>("DeserializeValue", parts[1]);
                return (key,value);
            })
            .ToDictionary(x => x.key, x=> x.value);
    }
    private Queue<ICommand> ParseCommands(string commandData)
    {
        var commands = commandData.Split(',')
            .Where(command => command.Contains("type"))
            .Select(command => IoC.Resolve<ICommand>("DeserializeCommand", command))
            .ToList();

        return new Queue<ICommand>(commands);
    }

    private object CreateGameScope(List<string> gameOptions, Dictionary<string, object> gameObjects, TimeSpan timeSpan)
    {
        return IoC.Resolve<object>("Game.Scope.Create", gameOptions, gameObjects, timeSpan);
    }
}
