using LiteNetLib;
using LiteNetLib.Utils;
using NetworkShare;

namespace LearnBackendSource.Handlers
{
    public class AuthRequestHandler : IPacketHandler
    {
        public void HandlePacket(INetPacket packet, NetPeer peer)
        {
            var authRequest = (Net_AuthRequest)packet;

            Console.WriteLine($"[AuthHandler] Nhận yêu cầu đăng nhập từ Peer {peer.Id}: Username = {authRequest.Username}");

            // TODO: Kiểm tra logic database, tài khoản mật khẩu ở đây
            bool isSuccess = !string.IsNullOrEmpty(authRequest.Username) && authRequest.Password == "123456";

            // Tạo gói phản hồi gửi lại Client
            var response = new Net_AuthResponse
            {
                IsSuccess = isSuccess,
                Message = isSuccess ? "Đăng nhập thành công!" : "Sai tài khoản hoặc mật khẩu!"
            };

            // Gửi về Client (kèm byte Type ở đầu)
            NetDataWriter writer = new NetDataWriter();
            response.Serialize(writer);
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }
    }
}