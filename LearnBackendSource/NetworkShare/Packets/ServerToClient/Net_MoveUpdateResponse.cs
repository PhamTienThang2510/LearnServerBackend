using LiteNetLib.Utils;

namespace NetworkShare
{
    public class Net_MoveUpdateResponse : INetPacketResponse
    {
        public PacketResponseType ResponseType => PacketResponseType.MoveUpdateResponse;

        public int CellIndex { get; set; }
        public byte Symbol { get; set; } // 1 = X, 2 = O
        public byte NextTurnSymbol { get; set; } // 1 = X, 2 = O

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)ResponseType);
            writer.Put(CellIndex);
            writer.Put(Symbol);
            writer.Put(NextTurnSymbol);
        }

        public void Deserialize(NetDataReader reader)
        {
            CellIndex = reader.GetInt();
            Symbol = reader.GetByte();
            NextTurnSymbol = reader.GetByte();
        }
    }
}
