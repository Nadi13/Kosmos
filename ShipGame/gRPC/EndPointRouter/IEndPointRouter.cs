namespace gRPC.EndPointRouter;

public interface IEndPointRouter
{
    public bool route(OrderRequest orderRequest);
}
