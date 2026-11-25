namespace DAL.Interfaces;

public interface IRoomRepository
{
    List<Room> GetAll();
    List<Room> GetByFloor(int floor);
    List<Room> GetAvailableRooms(int? roomTypeId = null);

    bool Add(Room room);
    bool Update(Room room);
    bool Delete(int roomId);

    bool ExistsByRoomNumber(string roomNumber, int? excludeRoomId = null);

    bool UpdateStatus(int roomId, string newStatus);
    bool UpdateStatusByRoomNumber(string roomNumber, string newStatus);

    Room? GetById(int roomId);
    Room? GetByRoomNumber(string roomNumber);
}