using Entity.Constants;
using Entity.Entities;

namespace Common.Helpers;

public class Usersession
{
    public static Account? CurrentAccount { get; set; }
    public static Employee? CurrentEmployee { get; set; }
    public static Customer? CurrentCustomer { get; set; }
    public static bool IsLoggedIn => CurrentAccount != null;
    public static string DisplayName
    {
        get
        {
            if (CurrentAccount?.RoleId == RoleIds.Customer)
            {
                if (CurrentCustomer != null && !string.IsNullOrWhiteSpace(CurrentCustomer.FullName))
                    return CurrentCustomer.FullName;
            }
            else if (CurrentEmployee != null && !string.IsNullOrWhiteSpace(CurrentEmployee.FullName))
            {
                return CurrentEmployee.FullName;
            }

            if (CurrentAccount != null) return CurrentAccount.Username;
            return "Khách";
        }
    }
    public static void Logout()
    {
        CurrentAccount = null;
        CurrentEmployee = null;
        CurrentCustomer = null;
    }
}