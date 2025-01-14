namespace gRPC.StartEndPointService;
using gRPC.Services;
using ShipGame.Move;

public class StartEndPointCommand: ICommand
{
    WebApplication app;

    public StartEndPointCommand()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddGrpc();

        this.app = builder.Build();

        app.MapGrpcService<EndPointService>();
    }

    public void Execute() => this.app.Run();
}

