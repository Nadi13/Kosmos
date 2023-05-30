namespace gRPC.StartEndPointService;
using Microsoft.AspNetCore.Builder;
using ShipGame.Move;
using gRPC.Services;

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

