namespace Entity.Entities;

public class ShiftSchedule
{
    public int ScheduleId { get; set; }
    public int EmployeeId { get; set; }
    public int? ManagerId { get; set; }
    public string WorkDate { get; set; } = string.Empty;
    public string ShiftType { get; set; } = "Ca sáng";
    public string EmployeeName { get; set; } = string.Empty;
}