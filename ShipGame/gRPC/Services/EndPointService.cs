using gRPC.EndPointRouter;
using Grpc.Core;
using Hwdtech;
using ICommand = ShipGame.Move.ICommand;


namespace gRPC.Services
{
    public class EndPointService : EndPoint.EndPointBase
    {
        private readonly ILogger<EndPointService> _logger;
        private IEndPointRouter _router;
        public EndPointService(ILogger<EndPointService> logger, IEndPointRouter endPointRouter)
        {
            _logger = logger;
            _router = endPointRouter;
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
        public override async Task<OrderReply> Order(IAsyncStreamReader<OrderRequest> requestStream, IServerStreamWriter<OrderReply> responseStream, ServerCallContext context)
        {
            _logger.LogInformation("Start processing Order stream");
            try{
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    bool isRouted = _router.route(message);
                    await responseStream.WriteAsync(new OrderReply(){Status = isRouted});
                }
            }
            catch (Exception ex){
                _logger.LogError(ex, "Error processing Order stream");
            }
            
            _logger.LogInformation("End processing Order stream");
            return new OrderReply();
        }
        public async override Task<NewGameStatus> MigrateGame(GameStatus request, ServerCallContext context)
        {
            string gameId = request.GameId;
            bool isRouted = _router.routeMigrateCommand(request.NewServerId, gameId);
            return await Task.FromResult(new NewGameStatus
            {
                GameStatus = isRouted
            });
        }

        public override Task<AcceptStatus> AcceptGame(SerializedGameMessage request, ServerCallContext context)
        {
            string serializedGame = request.SerializedGame;
            bool isRouted = _router.routeAcceptCommand(serializedGame);
            return Task.FromResult(new AcceptStatus
            {
                AcceptStatus_ = isRouted
            });
        }

    }
}
