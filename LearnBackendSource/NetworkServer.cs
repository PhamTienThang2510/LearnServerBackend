using System;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using Microsoft.Extensions.DependencyInjection;
using NetworkShare;
using LearnBackendSource.Packets;
using LearnBackendSource.Handlers;

namespace LearnBackendSource
{
    public class NetworkServer : INetEventListener
    {
        private NetManager? _server;
        private bool _isRunning;
        private readonly IServiceProvider _serviceProvider;

        public NetworkServer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

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
            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    var packetType = (PacketType)reader.GetByte();
                    var packet = ResolvePacket(packetType, reader);
                    var handler = ResolveHandler(scope.ServiceProvider, packetType);
                    handler.HandlePacket(packet, peer);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Server] Error processing packet: {ex.Message}");
                }
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

        private INetPacket ResolvePacket(PacketType packetType, NetPacketReader reader)
        {
            var packetRegistry = _serviceProvider.GetRequiredService<PacketRegistry>();
            if (packetRegistry.PacketTypes.TryGetValue(packetType, out var packetTypeClass))
            {
                var packet = (INetPacket)Activator.CreateInstance(packetTypeClass)!;
                packet.Deserialize(reader);
                return packet;
            }

            throw new InvalidOperationException($"Chưa đăng ký PacketType {packetType} trong PacketRegistry.");
        }

        private IPacketHandler ResolveHandler(IServiceProvider provider, PacketType packetType)
        {
            return packetType switch
            {
                PacketType.AuthRequest => provider.GetRequiredService<AuthRequestHandler>(),
                _ => throw new NotImplementedException($"No handler implementation for {packetType}")
            };
        }
    }
}
