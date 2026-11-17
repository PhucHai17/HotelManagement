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

    public bool AddRoom(Room room, out string message)
    {
        if (!SessionManager.CheckPermission(out message,
                "Bạn không có quyền thêm phòng!",
                AppConstants.QuyenQuanLyPhong))
            return false;

        if (room == null)
        {
            message = "Thông tin phòng không hợp lệ!";
            return false;
        }

        if (string.IsNullOrWhiteSpace(room.RoomNumber))
        {
            message = "Số phòng không được để trống!";
            return false;
        }

        if (room.RoomTypeId <= 0)
        {
            message = "Vui lòng chọn loại phòng!";
            return false;
        }

        if (room.Floor <= 0)
        {
            message = "Tầng phòng không hợp lệ!";
            return false;
        }

        if (_roomRepo.ExistsByRoomNumber(room.RoomNumber, null))
        {
            message = $"Phòng {room.RoomNumber} đã tồn tại!";
            return false;
        }

        room.Status ??= AppConstants.PhongTrong;

        var result = _roomRepo.Add(room);
        if (result)
        {
            message = "Thêm phòng thành công!";
            return true;
        }

        message = "Thêm phòng thất bại!";
        return false;
    }

    public bool UpdateRoom(Room room, out string message)
    {
        if (!SessionManager.CheckPermission(out message,
                "Bạn không có quyền sửa phòng!",
                AppConstants.QuyenQuanLyPhong))
            return false;

        if (room == null)
        {
            message = "Thông tin phòng không hợp lệ!";
            return false;
        }

        if (room.RoomId <= 0)
        {
            message = "Mã phòng không hợp lệ!";
            return false;
        }

        if (string.IsNullOrWhiteSpace(room.RoomNumber))
        {
            message = "Số phòng không được để trống!";
            return false;
        }

        if (_roomRepo.ExistsByRoomNumber(room.RoomNumber, room.RoomId))
        {
            message = $"Phòng {room.RoomNumber} đã tồn tại!";
            return false;
        }

        var result = _roomRepo.Update(room);
        if (result)
        {
            message = "Cập nhật phòng thành công!";
            return true;
        }

        message = "Cập nhật phòng thất bại!";
        return false;
    }

    public bool DeleteRoom(int roomId, out string message)
    {
        if (!SessionManager.CheckPermission(out message,
                "Bạn không có quyền xóa phòng!",
                AppConstants.QuyenQuanLyPhong))
            return false;

        if (roomId <= 0)
        {
            message = "Mã phòng không hợp lệ!";
            return false;
        }

        var result = _roomRepo.Delete(roomId);
        if (result)
        {
            message = "Xóa phòng thành công!";
            return true;
        }

        message = "Xóa phòng thất bại!";
        return false;
    }

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