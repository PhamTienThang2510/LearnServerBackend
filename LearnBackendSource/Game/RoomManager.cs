using System;
using System.Collections.Generic;
using LiteNetLib;
using NetworkShare;

namespace LearnBackendSource.Game
{
    public class RoomManager
    {
        private static readonly object _lock = new object();
        private static int _nextRoomId = 1;

        private readonly Queue<NetPeer> _waitingQueue = new Queue<NetPeer>();
        private readonly Dictionary<int, CaroRoom> _rooms = new Dictionary<int, CaroRoom>();
        private readonly Dictionary<int, CaroRoom> _peerToRoom = new Dictionary<int, CaroRoom>();

        public void EnqueueMatchmaking(NetPeer peer)
        {
            lock (_lock)
            {
                // Nếu đang trong room cũ thì dọn dẹp
                if (_peerToRoom.TryGetValue(peer.Id, out var oldRoom))
                {
                    _rooms.Remove(oldRoom.RoomId);
                    _peerToRoom.Remove(oldRoom.Player1.Id);
                    _peerToRoom.Remove(oldRoom.Player2.Id);
                }

                Console.WriteLine($"[Matchmaker] Peer {peer.Id} đang tìm trận...");

                // Lọc queue để bỏ các peer đã disconnect
                while (_waitingQueue.Count > 0)
                {
                    var opponent = _waitingQueue.Dequeue();
                    if (opponent.Id == peer.Id) continue;
                    if (opponent.ConnectionState != ConnectionState.Connected) continue;

                    // Tìm thấy đối thủ! Tạo phòng mới
                    int roomId = _nextRoomId++;
                    var newRoom = new CaroRoom(roomId, opponent, peer);
                    _rooms[roomId] = newRoom;
                    _peerToRoom[opponent.Id] = newRoom;
                    _peerToRoom[peer.Id] = newRoom;

                    newRoom.StartGame();
                    return;
                }

                // Không có ai trong hàng đợi, đưa vào queue
                _waitingQueue.Enqueue(peer);
                Console.WriteLine($"[Matchmaker] Peer {peer.Id} đã vào hàng chờ (Queue count: {_waitingQueue.Count})");
            }
        }

        public void HandlePlayerMove(NetPeer peer, int cellIndex)
        {
            lock (_lock)
            {
                if (_peerToRoom.TryGetValue(peer.Id, out var room))
                {
                    room.HandleMove(peer, cellIndex);
                }
                else
                {
                    Console.WriteLine($"[RoomManager] Peer {peer.Id} gửi nước đi nhưng không thuộc phòng nào!");
                }
            }
        }

        public void HandleRestartMatch(NetPeer peer)
        {
            lock (_lock)
            {
                if (_peerToRoom.TryGetValue(peer.Id, out var room))
                {
                    room.HandleRestart(peer);
                }
            }
        }

        public void OnPeerDisconnected(NetPeer peer)
        {
            lock (_lock)
            {
                // Kiểm tra trong phòng
                if (_peerToRoom.TryGetValue(peer.Id, out var room))
                {
                    room.HandleDisconnect(peer);
                    _rooms.Remove(room.RoomId);
                    _peerToRoom.Remove(room.Player1.Id);
                    _peerToRoom.Remove(room.Player2.Id);
                    Console.WriteLine($"[RoomManager] Đã đóng phòng {room.RoomId} do Peer {peer.Id} ngắt kết nối.");
                }
            }
        }
    }
}
