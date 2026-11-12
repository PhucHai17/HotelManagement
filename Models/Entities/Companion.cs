namespace Entity.Entities;

public class Companion
{
    public int CompanionId { get; set; }
    public int BookingDetailId { get; set; }
    public string FullName { get; set; } = string.Empty;
    // cccd
    public string NationalId { get; set; } = string.Empty;
}