using BLL.Interfaces;

namespace BLL.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepo;
    private readonly IRoomTypeRepository _roomTypeRepo;

    public RoomService() : this(new RoomTypeRepository(), new RoomRepository())
    {
    }

    public RoomService(IRoomTypeRepository roomTypeRepo, IRoomRepository roomRepo)
    {
        _roomTypeRepo = roomTypeRepo ?? throw new ArgumentNullException(nameof(roomTypeRepo));
        _roomRepo = roomRepo ?? throw new ArgumentNullException(nameof(roomRepo));
    }

    public List<Room> GetAll()
    {
        return _roomRepo.GetAll();
    }

    public List<Room> GetByFloor(int floor)
    {
        return _roomRepo.GetByFloor(floor);
    }

    public List<Room> GetAvailableRooms(int? roomTypeId = null)
    {
        return _roomRepo.GetAvailableRooms(roomTypeId);
    }

    public List<RoomType> GetAllRoomTypes()
    {
        return _roomTypeRepo.GetAll();
    }

    // Cập nhật trạng thái phòng kèm kiểm tra quyền
    public bool UpdateStatus(int roomId, string newStatus, out string message)
    {
        if (!SessionManager.CheckPermission(
                out message,
                "Bạn không có quyền cập nhật trạng thái phòng!",
                AppConstants.QuyenQuanLyPhong))
            return false;

        if (roomId <= 0)
        {
            message = "Mã phòng không hợp lệ!";
            return false;
        }

        var result = _roomRepo.UpdateStatus(roomId, newStatus);
        if (result)
        {
            message = $"Đã cập nhật trạng thái phòng thành: {newStatus}";
            return true;
        }

        message = "Không thể cập nhật trạng thái phòng!";
        return false;
    }

    public Room? GetByRoomNumber(string roomNumber)
    {
        return _roomRepo.GetByRoomNumber(roomNumber);
    }
}
