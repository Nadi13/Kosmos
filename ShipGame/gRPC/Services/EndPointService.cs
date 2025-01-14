using Grpc.Core;
using GRpc.Server;
using Hwdtech;
using ICommand = ShipGame.Move.ICommand;

namespace gRPC.Services
{
    public class EndPointService : EndPoint.EndPointBase
    {
        private GRpc.Server.IEndPointRouter router;
        public EndPointService(GRpc.Server.IEndPointRouter router)
        {
            this.router = router;
        }

        public override Task<CommandResponse> Message(CommandRequest request, ServerCallContext context)
        {
            string gameId = request.GameId;
            var cmd = IoC.Resolve<ICommand>("CreateCommandByNameForObject", request);
            var threadID = IoC.Resolve<string>("Storage.GetThreadByGameID", gameId);
            IoC.Resolve<ICommand>("SendCommandByThreadID", threadID, cmd).Execute();
            return Task.FromResult(new CommandResponse
            {
                Status = 202
            });
        }
        public override Task<ExternalCommandReply> Command(ExternalCommandRequest request, ServerCallContext context)
        {
            var r = router.isSent(request);
            return Task.FromResult(new ExternalCommandReply{Status = r});
        }

        public override Task<NewGameStatusReply> MigrateGame(SendGameToAnotherServerRequest request, ServerCallContext context)
        {
            var r = router.isMigrate(request);
            return Task.FromResult(new NewGameStatusReply{GameStatus = r});
        }

        public override Task<AcceptStatusReply> AcceptGame(SerializedGameRequest request, ServerCallContext context)
        {
            var r = router.isAccept(request);
            return Task.FromResult(new AcceptStatusReply{AcceptStatus = r});
        }
    }
}
