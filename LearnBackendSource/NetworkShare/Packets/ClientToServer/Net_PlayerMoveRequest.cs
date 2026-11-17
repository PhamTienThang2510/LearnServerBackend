using LiteNetLib.Utils;

namespace NetworkShare
{
    public class Net_PlayerMoveRequest : INetPacket
    {
        public PacketType Type => PacketType.PlayerMoveRequest;
        public int CellIndex { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)Type);
            writer.Put(CellIndex);
        }

        public void Deserialize(NetDataReader reader)
        {
            CellIndex = reader.GetInt();
        }
    }
}
