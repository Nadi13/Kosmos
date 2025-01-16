using gRPC;

namespace GRpc.Server {
    public interface IEndPointRouter {
        public bool isSent(ExternalCommandRequest externalCommandRequest);
        public bool isMigrate(SendGameToAnotherServerRequest sendGameToAnotherServerRequest);
        public bool isAccept(string serializedGame);
    }
}
