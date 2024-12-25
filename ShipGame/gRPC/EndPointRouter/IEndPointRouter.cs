namespace gRPC.EndPointRouter;

public interface IEndPointRouter
{
    public bool route(OrderRequest orderRequest);
    public bool routeMigrateCommand(string serverId, string gameId);
    public bool routeAcceptCommand(string serializedGame);
}
