namespace Entity.DTOs;

public class CheckOutSummary
{
    public double RoomCharge { get; set; }
    public double ServiceCharge { get; set; }
    public double Discount { get; set; }
    public double Vat { get; set; }
    public double Total { get; set; }
    public int Nights { get; set; }
    
}