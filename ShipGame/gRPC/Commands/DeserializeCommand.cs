using Hwdtech;
using ICommand = ShipGame.Move.ICommand;
using ShipGame.Game;
using ShipGame.Server;

namespace gRPC.Commands
{
    public class DeserializeCommand : ICommand
    {
        private string threadId;
        private string serializedString;

        public DeserializeCommand(string threadId, string serializedGame)
        {
            this.threadId = threadId;
            serializedString = serializedGame;
        }

        public void Execute()
        {
            string[] serializedData = serializedString.Split('|');
            List<string> gameOptions = new();
            Dictionary<string, object> gameObjects = new();
            Queue<ICommand> gameQueue = new();

            foreach(string optionData in serializedData[0].Split(',')){
                if(optionData.Contains("Scope"))
                {
                    gameOptions.Add(optionData);
                }
            }

            foreach(string propertyData in serializedData[1].Split(';')){
                if (propertyData.Contains(':'))
                {
                    string key = propertyData.Split(" : ")[0];
                    string stringValue = propertyData.Split(" : ")[1];

                    object objectValue = IoC.Resolve<object>("DeserializeValue", stringValue);

                    gameObjects[key] = objectValue;
                }

            }

            foreach(string commandData in serializedData[2].Split(',')){
                if(commandData.Contains("type"))
                {
                    ICommand deserializedCommand = IoC.Resolve<ICommand>("DeserializeCommand", commandData);

                    gameQueue.Enqueue(deserializedCommand);
                }

            }
            TimeSpan timespan = IoC.Resolve<TimeSpan>("DeserializeTimespan", serializedData[3]);

            var gameScope = IoC.Resolve<object>("Game.Scope.Create", gameOptions, gameObjects, timespan);
            string idForNewGame = IoC.Resolve<string>("ThreadScope.GameId.New");
            ICommand newGameCommand = new SetScopeCommand(idForNewGame, gameScope, gameQueue);
            ISender senderToCurrentThreadQueue = IoC.Resolve<ISender>("MySender");
            senderToCurrentThreadQueue.Send(newGameCommand);
        }
    }
}
