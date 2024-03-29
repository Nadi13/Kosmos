using Grpc.Core;
using Hwdtech;
using ICommand = ShipGame.Move.ICommand;

namespace gRPC.Services
{
    public class EndPointService : EndPoint.EndPointBase
    {
        private readonly ILogger<EndPointService> _logger;
        public EndPointService(ILogger<EndPointService> logger)
        {
            _logger = logger;
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
    }
}
