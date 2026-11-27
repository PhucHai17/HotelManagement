using Common.Constants;
using Common.Helpers;
using UI.Controls;

namespace UI.Forms;

public class ManagerMainForm : ReceptionistMainForm
{
    private Panel pnlNhanVien = null!;
    private Panel pnlNhatKy = null!;
    private EmployeeTab _employeeTab = null!;
    private SystemLogTab _systemLogTab = null!;

    protected override string LayTieuDeForm() => SessionManager.CurrentUser?.RoleId == AppConstants.MaVTAdmin
        ? "HotelPro - Giao diện Quản trị viên"
        : "HotelPro - Giao diện Quản lý";

    protected override string LayNhanVaiTroSidebar() => SessionManager.CurrentUser?.RoleId == AppConstants.MaVTAdmin
        ? "Quản trị viên"
        : "Quản lý";

    protected override List<string> LayDanhSachChucNangMenu() =>
        new() { "Báo cáo", "Nhân viên", "Phân ca & Giao ca", "Nhật ký hệ thống" };

    protected override void KhoiTaoCacTabNoiDung()
    {
        base.KhoiTaoCacTabNoiDung();
        KhoiTaoTabNhanVien();
        KhoiTaoTabNhatKy();
    }

    protected override void ChuyenTabMacDinh() => XuLyChuyenMenu("Báo cáo");
    protected override void TaiDuLieuMacDinh() => TaiDuLieuBaoCao();

    protected override bool XuLyChuyenMenuMoRong(string tenChucNang)
    {
        switch (tenChucNang)
        {
            case "Nhân viên":
                if (pnlNhanVien == null) return false;
                ChuyenTab(pnlNhanVien, tenChucNang);
                _employeeTab?.LoadEmployees();
                return true;
            case "Phân ca & Giao ca":
                MessageBox.Show(
                    this,
                    "Chức năng phân ca và giao ca sẽ được cập nhật trong tương lai.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return true;
            case "Nhật ký hệ thống":
                if (pnlNhatKy == null) return false;
                ChuyenTab(pnlNhatKy, tenChucNang);
                _systemLogTab?.LoadLogs();
                return true;
            default:
                return base.XuLyChuyenMenuMoRong(tenChucNang);
        }
    }

    private void KhoiTaoTabNhanVien()
    {
        pnlNhanVien = TaoKhungTabCoBan(
            "QUẢN LÝ NHÂN VIÊN",
            "Quản lý hồ sơ nhân sự và tài khoản nhân viên.");
        _employeeTab = new EmployeeTab { Dock = DockStyle.Fill };
        pnlNhanVien.Controls.Add(_employeeTab);
        _employeeTab.BringToFront();
        pnlMainContent.Controls.Add(pnlNhanVien);
    }

    private void KhoiTaoTabNhatKy()
    {
        pnlNhatKy = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = Padding.Empty };
        _systemLogTab = new SystemLogTab { Dock = DockStyle.Fill };
        pnlNhatKy.Controls.Add(_systemLogTab);
        pnlMainContent.Controls.Add(pnlNhatKy);
    }

    private static Panel TaoKhungTabCoBan(string tieuDe, string moTa)
    {
        var pnl = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(243, 244, 246),
            Padding = Padding.Empty
        };

        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 86,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(20, 14, 20, 12),
            Margin = new Padding(0, 0, 0, 12)
        };

        pnlHeader.Controls.Add(new Label
        {
            Text = moTa,
            Font = new Font(AppConstants.FontChuChinh, 10f),
            ForeColor = Color.FromArgb(100, 116, 139),
            Location = new Point(20, 48),
            AutoSize = true,
            UseMnemonic = false
        });
        pnlHeader.Controls.Add(new Label
        {
            Text = tieuDe,
            Font = new Font(AppConstants.FontChuChinh, 14f, FontStyle.Bold),
            ForeColor = AppConstants.MauXanhNavy,
            Location = new Point(20, 14),
            AutoSize = true,
            UseMnemonic = false
        });
        pnl.Controls.Add(pnlHeader);
        return pnl;
    }
}
