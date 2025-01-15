using Grpc.Net.Client;
using Google.Protobuf.Collections;
using Grpc.Core;
using Npgsql;
using gRPCclient;

using var channel = GrpcChannel.ForAddress("http://localhost:5103");
var client = new EndPoint.EndPointClient(channel);

var messages = new List<string>();
var connectionString = "Host=localhost;Username=postgres;Password=0000;Database=outbox";
await using var connection = new NpgsqlConnection(connectionString);
await connection.OpenAsync();
await using (var cmd = new NpgsqlCommand("SELECT serialized_message FROM transaction_messages WHERE is_sent=false", connection))
await using (var reader = await cmd.ExecuteReaderAsync())
{
    while (await reader.ReadAsync())
        messages.Add(reader.GetString(0));
}
foreach(string mes in messages){
    Dictionary<string, string> dict = new();
    foreach(string prop in mes.Split(';')){
        dict.Add(prop.Split('=')[0], prop.Split('=')[1]);
    }
    try{
        var gRPCMess = new ExternalCommandRequest();
        var propsMap = new MapField<string, string>{dict};
        gRPCMess.Map.Add(propsMap);
        gRPCMess.GameId = propsMap.First().Value;
        var reply = client.Command(gRPCMess);
        await using (var cmd = new NpgsqlCommand("UPDATE transaction_messages SET is_sent=true WHERE \"serialized_message\"=(@p)", connection))
        {
            cmd.Parameters.AddWithValue("p", mes);
            await cmd.ExecuteNonQueryAsync();
        }
    }
    catch {
        continue;
    }

}
   