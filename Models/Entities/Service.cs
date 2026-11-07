namespace Entity.Entities;

public class Service : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public string Unit { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public virtual ICollection<ServiceUsage> ServiceUsges { get; set; } = new List<ServiceUsage>();
}