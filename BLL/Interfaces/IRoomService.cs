namespace BLL.Interfaces;

public interface IRoomService
{
    List<Room> GetAll();
    List<Room> GetByFloor(int floor);
    List<Room> GetAvailableRooms(int? roomTypeId = null);
    List<RoomType> GetAllRoomTypes();
    bool UpdateStatus(int roomId, string newStatus, out string message);
    Room? GetByRoomNumber(string roomNumber);
}
