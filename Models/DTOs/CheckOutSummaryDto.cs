namespace Entity.DTOs;

using System;
using System.Collections.Generic;

public class CheckOutSummaryDto
{
    public int BookingId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int TotalNights { get; set; }
    public decimal RoomCharge { get; set; }
    public decimal ServiceCharge { get; set; }
    public decimal Surcharge { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DepositPaid { get; set; }
    public decimal FinalAmountToPay { get; set; }

    public List<ServiceItemDto> ServicesUsed { get; set; } = new();
}

public class ServiceItemDto
{
    public string ServiceName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;
}
