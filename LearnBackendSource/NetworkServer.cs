using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;
using LearnBackendSource.Game;
using LearnBackendSource.Handlers;
using NetworkShare;

namespace LearnBackendSource
{
    public class NetworkServer : INetEventListener
    {
        private NetManager? _server;
        private bool _isRunning;
        private readonly RoomManager _roomManager = new RoomManager();

        public void Start(int port = 9050, string key = "")
        {
            _server = new NetManager(this);
            _server.Start(port);
            _isRunning = true;

            Console.WriteLine($"[Server] Started on port {port}. Waiting for connections...");

            while (_isRunning)
            {
                _server.PollEvents();
                Thread.Sleep(15);
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _server?.Stop();
            Console.WriteLine("[Server] Stopped.");
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Console.WriteLine($"[Server] Peer connected from {peer.Address}:{peer.Port} (Peer ID: {peer.Id})");
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Console.WriteLine($"[Server] Peer disconnected: {peer.Address}, Reason: {disconnectInfo.Reason}");
            _roomManager.OnPeerDisconnected(peer);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Console.WriteLine($"[Server] Network error from {endPoint}: {socketError}");
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            try
            {
                var packetType = (PacketType)reader.GetByte();
                switch (packetType)
                {
                    case PacketType.AuthRequest:
                        var request = new Net_AuthRequest();
                        request.Deserialize(reader);

                        var response = AuthRequestHandler.HandleAuthRequest(peer, request);
                        var writer = new NetDataWriter();
                        response.Serialize(writer);
                        peer.Send(writer, DeliveryMethod.ReliableOrdered);
                        break;

                    case PacketType.FindMatchRequest:
                        _roomManager.EnqueueMatchmaking(peer);
                        break;

                    case PacketType.PlayerMoveRequest:
                        var moveRequest = new Net_PlayerMoveRequest();
                        moveRequest.Deserialize(reader);
                        _roomManager.HandlePlayerMove(peer, moveRequest.CellIndex);
                        break;

                    case PacketType.RestartMatchRequest:
                        _roomManager.HandleRestartMatch(peer);
                        break;

                    default:
                        Console.WriteLine($"[Server] Unknown packet type received: {packetType}");
                        break;
                }
            }
            finally
            {
                reader.Recycle();
            }
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            Console.WriteLine($"[Server] Connection request from {request.RemoteEndPoint}");
            // Chấp nhận kết nối từ mọi client
            request.Accept();
        }
    }
}
