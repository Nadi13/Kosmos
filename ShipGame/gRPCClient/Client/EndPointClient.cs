using Grpc.Net.Client;

namespace gRPCclient;

public class EndPointClient
{
    public static void Call(string ip, string message)
    {
        using var channel = GrpcChannel.ForAddress(ip);
        var client = new EndPoint.EndPointClient(channel);
        var response = client.AcceptGame(new SerializedGameRequest{SerializedGame = message});
        Console.WriteLine(response);
    }
}