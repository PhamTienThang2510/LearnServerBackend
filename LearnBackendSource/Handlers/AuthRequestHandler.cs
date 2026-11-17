using LiteNetLib;
using LiteNetLib.Utils;
using NetworkShare;
namespace LearnBackendSource.Handlers
{
    public static class AuthRequestHandler 
    {
        public static Net_AuthResponse HandleAuthRequest(NetPeer peer, Net_AuthRequest authRequest)
        {
            Console.WriteLine($"[Server] Received AuthRequest from {peer.Id}: Username={authRequest.Username}, Password={authRequest.Password}");
            // Xử lý xác thực ở đây, ví dụ: kiểm tra cơ sở dữ liệu
            if(authRequest.Username == "admin" && authRequest.Password == "123")
            {
                return new Net_AuthResponse
                {
                    IsSuccess = true,
                    Message = "Authentication successful"
                };
            }
            else
            {
                return new Net_AuthResponse
                {
                    IsSuccess = false,
                    Message = "Invalid username or password"
                };
            }
        }
    }
}