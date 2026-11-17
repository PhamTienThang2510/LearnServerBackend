using LiteNetLib.Utils;

namespace NetworkShare
{
    public class Net_GameOverResponse : INetPacketResponse
    {
        public PacketResponseType ResponseType => PacketResponseType.GameOverResponse;

        public byte WinnerSymbol { get; set; } // 0 = Hòa, 1 = X, 2 = O
        public string Reason { get; set; } = string.Empty;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)ResponseType);
            writer.Put(WinnerSymbol);
            writer.Put(Reason ?? string.Empty);
        }

        public void Deserialize(NetDataReader reader)
        {
            WinnerSymbol = reader.GetByte();
            Reason = reader.GetString();
        }
    }
}
