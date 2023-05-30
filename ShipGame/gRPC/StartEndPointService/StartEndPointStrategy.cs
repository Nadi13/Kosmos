namespace gRPC.StartEndPointService;
using ShipGame.Move;
    public class StartEndPointStrategy: IStrategy
    {
    public object RunStrategy(params object[] args)
    {
        return new StartEndPointCommand();
    }

}

