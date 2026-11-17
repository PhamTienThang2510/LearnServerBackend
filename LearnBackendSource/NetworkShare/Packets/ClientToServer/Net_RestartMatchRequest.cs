using LiteNetLib.Utils;

namespace NetworkShare
{
    public class Net_RestartMatchRequest : INetPacket
    {
        public PacketType Type => PacketType.RestartMatchRequest;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)Type);
        }

        public void Deserialize(NetDataReader reader)
        {
        }
    }
}
