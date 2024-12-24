namespace gRPC.Commands;

using gRPC;
using Hwdtech;
using ICommand = ShipGame.Move.ICommand;
using IStrategy = ShipGame.Move.IStrategy;


class MakeCommandByOrderRequestCommand : ICommand
{
    string _gameId;
    Google.Protobuf.Collections.MapField<string, string> _orderMap;
    public MakeCommandByOrderRequestCommand(OrderRequest request){
        _gameId = request.GameId;
        _orderMap = request.Map;
    }
    public void Execute()
    {
        Dictionary<string,string> orderProperties = _orderMap.ToDictionary(x => x.Key, x => x.Value);
        ICommand command = IoC.Resolve<ICommand>("OrderDictionaryToICommand", orderProperties);
        IoC.Resolve<ICommand>("SendCommandToGame", _gameId, command).Execute();
    }
}
