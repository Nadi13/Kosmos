using System.Collections.Concurrent;
using ShipGame.Move;
using ShipGame.Server;
using gRPC.EndPointRouter;
using gRPC.Commands;

namespace gRPC.Router
{
    public class EndPointRouter : IEndPointRouter
    {
        ConcurrentDictionary<string, string> _threadIdByGameIdDictionary;
        ConcurrentDictionary<string, ISender> _senderByThreadIdDictionary;
        public EndPointRouter(ConcurrentDictionary<string, string> threadIdByGameIdDictionary, ConcurrentDictionary<string, ISender> senderByThreadIdDictionary){
            _threadIdByGameIdDictionary = threadIdByGameIdDictionary;
            _senderByThreadIdDictionary = senderByThreadIdDictionary;
        }
        public bool route(OrderRequest orderRequest)
        {
            try
            {
                string threadId = _threadIdByGameIdDictionary[orderRequest.GameId];
                ISender sender = _senderByThreadIdDictionary[threadId];
                ICommand command = new MakeCommandByOrderRequestCommand(orderRequest);
                sender.Send(command);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
