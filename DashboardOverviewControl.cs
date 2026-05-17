using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

#nullable disable

namespace TBUProject
{
    public class DashboardOverviewControl : UserControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Action<int> OnNavigateRequested { get; set; }

        public DashboardOverviewControl()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = ColorTranslator.FromHtml("#F7FAFC");
            this.AutoScroll = true;
            this.Font = new Font("Segoe UI", 9f, FontStyle.Regular);

            LoadDashboardOverview();
        }

        private void RequestNavigation(int index)
        {
            OnNavigateRequested?.Invoke(index);
        }

        private void LoadDashboardOverview()
        {
            // HEADER ROW
            Label lblPageTitle = new Label();
            lblPageTitle.Text = "Trang Chủ";
            lblPageTitle.Font = new Font("Segoe UI", 18f, FontStyle.Bold);
            lblPageTitle.ForeColor = ColorTranslator.FromHtml("#1A202C");
            lblPageTitle.AutoSize = true;
            lblPageTitle.Location = new Point(40, 30);
            this.Controls.Add(lblPageTitle);

            // Thanh Search bo góc với logic Placeholder
            RoundedTextBox txtSearch = new RoundedTextBox();
            txtSearch.Text = "Tìm kiếm...";
            txtSearch.ForeColor = Color.Gray;
            txtSearch.Size = new Size(350, 38);
            txtSearch.Location = new Point(550, 30);
            
            txtSearch.GotFocus += (s, e) => {
                if (txtSearch.Text == "Tìm kiếm...") {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = Color.Black;
                }
            };
            txtSearch.LostFocus += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtSearch.Text)) {
                    txtSearch.Text = "Tìm kiếm...";
                    txtSearch.ForeColor = Color.Gray;
                }
            };
            
            this.Controls.Add(txtSearch);

            // Biểu tượng Notification (Cái chuông với viền tròn)
            Panel pnlBellContainer = new Panel();
            pnlBellContainer.Size = new Size(40, 40);
            pnlBellContainer.Location = new Point(920, 29); // Căn chỉnh cho khớp với thanh Search
            pnlBellContainer.BackColor = Color.Transparent;
            pnlBellContainer.Cursor = Cursors.Hand;
            
            pnlBellContainer.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, pnlBellContainer.Width - 1, pnlBellContainer.Height - 1);
                
                // Cắt Panel thành hình tròn thực thụ để loại bỏ 4 góc vuông
                using (GraphicsPath path = new GraphicsPath()) {
                    path.AddEllipse(rect);
                    pnlBellContainer.Region = new Region(path);
                }

                using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1)) {
                    e.Graphics.DrawEllipse(pen, rect);
                }
            };

            Label lblBell = new Label();
            lblBell.Text = "🔔";
            lblBell.Font = new Font("Segoe UI", 12f);
            lblBell.AutoSize = false;
            lblBell.Size = new Size(40, 40);
            lblBell.TextAlign = ContentAlignment.MiddleCenter;
            lblBell.Location = new Point(0, 0);
            lblBell.BackColor = Color.Transparent;
            lblBell.Cursor = Cursors.Hand;

            // Hiệu ứng Hover
            pnlBellContainer.MouseEnter += (s, e) => { pnlBellContainer.BackColor = ColorTranslator.FromHtml("#EDF2F7"); };
            pnlBellContainer.MouseLeave += (s, e) => { pnlBellContainer.BackColor = Color.White; };
            lblBell.MouseEnter += (s, e) => { pnlBellContainer.BackColor = ColorTranslator.FromHtml("#EDF2F7"); };
            lblBell.MouseLeave += (s, e) => { pnlBellContainer.BackColor = Color.White; };

            pnlBellContainer.Controls.Add(lblBell);
            this.Controls.Add(pnlBellContainer);

            // WELCOME BANNER (Gradient)
            GradientPanel pnlBanner = new GradientPanel();
            pnlBanner.Size = new Size(920, 130);
            pnlBanner.Location = new Point(40, 90);
            
            Label lblWelcome = new Label();
            lblWelcome.Text = "Xin chào, Tiến sĩ Nguyễn!";
            lblWelcome.ForeColor = Color.White;
            lblWelcome.Font = new Font("Segoe UI", 20f, FontStyle.Bold);
            lblWelcome.AutoSize = true;
            lblWelcome.Location = new Point(30, 30);
            lblWelcome.BackColor = Color.Transparent;

            Label lblBannerDesc = new Label();
            lblBannerDesc.Text = "Nền tảng quản lý thông minh – Đồng hành cùng Giảng viên nâng tầm chất lượng đồ án\nvà khóa luận tốt nghiệp tại Đại học Thái Bình.";
            lblBannerDesc.ForeColor = ColorTranslator.FromHtml("#E2E8F0");
            lblBannerDesc.Font = new Font("Segoe UI", 10f);
            lblBannerDesc.AutoSize = true;
            lblBannerDesc.Location = new Point(30, 75);
            lblBannerDesc.BackColor = Color.Transparent;

            pnlBanner.Controls.Add(lblWelcome);
            pnlBanner.Controls.Add(lblBannerDesc);
            this.Controls.Add(pnlBanner);

            // KPI METRICS CARDS ROW
            int cardX = 40;
            this.Controls.Add(CreateKpiCard("👥", "15", "Nhóm đang\nhướng dẫn", "+2 tuần này", cardX, 240, Color.Green, (s, e) => RequestNavigation(5)));
            cardX += 240;
            this.Controls.Add(CreateKpiCard("📁", "8", "Đề tài\nchờ duyệt", "Cần xử lý", cardX, 240, Color.Orange, (s, e) => RequestNavigation(1)));
            cardX += 240;
            this.Controls.Add(CreateKpiCard("📋", "12", "Báo cáo tiến độ\nchờ xử lý", "", cardX, 240, Color.Transparent, (s, e) => RequestNavigation(3)));
            cardX += 240;
            this.Controls.Add(CreateKpiCard("📢", "3", "Thông báo\nmới", "Hôm nay", cardX, 240, ColorTranslator.FromHtml("#3182CE"), (s, e) => RequestNavigation(6)));

            // ACTIONABLE TO-DO LIST (LEFT COL - 65%)
            Label lblTodoTitle = new Label();
            lblTodoTitle.Text = "📑 Việc cần làm ngay";
            lblTodoTitle.Font = new Font("Segoe UI", 14f, FontStyle.Bold);
            lblTodoTitle.Location = new Point(40, 410);
            lblTodoTitle.AutoSize = true;
            this.Controls.Add(lblTodoTitle);

            Label lblViewAll = new Label();
            lblViewAll.Text = "Xem tất cả";
            lblViewAll.ForeColor = ColorTranslator.FromHtml("#3182CE");
            lblViewAll.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            lblViewAll.Location = new Point(550, 415);
            lblViewAll.AutoSize = true;
            lblViewAll.Cursor = Cursors.Hand;
            this.Controls.Add(lblViewAll);

            // Các hàng tác vụ
            this.Controls.Add(CreateTaskItem("Sinh viên Trần Văn B đã nộp đề tài mới", "Đề tài: Hệ thống thư viện thông minh • 2 giờ trước", "Duyệt đề cương", 40, 450, ColorTranslator.FromHtml("#3182CE"), true));
            this.Controls.Add(CreateTaskItem("Nhóm 4 - Công nghệ phần mềm bị trễ hạn", "Thiếu: Báo cáo tiến độ 2 • Trễ 2 ngày", "Gửi nhắc nhở", 40, 540, ColorTranslator.FromHtml("#D69E2E"), false));
            this.Controls.Add(CreateTaskItem("Chấm điểm bảo vệ Nhóm 12", "Đề tài: Nền tảng Thương mại điện tử • Hạn: Ngày mai", "Nhập điểm", 40, 630, ColorTranslator.FromHtml("#805AD5"), false));

            // TIMELINE & ANNOUNCEMENTS (RIGHT COL - 35%)
            Label lblTimelineTitle = new Label();
            lblTimelineTitle.Text = "📈 Tiến độ học kỳ";
            lblTimelineTitle.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
            lblTimelineTitle.Location = new Point(650, 410);
            lblTimelineTitle.AutoSize = true;
            this.Controls.Add(lblTimelineTitle);

            CardPanel pnlTimeline = new CardPanel();
            pnlTimeline.Size = new Size(310, 180);
            pnlTimeline.Location = new Point(650, 450);
            pnlTimeline.BackColor = Color.White;
            
            // Vẽ các node cho timeline
            pnlTimeline.Controls.Add(CreateTimelineNode("Hạn đăng ký đề tài", "15 Th10, 2023", "✅", 20));
            pnlTimeline.Controls.Add(CreateTimelineNode("Nộp báo cáo giữa kỳ", "20 Th11, 2023 (còn 5 ngày)", "🔵", 70));
            pnlTimeline.Controls.Add(CreateTimelineNode("Nộp báo cáo toàn văn", "10 Th12, 2023", "⚪", 120));
            
            this.Controls.Add(pnlTimeline);

            Label lblAnnounceTitle = new Label();
            lblAnnounceTitle.Text = "THÔNG BÁO MỚI NHẤT";
            lblAnnounceTitle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            lblAnnounceTitle.ForeColor = ColorTranslator.FromHtml("#A0AEC0");
            lblAnnounceTitle.Location = new Point(650, 650);
            lblAnnounceTitle.AutoSize = true;
            this.Controls.Add(lblAnnounceTitle);

            CardPanel pnlAnnounce = new CardPanel();
            pnlAnnounce.Size = new Size(310, 130);
            pnlAnnounce.Location = new Point(650, 680);
            pnlAnnounce.BackColor = Color.White;

            pnlAnnounce.Controls.Add(CreateAnnounceItem("HỆ THỐNG", "Hôm nay, 09:00 AM", "Kế hoạch bảo trì cổng nhập điểm vào cuối tuần này.", 15, ColorTranslator.FromHtml("#3182CE")));
            pnlAnnounce.Controls.Add(CreateAnnounceItem("HỌC VỤ", "Hôm qua", "Cập nhật hướng dẫn trình bày đồ án 2023-2024.", 70, ColorTranslator.FromHtml("#38A169")));

            this.Controls.Add(pnlAnnounce);
        }

        // HÀM TẠO THẺ KPI
        private CardPanel CreateKpiCard(string icon, string num, string text, string badge, int x, int y, Color badgeColor, EventHandler onClick = null)
        {
            CardPanel pnl = new CardPanel();
            pnl.Size = new Size(220, 130);
            pnl.Location = new Point(x, y);
            pnl.BackColor = Color.White;
            if (onClick != null)
            {
                pnl.Cursor = Cursors.Hand;
                pnl.Click += onClick;
            }

            Label lblIcon = new Label();
            lblIcon.Text = icon;
            lblIcon.Font = new Font("Segoe UI", 16f);
            lblIcon.Location = new Point(15, 15);
            lblIcon.AutoSize = true;
            if (onClick != null) lblIcon.Click += onClick;
            pnl.Controls.Add(lblIcon);

            Label lblNum = new Label();
            lblNum.Text = num;
            lblNum.Font = new Font("Segoe UI", 22f, FontStyle.Bold);
            lblNum.Location = new Point(10, 45);
            lblNum.AutoSize = true;
            if (onClick != null) lblNum.Click += onClick;
            pnl.Controls.Add(lblNum);

            Label lblText = new Label();
            lblText.Text = text;
            lblText.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
            lblText.ForeColor = ColorTranslator.FromHtml("#718096");
            lblText.Location = new Point(15, 85);
            lblText.AutoSize = true;
            if (onClick != null) lblText.Click += onClick;
            pnl.Controls.Add(lblText);

            if (!string.IsNullOrEmpty(badge))
            {
                Label lblBadge = new Label();
                lblBadge.Text = badge;
                lblBadge.Font = new Font("Segoe UI", 7.5f, FontStyle.Bold);
                lblBadge.ForeColor = badgeColor;
                lblBadge.Location = new Point(130, 20); // Dịch một chút để vừa Card
                lblBadge.AutoSize = true;
                if (onClick != null) lblBadge.Click += onClick;
                pnl.Controls.Add(lblBadge);
            }

            return pnl;
        }

        // HÀM TẠO ITEM TO-DO LIST
        private CardPanel CreateTaskItem(string title, string sub, string btnText, int x, int y, Color leftBorderColor, bool isPrimaryBtn)
        {
            CardPanel pnl = new CardPanel();
            pnl.Size = new Size(580, 75);
            pnl.Location = new Point(x, y);
            pnl.BackColor = Color.White;

            // Màu viền trái đại diện trạng thái
            Panel pnlLeftBorder = new Panel();
            pnlLeftBorder.Width = 4;
            pnlLeftBorder.Dock = DockStyle.Left;
            pnlLeftBorder.BackColor = leftBorderColor;
            pnl.Controls.Add(pnlLeftBorder);

            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            lblTitle.Location = new Point(20, 15);
            lblTitle.AutoSize = true;
            pnl.Controls.Add(lblTitle);

            Label lblSub = new Label();
            lblSub.Text = sub;
            lblSub.Font = new Font("Segoe UI", 9f);
            lblSub.ForeColor = ColorTranslator.FromHtml("#A0AEC0");
            lblSub.Location = new Point(20, 40);
            lblSub.AutoSize = true;
            pnl.Controls.Add(lblSub);

            RoundedButton btn = new RoundedButton();
            btn.Text = btnText;
            btn.Size = new Size(135, 35);
            btn.Location = new Point(430, 20);
            btn.FlatStyle = FlatStyle.Flat;
            btn.Cursor = Cursors.Hand;
            btn.BorderRadius = 10;
            
            // Tất cả các nút dùng cùng màu xanh chuyên nghiệp
            btn.BackColor = ColorTranslator.FromHtml("#3182CE");
            btn.ForeColor = Color.White;
            btn.FlatAppearance.BorderSize = 0;
            
            pnl.Controls.Add(btn);

            return pnl;
        }

        // HÀM TẠO NODE TIMELINE
        private Panel CreateTimelineNode(string title, string date, string icon, int y)
        {
            Panel pnl = new Panel();
            pnl.Size = new Size(290, 40);
            pnl.Location = new Point(10, y);

            Label lblIcon = new Label();
            lblIcon.Text = icon;
            lblIcon.Location = new Point(5, 10);
            lblIcon.AutoSize = true;
            pnl.Controls.Add(lblIcon);

            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            lblTitle.Location = new Point(35, 5);
            lblTitle.AutoSize = true;
            pnl.Controls.Add(lblTitle);

            Label lblDate = new Label();
            lblDate.Text = date;
            lblDate.Font = new Font("Segoe UI", 8f);
            lblDate.ForeColor = ColorTranslator.FromHtml("#A0AEC0");
            lblDate.Location = new Point(35, 22);
            lblDate.AutoSize = true;
            pnl.Controls.Add(lblDate);

            return pnl;
        }

        // HÀM TẠO THÔNG BÁO
        private Panel CreateAnnounceItem(string type, string date, string text, int y, Color typeColor)
        {
            Panel pnl = new Panel();
            pnl.Size = new Size(290, 50);
            pnl.Location = new Point(10, y);

            Label lblBadge = new Label();
            lblBadge.Text = type;
            lblBadge.Font = new Font("Segoe UI", 7f, FontStyle.Bold);
            lblBadge.ForeColor = typeColor;
            // Màu nền hơi nhạt
            lblBadge.BackColor = Color.FromArgb(30, typeColor);
            lblBadge.Location = new Point(5, 5);
            lblBadge.AutoSize = true;
            pnl.Controls.Add(lblBadge);

            Label lblDate = new Label();
            lblDate.Text = date;
            lblDate.Font = new Font("Segoe UI", 8f);
            lblDate.ForeColor = ColorTranslator.FromHtml("#A0AEC0");
            lblDate.Location = new Point(60, 5);
            lblDate.AutoSize = true;
            pnl.Controls.Add(lblDate);

            Label lblText = new Label();
            lblText.Text = text;
            lblText.Font = new Font("Segoe UI", 8.5f);
            lblText.Location = new Point(5, 25);
            lblText.AutoSize = false;
            lblText.Size = new Size(280, 20); // Giới hạn width để ko bị tràn
            pnl.Controls.Add(lblText);

            return pnl;
        }

        // --- Custom Controls ---
        private class GradientPanel : Panel
        {
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public int BorderRadius { get; set; } = 15;

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Color startColor = ColorTranslator.FromHtml("#0F2042");
                Color endColor = ColorTranslator.FromHtml("#1A365D");

                Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                using (GraphicsPath path = GetRoundedPath(rect, BorderRadius))
                using (LinearGradientBrush brush = new LinearGradientBrush(rect, startColor, endColor, LinearGradientMode.Horizontal))
                {
                    this.Region = new Region(path);
                    e.Graphics.FillPath(brush, path);
                }
            }

            private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
            {
                GraphicsPath path = new GraphicsPath();
                int diameter = radius * 2;
                path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
                path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();
                return path;
            }
        }

        private class CardPanel : Panel
        {
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public int BorderRadius { get; set; } = 10;
            
            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

                using (GraphicsPath path = GetRoundedPath(rect, BorderRadius))
                {
                    this.Region = new Region(path);
                    // Vẽ viền xám nhạt nhẹ nhàng
                    using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            }

            private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
            {
                GraphicsPath path = new GraphicsPath();
                int diameter = radius * 2;
                path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
                path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();
                return path;
            }
        }

        private class RoundedButton : Button
        {
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public int BorderRadius { get; set; } = 8;
            private bool isHovered = false;

            public RoundedButton()
            {
                this.MouseEnter += (s, e) => { isHovered = true; this.Invalidate(); };
                this.MouseLeave += (s, e) => { isHovered = false; this.Invalidate(); };
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);

                using (GraphicsPath path = GetRoundedPath(rect, BorderRadius))
                {
                    this.Region = new Region(path);

                    // Khi di chuột vào (Hover), màu sẽ sáng lên một chút
                    Color fillOuter = isHovered ? ControlPaint.Light(this.BackColor, 0.2f) : this.BackColor;

                    using (SolidBrush brush = new SolidBrush(fillOuter))
                    {
                        e.Graphics.FillPath(brush, path);
                    }

                    TextRenderer.DrawText(e.Graphics, this.Text, this.Font, rect, this.ForeColor, 
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
            }

            private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
            {
                GraphicsPath path = new GraphicsPath();
                int diameter = radius * 2;
                path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
                path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();
                return path;
            }
        }

        private class RoundedTextBox : UserControl
        {
            private TextBox textBox;
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public int BorderRadius { get; set; } = 8;
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public Color BorderColor { get; set; } = ColorTranslator.FromHtml("#E2E8F0");

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public override string Text { get => textBox.Text; set => textBox.Text = value; }
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public override Font Font { get => base.Font; set { base.Font = value; textBox.Font = value; } }
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public override Color ForeColor { get => base.ForeColor; set { base.ForeColor = value; textBox.ForeColor = value; } }

            public RoundedTextBox()
            {
                textBox = new TextBox();
                textBox.BorderStyle = BorderStyle.None;
                textBox.Location = new Point(42, 11); 
                textBox.Width = this.Width - 55;
                textBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                this.BackColor = Color.White;
                this.Padding = new Padding(12, 10, 12, 10);
                this.Size = new Size(320, 38);
                this.Controls.Add(textBox);

                textBox.GotFocus += (s, e) => { BorderColor = ColorTranslator.FromHtml("#3182CE"); this.Invalidate(); };
                textBox.LostFocus += (s, e) => { BorderColor = ColorTranslator.FromHtml("#E2E8F0"); this.Invalidate(); };
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                using (GraphicsPath path = GetRoundedPath(rect, BorderRadius))
                {
                    using (SolidBrush brush = new SolidBrush(Color.White))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                    using (Pen pen = new Pen(BorderColor, 1.5f))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }

                    // Vẽ icon kính lúp bằng vector (GDI+) cho sắc nét
                    using (Pen iconPen = new Pen(ColorTranslator.FromHtml("#A0AEC0"), 2f))
                    {
                        // Vòng tròn kính lúp
                        e.Graphics.DrawEllipse(iconPen, 15, 12, 11, 11);
                        // Cán kính lúp
                        e.Graphics.DrawLine(iconPen, 23, 21, 28, 26);
                    }
                }
            }

            private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
            {
                GraphicsPath path = new GraphicsPath();
                int diameter = radius * 2;
                path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
                path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();
                return path;
            }

            public new event EventHandler GotFocus { add => textBox.GotFocus += value; remove => textBox.GotFocus -= value; }
            public new event EventHandler LostFocus { add => textBox.LostFocus += value; remove => textBox.LostFocus -= value; }
        }
    }
}
