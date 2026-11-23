using BLL.Interfaces;

namespace BLL.Services;

// Service quản lý hóa đơn
public class InvoiceService : IInvoiceService
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IInvoiceRepository _invoiceRepo;

    public InvoiceService() : this(new BookingRepository(), new InvoiceRepository())
    {
    }

    public InvoiceService(IBookingRepository bookingRepo, IInvoiceRepository invoiceRepo)
    {
        _bookingRepo = bookingRepo ?? throw new ArgumentNullException(nameof(bookingRepo));
        _invoiceRepo = invoiceRepo ?? throw new ArgumentNullException(nameof(invoiceRepo));
    }

    // Lấy tất cả hóa đơn
    public List<Invoice> GetAll()
    {
        SessionManager.EnsureAnyPermission(
            "Bạn không có quyền xem danh sách hóa đơn!",
            AppConstants.QuyenQuanLyDatPhong,
            AppConstants.QuyenXemBaoCao);
        var ds = _invoiceRepo.GetAll();
        foreach (var hd in ds)
        {
            ChuanHoaSoTien(hd);
        }
        return ds;
    }

    // Lấy hóa đơn theo mã
    public Invoice? GetById(int invoiceId)
    {
        SessionManager.EnsureAnyPermission(
            "Bạn không có quyền xem thông tin hóa đơn!",
            AppConstants.QuyenQuanLyDatPhong,
            AppConstants.QuyenXemBaoCao,
            AppConstants.QuyenDatPhongOnline);

        var invoice = _invoiceRepo.GetById(invoiceId);
        if (invoice != null)
        {
            if (!SessionManager.HasAnyPermission(AppConstants.QuyenQuanLyDatPhong, AppConstants.QuyenXemBaoCao))
            {
                var booking = _bookingRepo.GetById(invoice.BookingId);
                if (booking == null || !SessionManager.HasStaffOrOwnerAccess(booking.CustomerId, AppConstants.QuyenQuanLyDatPhong, AppConstants.QuyenXemBaoCao))
                    throw new UnauthorizedAccessException("Bạn chỉ có quyền xem hóa đơn của chính mình!");
            }

            ChuanHoaSoTien(invoice);
        }

        return invoice;
    }

    // Thanh toán hóa đơn
    public bool Pay(int invoiceId, string paymentMethod, out string message)
    {
        if (!SessionManager.CheckPermission(
                out message,
                "Bạn không có quyền thanh toán hóa đơn!",
                AppConstants.QuyenQuanLyDatPhong))
        {
            return false;
        }

        if (invoiceId <= 0)
        {
            message = "Mã hóa đơn không hợp lệ!";
            return false;
        }

        var result = _invoiceRepo.PayInvoice(invoiceId, paymentMethod);
        if (result)
        {
            message = $"Thanh toán hóa đơn HD{invoiceId:D3} thành công bằng {paymentMethod}!";
            return true;
        }

        message = "Thanh toán thất bại!";
        return false;
    }
    private static void ChuanHoaSoTien(Invoice hd)
    {
        var tamTinh = hd.RoomCharge + hd.ServiceCharge;

        // Khôi phục đúng VAT và Tổng cộng nếu DB lưu nhầm VAT vào cột TotalAmount
        if (hd.VatAmount <= 0 && hd.TotalAmount > 0 && hd.TotalAmount < tamTinh)
        {
            hd.VatAmount = hd.TotalAmount;
            hd.TotalAmount = tamTinh + hd.VatAmount;
            return;
        }

        if (hd.VatAmount <= 0 && tamTinh > 0)
        {
            hd.VatAmount = Math.Round(tamTinh * 8 / 100, 0);
        }

        if (hd.TotalAmount < tamTinh)
        {
            hd.TotalAmount = tamTinh + hd.VatAmount;
        }
    }
}
