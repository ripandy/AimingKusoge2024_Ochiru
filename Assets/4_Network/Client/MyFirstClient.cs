using Cysharp.Net.Http;
using Grpc.Net.Client;
using MagicOnion.Client;
using MagicOnionTest.Shared;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Network.Client
{
    public class MyFirstClient : MonoBehaviour
    {
        private async void Start()
        {
            // Connect to the server using gRPC channel.
            var handler = new YetAnotherHttpHandler { Http2Only = true };
            var channelOptions = new GrpcChannelOptions { HttpHandler = handler };
            var channel = GrpcChannel.ForAddress("http://localhost:5000", channelOptions);

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