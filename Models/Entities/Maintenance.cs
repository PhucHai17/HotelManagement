namespace Entity.Entities;

public class Maintenance
{
    public int MaintenanceId { get; set; }
    public int RoomId { get; set; }
    public string Description { get; set; } = string.Empty;
    // Chờ xử lý / Đang sửa / Hoàn tất
    public string Status { get; set; } = "Chờ xử lý";
    public string RoomNumber { get; set; } = string.Empty;
}