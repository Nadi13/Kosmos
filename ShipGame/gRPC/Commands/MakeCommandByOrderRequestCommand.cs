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
        Dictionary<string,string> orderProperties = IoC.Resolve<Dictionary<string,string>>("ProtobufMapToDictionary", _orderMap);
        ICommand command = IoC.Resolve<ICommand>("OrderDictionaryToICommand", orderProperties);
        IoC.Resolve<IStrategy>("SendCommandToGame", _gameId, command).RunStrategy();
    }
}
