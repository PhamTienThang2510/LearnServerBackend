using LiteNetLib.Utils;

namespace NetworkShare
{
    public class Net_FindMatchRequest : INetPacket
    {
        public PacketType Type => PacketType.FindMatchRequest;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)Type);
        }

        public void Deserialize(NetDataReader reader)
        {
        }
    }
}
