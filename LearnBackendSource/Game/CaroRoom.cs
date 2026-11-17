using System;
using LiteNetLib;
using LiteNetLib.Utils;
using NetworkShare;

namespace LearnBackendSource.Game
{
    public class CaroRoom
    {
        public int RoomId { get; }
        public NetPeer Player1 { get; } // Quân X (1), đi trước
        public NetPeer Player2 { get; } // Quân O (2)

        public int[] Board { get; } = new int[9];
        public byte CurrentTurnSymbol { get; private set; } = 1;
        public bool IsGameOver { get; private set; } = false;

        private static readonly int[][] WinLines = new int[][]
        {
            new int[] { 0, 1, 2 }, // Hàng 1
            new int[] { 3, 4, 5 }, // Hàng 2
            new int[] { 6, 7, 8 }, // Hàng 3
            new int[] { 0, 3, 6 }, // Cột 1
            new int[] { 1, 4, 7 }, // Cột 2
            new int[] { 2, 5, 8 }, // Cột 3
            new int[] { 0, 4, 8 }, // Chéo 1
            new int[] { 2, 4, 6 }  // Chéo 2
        };

        public CaroRoom(int roomId, NetPeer player1, NetPeer player2)
        {
            RoomId = roomId;
            Player1 = player1;
            Player2 = player2;
        }

        public void StartGame()
        {
            Array.Clear(Board, 0, Board.Length);
            CurrentTurnSymbol = 1; // X đi trước
            IsGameOver = false;

            Console.WriteLine($"[Room {RoomId}] Trận đấu bắt đầu giữa Peer {Player1.Id} (X) và Peer {Player2.Id} (O)");

            // Báo cho Player 1
            SendPacket(Player1, new Net_MatchStartResponse
            {
                RoomId = RoomId,
                YourSymbol = 1, // X
                FirstTurnSymbol = 1,
                OpponentName = $"Player_{Player2.Id}"
            });

            // Báo cho Player 2
            SendPacket(Player2, new Net_MatchStartResponse
            {
                RoomId = RoomId,
                YourSymbol = 2, // O
                FirstTurnSymbol = 1,
                OpponentName = $"Player_{Player1.Id}"
            });
        }

        public void HandleMove(NetPeer peer, int cellIndex)
        {
            if (IsGameOver) return;
            if (cellIndex < 0 || cellIndex >= 9) return;
            if (Board[cellIndex] != 0) return; // Ô đã có người đánh

            byte senderSymbol = (peer.Id == Player1.Id) ? (byte)1 : (byte)2;
            if (senderSymbol != CurrentTurnSymbol)
            {
                Console.WriteLine($"[Room {RoomId}] Peer {peer.Id} đánh không đúng lượt!");
                return;
            }

            Board[cellIndex] = senderSymbol;
            Console.WriteLine($"[Room {RoomId}] Peer {peer.Id} ({(senderSymbol == 1 ? "X" : "O")}) đánh ô {cellIndex}");

            // Kiểm tra thắng
            if (CheckWin(senderSymbol))
            {
                IsGameOver = true;
                // Cập nhật nước đi cuối cùng
                Broadcast(new Net_MoveUpdateResponse
                {
                    CellIndex = cellIndex,
                    Symbol = senderSymbol,
                    NextTurnSymbol = 0
                });

                // Báo kết thúc
                Broadcast(new Net_GameOverResponse
                {
                    WinnerSymbol = senderSymbol,
                    Reason = $"Người chơi {(senderSymbol == 1 ? "X" : "O")} chiến thắng!"
                });
                return;
            }

            // Kiểm tra hòa
            if (CheckDraw())
            {
                IsGameOver = true;
                Broadcast(new Net_MoveUpdateResponse
                {
                    CellIndex = cellIndex,
                    Symbol = senderSymbol,
                    NextTurnSymbol = 0
                });

                Broadcast(new Net_GameOverResponse
                {
                    WinnerSymbol = 0,
                    Reason = "Trận đấu hòa!"
                });
                return;
            }

            // Đổi lượt
            CurrentTurnSymbol = (byte)(CurrentTurnSymbol == 1 ? 2 : 1);
            Broadcast(new Net_MoveUpdateResponse
            {
                CellIndex = cellIndex,
                Symbol = senderSymbol,
                NextTurnSymbol = CurrentTurnSymbol
            });
        }

        public void HandleRestart(NetPeer peer)
        {
            Console.WriteLine($"[Room {RoomId}] Nhận yêu cầu chơi lại từ Peer {peer.Id}");
            StartGame();
        }

        public void HandleDisconnect(NetPeer disconnectedPeer)
        {
            IsGameOver = true;
            NetPeer remainingPeer = (disconnectedPeer.Id == Player1.Id) ? Player2 : Player1;
            if (remainingPeer != null && remainingPeer.ConnectionState == ConnectionState.Connected)
            {
                SendPacket(remainingPeer, new Net_OpponentLeftResponse
                {
                    Message = "Đối thủ đã mất kết nối hoặc thoát trận. Bạn thắng!"
                });
            }
        }

        private bool CheckWin(int symbol)
        {
            foreach (var line in WinLines)
            {
                if (Board[line[0]] == symbol &&
                    Board[line[1]] == symbol &&
                    Board[line[2]] == symbol)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckDraw()
        {
            for (int i = 0; i < 9; i++)
            {
                if (Board[i] == 0) return false;
            }
            return true;
        }

        private void Broadcast(INetPacketResponse packet)
        {
            if (Player1.ConnectionState == ConnectionState.Connected)
            {
                SendPacket(Player1, packet);
            }
            if (Player2.ConnectionState == ConnectionState.Connected)
            {
                SendPacket(Player2, packet);
            }
        }

        public static void SendPacket(NetPeer peer, INetPacketResponse packet)
        {
            var writer = new NetDataWriter();
            packet.Serialize(writer);
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }
    }
}
