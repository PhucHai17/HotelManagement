using Common.Constants;
using UI.Controls;

namespace UI.Helpers;

internal static class AuthLayoutHelper
{
    public static (Panel LeftBanner, Panel RightHost, FlowLayoutPanel FormColumn) BuildAuthSplitLayout(
        System.Windows.Forms.Form form,
        string slogan,
        int formWidth)
    {
        // Cột phải fill trước để panel ảnh bên trái bám sát mép (0, 0) không lộ viền trắng
        var pnlRight = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppConstants.MauNen,
            AutoScroll = true,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        var pnlLeft = ImageHelper.CreateCoverImagePanel(
            ImageHelper.LoadHero,
            overlayColor: Color.FromArgb(182, 22, 37, 68));
        pnlLeft.Dock = DockStyle.Left;
        pnlLeft.BackColor = AppConstants.MauXanhNavyDam;
        pnlLeft.Margin = Padding.Empty;
        pnlLeft.Padding = Padding.Empty;

        // Nhóm thương hiệu HOTELPRO + khẩu hiệu căn giữa trên ảnh nền
        var pnlLogoGroup = new FlowLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Color.Transparent,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        var lblLogo = new HotelProLogo
        {
            Font = new Font(AppConstants.FontChuChinh, 28f, FontStyle.Bold),
            AutoSize = true,
            Anchor = AnchorStyles.None,
            Margin = new Padding(0, 0, 0, 10)
        };

        var lblSlogan = new Label
        {
            Text = slogan,
            ForeColor = Color.FromArgb(226, 232, 240),
            Font = new Font(AppConstants.FontChuChinh, 10.5f, FontStyle.Regular),
            AutoSize = true,
            MaximumSize = new Size(320, 0),
            Anchor = AnchorStyles.None,
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.Transparent,
            Margin = Padding.Empty
        };

        pnlLogoGroup.Controls.Add(lblLogo);
        pnlLogoGroup.Controls.Add(lblSlogan);
        pnlLeft.Controls.Add(pnlLogoGroup);

        // Cột form nhập liệu xếp dọc tự động theo DPI, không bao giờ chồng đè nhãn/ô nhập
        var flowForm = new FlowLayoutPanel
        {
            Width = formWidth,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = AppConstants.MauNen,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        pnlRight.Controls.Add(flowForm);

        var isUpdatingLayout = false;
        void CapNhatBoCuc()
        {
            if (isUpdatingLayout) return;
            isUpdatingLayout = true;
            try
            {
                var leftWidth = Math.Max(280, (int)Math.Round(form.ClientSize.Width * 0.38));
                if (pnlLeft.Width != leftWidth)
                    pnlLeft.Width = leftWidth;

                pnlLogoGroup.PerformLayout();
                pnlLogoGroup.Location = new Point(
                    Math.Max(12, (pnlLeft.ClientSize.Width - pnlLogoGroup.Width) / 2),
                    Math.Max(24, (pnlLeft.ClientSize.Height - pnlLogoGroup.Height) / 2));

                flowForm.PerformLayout();
                flowForm.Location = new Point(
                    Math.Max(20, (pnlRight.ClientSize.Width - flowForm.Width) / 2),
                    Math.Max(16, (pnlRight.ClientSize.Height - flowForm.Height) / 2));
            }
            finally
            {
                isUpdatingLayout = false;
            }
        }

        form.Resize += (_, _) => CapNhatBoCuc();
        pnlLeft.Resize += (_, _) => CapNhatBoCuc();
        pnlRight.Resize += (_, _) => CapNhatBoCuc();
        flowForm.SizeChanged += (_, _) => CapNhatBoCuc();
        form.Load += (_, _) => CapNhatBoCuc();
        form.Shown += (_, _) => CapNhatBoCuc();

        form.Controls.Add(pnlRight);
        form.Controls.Add(pnlLeft);

        CapNhatBoCuc();
        return (pnlLeft, pnlRight, flowForm);
    }

    public static Panel CreateLabeledInput(
        string labelText,
        int width,
        out RoundedTextBox input,
        string placeholder = "",
        bool isPassword = false,
        int bottomMargin = 14)
    {
        var block = new Panel
        {
            Width = width,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = AppConstants.MauNen,
            Margin = new Padding(0, 0, 0, bottomMargin)
        };

        var lbl = new Label
        {
            Text = labelText,
            Location = new Point(0, 0),
            AutoSize = true,
            ForeColor = Color.FromArgb(71, 85, 105),
            Font = new Font(AppConstants.FontChuChinh, 9.5f, FontStyle.Regular),
            Margin = Padding.Empty
        };

        var inputTop = Math.Max(22, lbl.PreferredHeight + 6);
        input = new RoundedTextBox
        {
            Location = new Point(0, inputTop),
            Size = new Size(width, 38),
            PlaceholderText = placeholder
        };
        if (isPassword)
            input.PasswordChar = '●';

        block.Controls.Add(lbl);
        block.Controls.Add(input);
        block.AutoSize = false;
        block.Size = new Size(width, input.Bottom);
        return block;
    }

    public static Label CreateHeaderLabel(string text, int bottomMargin = 20)
    {
        return new Label
        {
            Text = text,
            Font = new Font(AppConstants.FontChuChinh, 21f, FontStyle.Bold),
            ForeColor = AppConstants.MauChuChinh,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, bottomMargin)
        };
    }

    public static LinkLabel CreateCenteredSwitchLink(string prefixText, string actionText, int width, int bottomMargin)
    {
        var fullText = $"{prefixText}{actionText}";
        var link = new LinkLabel
        {
            Text = fullText,
            Size = new Size(width, 28),
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font(AppConstants.FontChuChinh, 9.5f, FontStyle.Regular),
            ForeColor = AppConstants.MauChuPhu,
            LinkColor = AppConstants.MauChuChinh,
            ActiveLinkColor = AppConstants.MauVangCamDam,
            VisitedLinkColor = AppConstants.MauChuChinh,
            LinkBehavior = LinkBehavior.HoverUnderline,
            LinkArea = new LinkArea(prefixText.Length, actionText.Length),
            Margin = new Padding(0, 0, 0, bottomMargin)
        };
        return link;
    }
}
