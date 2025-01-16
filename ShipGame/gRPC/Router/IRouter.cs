using gRPC;

namespace GRpc.Server {
    public interface IEndPointRouter {
        public bool isSent(ExternalCommandRequest externalCommandRequest);
    }
}
