using LiteNetLib.Utils;

namespace NetworkShare
{
    public class Net_OpponentLeftResponse : INetPacketResponse
    {
        public PacketResponseType ResponseType => PacketResponseType.OpponentLeftResponse;

        public string Message { get; set; } = string.Empty;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)ResponseType);
            writer.Put(Message ?? string.Empty);
        }

        public void Deserialize(NetDataReader reader)
        {
            Message = reader.GetString();
        }
    }
}
