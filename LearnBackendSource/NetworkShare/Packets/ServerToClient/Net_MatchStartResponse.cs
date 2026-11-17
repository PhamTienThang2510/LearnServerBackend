using LiteNetLib.Utils;

namespace NetworkShare
{
    public class Net_MatchStartResponse : INetPacketResponse
    {
        public PacketResponseType ResponseType => PacketResponseType.MatchStartResponse;

        public int RoomId { get; set; }
        public byte YourSymbol { get; set; } // 1 = X, 2 = O
        public byte FirstTurnSymbol { get; set; } // 1 = X, 2 = O
        public string OpponentName { get; set; } = string.Empty;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)ResponseType);
            writer.Put(RoomId);
            writer.Put(YourSymbol);
            writer.Put(FirstTurnSymbol);
            writer.Put(OpponentName ?? string.Empty);
        }

        public void Deserialize(NetDataReader reader)
        {
            RoomId = reader.GetInt();
            YourSymbol = reader.GetByte();
            FirstTurnSymbol = reader.GetByte();
            OpponentName = reader.GetString();
        }
    }
}
