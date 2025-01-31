using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using MagicOnion.Client;
using MagicOnionTest.Shared;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Network.Client
{
    public class GamingHubClient : IGamingHubReceiver
    {
        private IGamingHub client;
        
        private readonly Dictionary<string, GameObject> players = new();
        
        public async ValueTask<GameObject> ConnectAsync(ChannelBase grpcChannel, string roomName, string playerName)
        {
            client = await StreamingHubClient.ConnectAsync<IGamingHub, IGamingHubReceiver>(grpcChannel, this);
            await client.JoinAsync(roomName, playerName, Vector3.zero, Quaternion.identity);
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
                Object.Destroy(cube);
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