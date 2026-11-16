using LiteNetLib.Utils;
using LiteNetLib;
namespace NetworkShare
{
    public enum PacketType
    {
        #region Client to Server
        Invalid = 0,
        AuthRequest = 1,
        #endregion
        #region Server to Client
        AuthResponse = 100,
        #endregion
    }

    public interface INetPacket : INetSerializable
    {
        PacketType Type { get; }
    }
    public interface IPacketHandler
    {
        void HandlePacket(INetPacket packet, NetPeer peer);
    }
}
