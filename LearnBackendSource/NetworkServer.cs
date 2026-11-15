using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;

namespace LearnBackendSource
{
    public class NetworkServer : INetEventListener
    {
        private NetManager? _server;
        private bool _isRunning;

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
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Console.WriteLine($"[Server] Network error from {endPoint}: {socketError}");
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            byte[] bytes = reader.GetRemainingBytes();
            string msg = System.Text.Encoding.UTF8.GetString(bytes);
            Console.WriteLine($"[Server] Received from peer {peer.Id}: {msg}");

            // Gửi phản hồi lại cho Client
            NetDataWriter writer = new NetDataWriter();
            writer.Put($"[Server Response]: Server da nhan duoc '{msg}'!");
            peer.Send(writer, DeliveryMethod.ReliableOrdered);

            reader.Recycle();
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
