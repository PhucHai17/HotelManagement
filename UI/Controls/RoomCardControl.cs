

using System.ComponentModel;
using System.Drawing.Drawing2D;
using Common.Constants;
using Entity.Entities;

namespace UI.Controls;

// Thẻ hiển thị trạng thái phòng
public class RoomCardControl : UserControl
{
    private readonly Label _lblRoomNumber;
    private readonly Label _lblStatus;
    private Color _borderColor = Color.FromArgb(203, 213, 225);
    private Room _room = new();

    public RoomCardControl()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer,
            true);
        Size = new Size(115, 75);
        Margin = new Padding(6);
        Cursor = Cursors.Hand;
        BackColor = Color.White;

        _lblRoomNumber = new Label
        {
            Dock = DockStyle.Top,
            Height = 42,
            Font = new Font(AppConstants.FontChuChinh, 16f, FontStyle.Bold),
            TextAlign = ContentAlignment.BottomCenter,
            BackColor = Color.Transparent,
            Text = "101"
        };

        _lblStatus = new Label
        {
            Dock = DockStyle.Fill,
            Font = new Font(AppConstants.FontChuChinh, 10f, FontStyle.Regular),
            TextAlign = ContentAlignment.TopCenter,
            BackColor = Color.Transparent,
            Text = "Trống"
        };

        _lblRoomNumber.Click += (s, e) => OnRoomSelected?.Invoke(_room);
        _lblStatus.Click += (s, e) => OnRoomSelected?.Invoke(_room);
        Click += (s, e) => OnRoomSelected?.Invoke(_room);

        Controls.Add(_lblStatus);
        Controls.Add(_lblRoomNumber);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Room RoomInfo
    {
        get => _room;
        set
        {
            _room = value;
            UpdateUi();
        }
    }

    public event Action<Room>? OnRoomSelected;

    private void UpdateUi()
    {
        _lblRoomNumber.Text = _room.RoomNumber;
        _lblStatus.Text = _room.Status;

        switch (_room.Status)
        {
            case AppConstants.PhongTrong:
                BackColor = Color.FromArgb(220, 252, 231);
                _borderColor = Color.FromArgb(134, 239, 172);
                _lblRoomNumber.ForeColor = Color.FromArgb(21, 128, 61);
                _lblStatus.ForeColor = Color.FromArgb(21, 128, 61);
                break;

            case AppConstants.PhongCoKhach:
                BackColor = Color.FromArgb(254, 226, 226);
                _borderColor = Color.FromArgb(252, 165, 165);
                _lblRoomNumber.ForeColor = Color.FromArgb(185, 28, 28);
                _lblStatus.ForeColor = Color.FromArgb(185, 28, 28);
                break;

            case AppConstants.PhongDaDat:
                BackColor = Color.FromArgb(254, 243, 199);
                _borderColor = Color.FromArgb(253, 224, 71);
                _lblRoomNumber.ForeColor = Color.FromArgb(180, 83, 9);
                _lblStatus.ForeColor = Color.FromArgb(180, 83, 9);
                break;

            case AppConstants.PhongDangDon:
                BackColor = Color.FromArgb(224, 242, 254);
                _borderColor = Color.FromArgb(125, 211, 252);
                _lblRoomNumber.ForeColor = Color.FromArgb(3, 105, 161);
                _lblStatus.ForeColor = Color.FromArgb(3, 105, 161);
                break;

            default:
                BackColor = Color.White;
                _borderColor = Color.FromArgb(226, 232, 240);
                _lblRoomNumber.ForeColor = Color.Black;
                _lblStatus.ForeColor = Color.Black;
                break;
        }

        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var rect = new Rectangle(1, 1, Width - 3, Height - 3);
        using var path = CreateRoundedRectanglePath(rect, 6);
        using var brush = new SolidBrush(BackColor);
        g.FillPath(brush, path);

        using var pen = new Pen(_borderColor, 1.4f);
        g.DrawPath(pen, path);
    }

    private static GraphicsPath CreateRoundedRectanglePath(Rectangle r, int radius)
    {
        var path = new GraphicsPath();
        var d = radius * 2;
        path.AddArc(r.X, r.Y, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }
}
