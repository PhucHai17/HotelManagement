namespace Entity.Entities;

public class WorkShift
{
    public int ShiftId { get; set; }
    public int EmployeeId { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public decimal OpeningCash { get; set; }
    public decimal ClosingCash { get; set; }

    public string HandoverNote { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;

    public TimeSpan Duration => EndTime - StartTime;

    public decimal CashDifference => ClosingCash - OpeningCash;

    public bool IsValid()
    {
        return EndTime >= StartTime;
    }
}