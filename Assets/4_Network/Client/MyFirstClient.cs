using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
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
        
        private readonly GamingHubClient gamingHubClient = new();
        private readonly CancellationTokenSource shutdownCancellation = new();

        private ChannelBase channel;
        private GameObject playerObject;
        
        private async void Start()
        {
            await InitializeClientAsync();
        }
        
        private async void OnDestroy()
        {
            // Clean up Hub and channel
            shutdownCancellation.Cancel();

            if (gamingHubClient != null) await gamingHubClient.DisposeAsync();
            if (channel != null) await channel.ShutdownAsync();
        }
        
        private async Task InitializeClientAsync()
        {
            // Connect to the server using gRPC channel.
            channel = GrpcChannelx.ForAddress(ServerUrl);

            // NOTE: If your project targets non-.NET Standard 2.1, use `Grpc.Core.Channel` class instead.
            // var channel = new Channel("localhost", 5001, new SslCredentials());

            // Create a proxy to call the server transparently.
            var client = MagicOnionClient.Create<IMyFirstService>(channel);

            // Call the server-side method using the proxy.
            var val1 = Random.Range(0, 1000);
            var val2 = Random.Range(0, 1000);
            var result = await client.SumAsync(val1, val2);
            Debug.Log($"[{GetType().Name}] val1: {val1} + val2: {val2} = Result: {result}");
            
            while (!shutdownCancellation.IsCancellationRequested)
            {
                try
                {
                    Debug.Log("Connecting to the server...");
                    playerObject = await gamingHubClient.ConnectAsync(channel, "RoomName", "PlayerName");
                    Debug.Log("Connection is established.");
                    break;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                Debug.Log("Failed to connect to the server. Retry after 5 seconds...");
                await Task.Delay(5000);
            }
        }
    }
}