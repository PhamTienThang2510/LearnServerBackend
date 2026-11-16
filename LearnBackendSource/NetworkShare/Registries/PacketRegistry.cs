using NetworkShare;
namespace LearnBackendSource.Packets
{
    public class PacketRegistry
    {
        private Dictionary<PacketType, Type> _packetTypes = new Dictionary<PacketType, Type>();
        public Dictionary<PacketType, Type> PacketTypes
        {
            get
            {
                if (_packetTypes.Count == 0)
                {
                    RegisterPackets();
                }
                return _packetTypes;
            }
        }
        private void RegisterPackets()
        {
            _packetTypes.Add(PacketType.AuthRequest, typeof(Net_AuthRequest));
            _packetTypes.Add(PacketType.AuthResponse, typeof(Net_AuthResponse));
            //_packetTypes.Add(PacketType.ChatMessage, typeof(Net_ChatMessage));
        }
    }
}