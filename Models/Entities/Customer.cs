namespace Entity.Entities;

public class Customer :Person
{
    public int CustomerId { get; set; }
    public int? AccountId { get; set; }
   //ma hang thanh vien
    public int TierId { get; set; } = 1;
    public string Email { get; set; } = string.Empty;
    // diem tich luy
    public int LoyaltyPoints { get; set; } = 0;
    // ten hang thanh vien
    public string TierName { get; set; } = "Thường";
    // so lan luu tru
    public int StayCount { get; set; } = 0;
    public string Note { get; set; } = string.Empty;
    public override string ToString() => $"{FullName} ({PhoneNumber})";

   
}

