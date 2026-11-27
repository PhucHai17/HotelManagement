using Common.Constants;
using Common.Helpers;
using UI.Controls;
using UI.Helpers;

namespace UI.Forms;

public class ManagerMainForm : ReceptionistMainForm
{
    private Panel pnlNhanVien = null!;
    private Panel pnlNhatKy = null!;
    private Panel pnlPhanCa = null!;
    private EmployeeTab _employeeTab = null!;
    private SystemLogTab _systemLogTab = null!;
    private ShiftScheduleTab _shiftScheduleTab = null!;

    protected override string LayTieuDeForm() => SessionManager.CurrentUser?.RoleId == AppConstants.MaVTAdmin ? "HotelPro - Giao diện Quản trị viên" : "HotelPro - Giao diện Quản lý";
    protected override string LayNhanVaiTroSidebar() => SessionManager.CurrentUser?.RoleId == AppConstants.MaVTAdmin ? "Quản trị viên" : "Quản lý";
    protected override List<string> LayDanhSachChucNangMenu() => new() { "Báo cáo", "Nhân viên", "Phân ca & Giao ca", "Nhật ký hệ thống" };

    protected override void KhoiTaoCacTabNoiDung()
    {
        base.KhoiTaoCacTabNoiDung();
        KhoiTaoTabNhanVien();
        KhoiTaoTabPhanCa();
        KhoiTaoTabNhatKy();
    }

    protected override void ChuyenTabMacDinh() => XuLyChuyenMenu("Báo cáo");
    protected override void TaiDuLieuMacDinh() => TaiDuLieuBaoCao();

    protected override bool XuLyChuyenMenuMoRong(string tenChucNang)
    {
        switch (tenChucNang)
        {
            case "Nhân viên":
                ChuyenTab(pnlNhanVien, tenChucNang); _employeeTab?.LoadEmployees(); return true;
            case "Phân ca & Giao ca":
                ChuyenTab(pnlPhanCa, tenChucNang); _shiftScheduleTab?.LoadSchedules(); return true;
            case "Nhật ký hệ thống":
                ChuyenTab(pnlNhatKy, tenChucNang); _systemLogTab?.LoadLogs(); return true;
            default: return base.XuLyChuyenMenuMoRong(tenChucNang);
        }
    }

    private void KhoiTaoTabNhanVien()
    {
        pnlNhanVien = CreateTabFrame("QUẢN LÝ NHÂN VIÊN", "Quản lý hồ sơ nhân sự, tài khoản và quyền truy cập.");
        _employeeTab = new EmployeeTab { Dock = DockStyle.Fill };
        pnlNhanVien.Controls.Add(_employeeTab);
        pnlMainContent.Controls.Add(pnlNhanVien);
    }

    private void KhoiTaoTabPhanCa()
    {
        pnlPhanCa = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(243, 244, 246) };
        _shiftScheduleTab = new ShiftScheduleTab { Dock = DockStyle.Fill };
        pnlPhanCa.Controls.Add(_shiftScheduleTab);
        pnlMainContent.Controls.Add(pnlPhanCa);
    }

    private void KhoiTaoTabNhatKy()
    {
        pnlNhatKy = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        _systemLogTab = new SystemLogTab { Dock = DockStyle.Fill };
        pnlNhatKy.Controls.Add(_systemLogTab);
        pnlMainContent.Controls.Add(pnlNhatKy);
    }

    private static Panel CreateTabFrame(string title, string description)
    {
        var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(243, 244, 246) };
        var header = new Panel { Dock = DockStyle.Top, Height = 86, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Padding = new Padding(20, 14, 20, 12) };
        header.Controls.Add(new Label { Text = description, Font = new Font(AppConstants.FontChuChinh, 10f), ForeColor = Color.FromArgb(100, 116, 139), Location = new Point(20, 48), AutoSize = true, UseMnemonic = false });
        header.Controls.Add(new Label { Text = title, Font = new Font(AppConstants.FontChuChinh, 14f, FontStyle.Bold), ForeColor = AppConstants.MauXanhNavy, Location = new Point(20, 14), AutoSize = true, UseMnemonic = false });
        panel.Controls.Add(header);
        return panel;
    }
}
