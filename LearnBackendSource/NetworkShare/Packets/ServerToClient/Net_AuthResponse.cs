namespace NetworkShare
{
    public class Net_AuthResponse : INetPacket
    {
        public PacketType Type => PacketType.AuthResponse;

        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public void Serialize(LiteNetLib.Utils.NetDataWriter writer)
        {
            writer.Put((byte)Type);
            writer.Put(IsSuccess);
            writer.Put(Message);
        }

        public void Deserialize(LiteNetLib.Utils.NetDataReader reader)
        {
            IsSuccess = reader.GetBool();
            Message = reader.GetString();
        }
    } 
}