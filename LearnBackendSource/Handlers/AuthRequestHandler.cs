using System;
using LiteNetLib;
using NetworkShare;

namespace LearnBackendSource.Handlers
{
    public static class AuthRequestHandler 
    {
        public static Net_AuthResponse HandleAuthRequest(NetPeer peer, Net_AuthRequest authRequest)
        {
            Console.WriteLine($"[Server] Received AuthRequest from Peer {peer.Id}: Username={authRequest.Username}, Password={authRequest.Password}");

            // Cho phép admin/123 hoặc bất kỳ tài khoản test không rỗng nào
            bool isValid = (authRequest.Username == "admin" && authRequest.Password == "123") ||
                           (!string.IsNullOrWhiteSpace(authRequest.Username) && !string.IsNullOrWhiteSpace(authRequest.Password));

            if (isValid)
            {
                return new Net_AuthResponse
                {
                    IsSuccess = true,
                    PlayerId = peer.Id,
                    Message = $"Chào mừng {authRequest.Username}!"
                };
            }
            else
            {
                return new Net_AuthResponse
                {
                    IsSuccess = false,
                    PlayerId = -1,
                    Message = "Tài khoản hoặc mật khẩu không hợp lệ"
                };
            }
        }
    }
}
