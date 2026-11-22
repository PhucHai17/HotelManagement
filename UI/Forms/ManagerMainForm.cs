using Common.Constants;
using Common.Helpers;

namespace UI.Forms;

// Giao diện chính của Quản lý và Quản trị viên
public class ManagerMainForm : ReceptionistMainForm
{
    private Panel pnlNhanVien = null!;
    private Panel pnlNhatKy = null!;

    protected override string LayTieuDeForm()
    {
        return SessionManager.CurrentUser?.RoleId == AppConstants.MaVTAdmin
            ? "HotelPro - Giao diện Quản trị viên"
            : "HotelPro - Giao diện Quản lý";
    }

    protected override string LayNhanVaiTroSidebar()
    {
        return SessionManager.CurrentUser?.RoleId == AppConstants.MaVTAdmin
            ? "Quản trị viên"
            : "Quản lý";
    }
    
    protected override List<string> LayDanhSachChucNangMenu()
    {
        return new List<string> { "Báo cáo", "Nhân viên", "Nhật ký hệ thống" };
    }
    
    protected override void KhoiTaoCacTabNoiDung()
    {
        base.KhoiTaoCacTabNoiDung();
        KhoiTaoTabNhanVien();
        KhoiTaoTabNhatKy();
    }
    
    protected override void ChuyenTabMacDinh()
    {
        XuLyChuyenMenu("Báo cáo");
    }
    
    protected override void TaiDuLieuMacDinh()
    {
        TaiDuLieuBaoCao();
    }
    
    protected override bool XuLyChuyenMenuMoRong(string tenChucNang)
    {
        switch (tenChucNang)
        {
            case "Nhân viên":
                if (pnlNhanVien != null)
                {
                    ChuyenTab(pnlNhanVien, "Nhân viên");
                    return true;
                }
                return false;
            case "Nhật ký hệ thống":
                if (pnlNhatKy != null)
                {
                    ChuyenTab(pnlNhatKy, "Nhật ký hệ thống");
                    return true;
                }
                return false;
            default:
                return base.XuLyChuyenMenuMoRong(tenChucNang);
        }
    }

    // Khởi tạo khung tab Quản lý nhân viên
    private void KhoiTaoTabNhanVien()
    {
        pnlNhanVien = TaoKhungTabCoBan(
            "QUẢN LÝ NHÂN VIÊN",
            "Phân quyền tài khoản, quản lý hồ sơ nhân sự và phân ca làm việc");
        pnlMainContent.Controls.Add(pnlNhanVien);
    }

    // Khởi tạo khung tab Nhật ký hệ thống
    private void KhoiTaoTabNhatKy()
    {
        pnlNhatKy = TaoKhungTabCoBan(
            "NHẬT KÝ HỆ THỐNG",
            "Theo dõi lịch sử truy cập, biến động phòng, hóa đơn và tác nghiệp của nhân sự");
        pnlMainContent.Controls.Add(pnlNhatKy);
    }
    
    private static Panel TaoKhungTabCoBan(string tieuDe, string moTa)
    {
        var pnl = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(243, 244, 246),
            Padding = new Padding(0)
        };

        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 70,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(20, 12, 20, 10),
            Margin = new Padding(0, 0, 0, 12)
        };

        var lblTieuDe = new Label
        {
            Text = tieuDe,
            Font = new Font(AppConstants.FontChuChinh, 14f, FontStyle.Bold),
            ForeColor = AppConstants.MauXanhNavy,
            Dock = DockStyle.Top,
            Height = 26,
            UseMnemonic = false
        };

        var lblMoTa = new Label
        {
            Text = moTa,
            Font = new Font(AppConstants.FontChuChinh, 10f, FontStyle.Regular),
            ForeColor = Color.FromArgb(100, 116, 139),
            Dock = DockStyle.Top,
            Height = 20,
            UseMnemonic = false
        };

        pnlHeader.Controls.Add(lblMoTa);
        pnlHeader.Controls.Add(lblTieuDe);
        pnl.Controls.Add(pnlHeader);

        return pnl;
    }
}
