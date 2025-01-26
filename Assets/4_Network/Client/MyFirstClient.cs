using MagicOnion;
using MagicOnion.Client;
using MagicOnionTest.Shared;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Network.Client
{
    public class MyFirstClient : MonoBehaviour
    {
        private const string ServerUrl = "http://localhost:5000";
        
        private async void Start()
        {
            // Connect to the server using gRPC channel.
            var channel = GrpcChannelx.ForAddress(ServerUrl);

            // NOTE: If your project targets non-.NET Standard 2.1, use `Grpc.Core.Channel` class instead.
            // var channel = new Channel("localhost", 5001, new SslCredentials());

            // Create a proxy to call the server transparently.
            var client = MagicOnionClient.Create<IMyFirstService>(channel);

            // Call the server-side method using the proxy.
            var val1 = Random.Range(0, 1000);
            var val2 = Random.Range(0, 1000);
            var result = await client.SumAsync(val1, val2);
            Debug.Log($"[{GetType().Name}] val1: {val1} + val2: {val2} = Result: {result}");
        }
    }
}