using LiteNetLib.Utils;
using LiteNetLib;

namespace NetworkShare
{
    public enum PacketType
    {
        Invalid = 0,
        AuthRequest = 1,
        FindMatchRequest = 2,
        PlayerMoveRequest = 3,
        RestartMatchRequest = 4,
    }

    public enum PacketResponseType
    {
        Invalid = 0,
        AuthResponse = 1,
        MatchStartResponse = 2,
        MoveUpdateResponse = 3,
        GameOverResponse = 4,
        OpponentLeftResponse = 5,
    }

    public interface INetPacket : INetSerializable
    {
        PacketType Type { get; }
    }

    public interface INetPacketResponse : INetSerializable
    {
        PacketResponseType ResponseType { get; }
    }
}
