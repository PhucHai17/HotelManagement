namespace Entity.Enums;

/// Phương thức thanh toán.
public enum PaymentMethod
{
    Cash = 0,         // Tiền mặt
    BankTransfer = 1, // Chuyển khoản / QR
    CreditCard = 2    // Thẻ tín dụng / ghi nợ
}
