namespace DAL.Interfaces;

// Interface repository phòng
public interface IRoomRepository
{
    List<Room> GetAll();
    List<Room> GetByFloor(int floor);
    List<Room> GetAvailableRooms(int? roomTypeId = null);
    bool UpdateStatus(int roomId, string newStatus);
    bool UpdateStatusByRoomNumber(string roomNumber, string newStatus);
    Room? GetById(int roomId);
    Room? GetByRoomNumber(string roomNumber);
}
