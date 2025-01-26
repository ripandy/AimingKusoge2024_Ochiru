using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Client;
using MagicOnionTest.Shared;
using UnityEngine;

namespace Network.Client
{
    public class GamingHubClient : MonoBehaviour, IGamingHubReceiver
    {
        private const string ServerUrl = "http://localhost:5000";
        
        private readonly CancellationTokenSource shutdownCancellation = new();
        
        private ChannelBase channel;
        private IGamingHub client;
        
        private readonly Dictionary<string, GameObject> players = new();
        private GameObject playerObject;
        
        private async void Start()
        {
            await InitializeClientAsync();
        }
        
        private async void OnDestroy()
        {
            // Clean up Hub and channel
            shutdownCancellation.Cancel();

            if (client != null) await client.DisposeAsync();
            if (channel != null) await channel.ShutdownAsync();
        }
        
        private async Task InitializeClientAsync()
        {
            // Initialize the Hub
            // NOTE: If you want to use SSL/TLS connection, see InitialSettings.OnRuntimeInitialize method.
            channel = GrpcChannelx.ForAddress(ServerUrl);

            while (!shutdownCancellation.IsCancellationRequested)
            {
                try
                {
                    Debug.Log("Connecting to the server...");
                    playerObject = await ConnectAsync(channel, "RoomName", "PlayerName");
                    Debug.Log("Connection is established.");
                    break;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                Debug.Log($"Failed to connect to the server. Retry after 5 seconds...");
                await Task.Delay(5 * 1000);
            }
        }

        public async ValueTask<GameObject> ConnectAsync(ChannelBase grpcChannel, string roomName, string playerName)
        {
            client = await StreamingHubClient.ConnectAsync<IGamingHub, IGamingHubReceiver>(grpcChannel, this);

            var roomPlayers = await client.JoinAsync(roomName, playerName, Vector3.zero, Quaternion.identity);
            foreach (var player in roomPlayers)
            {
                (this as IGamingHubReceiver).OnJoin(player);
            }

            return players[playerName];
        }

        // methods send to server.
        public ValueTask LeaveAsync()
        {
            return client.LeaveAsync();
        }

        public ValueTask MoveAsync(Vector3 position, Quaternion rotation)
        {
            return client.MoveAsync(position, rotation);
        }

        // dispose client-connection before channel.ShutDownAsync is important!
        public Task DisposeAsync()
        {
            return client.DisposeAsync();
        }

        // You can watch connection state, use this for retry etc.
        public Task WaitForDisconnect()
        {
            return client.WaitForDisconnect();
        }

        // Receivers of message from server.
        void IGamingHubReceiver.OnJoin(Player player)
        {
            Debug.Log("Join Player:" + player.Name);

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = player.Name;
            cube.transform.SetPositionAndRotation(player.Position, player.Rotation);
            players[player.Name] = cube;
        }

        void IGamingHubReceiver.OnLeave(Player player)
        {
            Debug.Log("Leave Player:" + player.Name);

            if (players.TryGetValue(player.Name, out var cube))
            {
                Destroy(cube);
            }
        }

        void IGamingHubReceiver.OnMove(Player player)
        {
            Debug.Log("Move Player:" + player.Name);

            if (players.TryGetValue(player.Name, out var cube))
            {
                cube.transform.SetPositionAndRotation(player.Position, player.Rotation);
            }
        }
    }
}