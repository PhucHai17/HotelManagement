namespace BLL.Interfaces;

public interface IRoomService
{
    List<Room> GetAll();
    List<Room> GetByFloor(int floor);
    List<Room> GetAvailableRooms(int? roomTypeId = null);
    List<RoomType> GetAllRoomTypes();

    bool AddRoom(Room room, out string message);
    bool UpdateRoom(Room room, out string message);
    bool DeleteRoom(int roomId, out string message);

    bool UpdateStatus(int roomId, string newStatus, out string message);
    Room? GetByRoomNumber(string roomNumber);
}
