using LiteNetLib;
using LiteNetLib.Utils;
using NetworkShare;
namespace NetworkShare
{
    public class Net_AuthResponse : INetPacketResponse
    {
        public PacketType ResponseType => PacketType.AuthResponse;

        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)ResponseType);
            writer.Put(IsSuccess);
            writer.Put(Message ?? string.Empty);
        }
        public void Deserialize(NetDataReader reader)
        {
            IsSuccess = reader.GetBool();
            Message = reader.GetString();
        }
    }
}