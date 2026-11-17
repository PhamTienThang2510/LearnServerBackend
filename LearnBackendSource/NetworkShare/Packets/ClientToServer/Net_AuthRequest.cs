using LiteNetLib.Utils;

namespace NetworkShare
{
    public class Net_AuthRequest : INetPacket
    {
        public PacketType Type => PacketType.AuthRequest;

        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)Type);
            writer.Put(Username ?? string.Empty);
            writer.Put(Password ?? string.Empty);
        }

        public void Deserialize(NetDataReader reader)
        {
            Username = reader.GetString();
            Password = reader.GetString();
        }
    }
}
