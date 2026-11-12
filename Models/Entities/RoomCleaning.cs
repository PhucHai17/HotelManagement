namespace Entity.Entities;

public class RoomCleaning
{
    // mã ca dọn
    public int CleaningId { get; set; }
    // mã phòng
    public int RoomId { get; set; }
    // nhân viên dọn
    public int? EmployeeId { get; set; }
    // cần dọn / đang dọn /dã xong
    public string Status { get; set; } = "Cần dọn";
    // ngày hoàn thành
    public string CompletedDate { get; set; } = string.Empty;
    //số phòng
    public string RoomNumber { get; set; } = string.Empty;
    //tên nhân viên
    public string EmployeeName { get; set; } = string.Empty;
}