using Entity.Entities;
using Entity.Enums;

namespace DAL.Interfaces;

public interface IRoomRepository : IGenericRepository<Room>
{
    IEnumerable<Room> GetAvailableRooms(DateTime checkIn, DateTime checkOut, int? roomTypeId = null);
    bool UpdateRoomStatus(int roomId, RoomStatus status);
    IEnumerable<Room> GetRoomsByStatus(RoomStatus status);
}