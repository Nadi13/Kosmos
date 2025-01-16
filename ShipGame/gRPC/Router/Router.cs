using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using gRPC;
using gRPC.StartEndPointService;
using ShipGame.Server;

namespace GRpc.Server
{
    public class RouterMessage: IEndPointRouter
    {
        ConcurrentDictionary<string, string> _threadIdByGameIdDictionary;
        ConcurrentDictionary<string, ISender> _senderByThreadIdDictionary;
        Random random = new Random();

        public RouterMessage(ConcurrentDictionary<string, string> threadIdByGameIdDictionary, ConcurrentDictionary<string, ISender> senderByThreadIdDictionary){
            _threadIdByGameIdDictionary = threadIdByGameIdDictionary;
            _senderByThreadIdDictionary = senderByThreadIdDictionary;
        }
        
        public bool isSent(ExternalCommandRequest externalCommandRequest) 
        {
            try 
            {
                var message = convertMessage(externalCommandRequest);
                var gameId = externalCommandRequest.GameId;
                var threadId = _threadIdByGameIdDictionary[gameId];
                var sender = _senderByThreadIdDictionary[threadId];
                ShipGame.Move.ICommand command = new Command(gameId, message);
                sender.Send(command);
                return true;
            }
            catch
            {
                return false;
            }     
        }

        public Dictionary<string, string> convertMessage(ExternalCommandRequest externalCommandRequest) 
        {
            var dictionary = new Dictionary<string, string>();
        
            foreach (var pair in externalCommandRequest.Map)
            {
                dictionary.Add(pair.Key, pair.Value);
            }

            return dictionary;
        }

        public bool isMigrate(SendGameToAnotherServerRequest sendGameToAnotherServerRequest)
        {
            try 
            {
                var gameId = sendGameToAnotherServerRequest.GameId;
                var serverId = sendGameToAnotherServerRequest.NewServerId;
                var message = new GameSerializerCommand(gameId, serverId);
                var threadId = _threadIdByGameIdDictionary[gameId];
                var sender = _senderByThreadIdDictionary[threadId];
                sender.Send(message);
                return true;
            }
            catch
            {
                return false;
            }    
        }

        public bool isAccept(string serializedGame)
        {
            try 
            {
                string threadId = _threadIdByGameIdDictionary.ElementAt(random.Next(0, _threadIdByGameIdDictionary.Count)).Value;
                var message = new DeserializeGameCommand(threadId, serializedGame);
                var sender = _senderByThreadIdDictionary[threadId];
                sender.Send(message);
                return true;
            }
            catch
            {
                return false;
            }   
        }
    }
}
