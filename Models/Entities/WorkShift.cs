namespace Entity.Entities;

public class WorkShift
{
    public int ShiftId { get; set; }
    public int EmployeeId { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public double OpeningCash { get; set; } = 0.0;
    public double ClosingCash { get; set; } = 0.0;
    public string HandoverNote { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
}