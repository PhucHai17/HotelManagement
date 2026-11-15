using BLL.Interfaces;

namespace BLL.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepo;

    public CustomerService() : this(new CustomerRepository())
    {
    }

    public CustomerService(ICustomerRepository customerRepo)
    {
        _customerRepo = customerRepo ?? throw new ArgumentNullException(nameof(customerRepo));
    }

    public List<Customer> GetAll()
    {
        SessionManager.EnsureAnyPermission(
            "Bạn không có quyền xem danh sách khách hàng!",
            AppConstants.QuyenQuanLyDatPhong,
            AppConstants.QuyenQuanLyPhong);
        return _customerRepo.GetAll();
    }

    public List<Customer> Search(string keyword)
    {
        SessionManager.EnsureAnyPermission(
            "Bạn không có quyền tìm kiếm khách hàng!",
            AppConstants.QuyenQuanLyDatPhong,
            AppConstants.QuyenQuanLyPhong);
        return _customerRepo.Search(keyword);
    }

    public Customer? GetByIdentityCardOrPhone(string keyword)
    {
        SessionManager.EnsureAnyPermission(
            "Bạn không có quyền tra cứu thông tin khách hàng!",
            AppConstants.QuyenQuanLyDatPhong,
            AppConstants.QuyenQuanLyPhong);
        return _customerRepo.GetByIdentityCardOrPhone(keyword);
    }

    // Lưu thông tin khách hàng mới hoặc cập nhật thông tin khách hàng cũ
    public bool SaveCustomer(Customer customer, out string message)
    {
        message = string.Empty;
        if (!SessionManager.HasStaffOrOwnerAccess(customer.CustomerId, AppConstants.QuyenQuanLyDatPhong,
                AppConstants.QuyenQuanLyPhong))
        {
            message = "Bạn không có quyền cập nhật hồ sơ khách hàng này!";
            return false;
        }

        if (string.IsNullOrWhiteSpace(customer.FullName))
        {
            message = "Họ tên khách hàng không được để trống!";
            return false;
        }

        var phone = customer.PhoneNumber?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(phone) || phone.Length < 9)
        {
            message = "Số điện thoại không hợp lệ (tối thiểu 9 chữ số)!";
            return false;
        }

        var nationalId = customer.NationalId?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(nationalId) && nationalId.Length < 9)
        {
            message = "Số CCCD / Hộ chiếu không hợp lệ!";
            return false;
        }

        customer.FullName = customer.FullName.Trim();
        customer.PhoneNumber = phone;
        customer.NationalId = nationalId;

        if (!string.IsNullOrWhiteSpace(phone))
        {
            var existingByPhone = _customerRepo.GetByIdentityCardOrPhone(phone);
            if (existingByPhone != null && existingByPhone.CustomerId != customer.CustomerId)
            {
                message = "Số điện thoại này đã được sử dụng bởi khách hàng khác!";
                return false;
            }
        }

        if (!string.IsNullOrWhiteSpace(nationalId))
        {
            var existingByCccd = _customerRepo.GetByIdentityCardOrPhone(nationalId);
            if (existingByCccd != null && existingByCccd.CustomerId != customer.CustomerId)
            {
                message = "Số CCCD này đã được sử dụng bởi khách hàng khác!";
                return false;
            }
        }

        if (customer.CustomerId > 0)
        {
            var success = _customerRepo.Update(customer);
            message = success ? "Cập nhật hồ sơ khách hàng thành công!" : "Không thể cập nhật hồ sơ!";
            return success;
        }

        var newId = _customerRepo.AddCustomer(customer);
        if (newId > 0)
        {
            customer.CustomerId = newId;
            message = "Thêm khách hàng mới thành công!";
            return true;
        }

        message = "Không thể thêm khách hàng!";
        return false;
    }
}
