using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;

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
            LoadDataFromDatabase();
        }

        private void RequestNavigation(int index)
        {
            OnNavigateRequested?.Invoke(index);
        }

        private void LoadDataFromDatabase()
        {
            try
            {
                // Truy vấn thống kê
                object countGroupsObj = DatabaseHelper.ExecuteScalar("SELECT COUNT(DISTINCT ma_sinh_vien) FROM do_an") ?? 0;
                object countTotalObj = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM do_an") ?? 0;
                object countProjectsObj = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM do_an WHERE ma_loai = 'LDA01' OR ma_loai = 'LDA03' OR ma_loai = 'LTL01' OR ma_loai = 'LDA02'") ?? 0;

                // Cập nhật text trên các thẻ KPI
                Control[] cGroups = this.Controls.Find("lblKpiGroups", true);
                if (cGroups.Length > 0 && cGroups[0] is Label lblGroups) lblGroups.Text = countGroupsObj.ToString();

                Control[] cTotal = this.Controls.Find("lblKpiTotal", true);
                if (cTotal.Length > 0 && cTotal[0] is Label lblTotal) lblTotal.Text = countTotalObj.ToString();

                Control[] cProjects = this.Controls.Find("lblKpiProjects", true);
                if (cProjects.Length > 0 && cProjects[0] is Label lblProjects) lblProjects.Text = countProjectsObj.ToString();

                // Nạp 10 đề tài gần nhất
                string recentTopicsQuery = @"
                    SELECT TOP 10 sv.ma_sinh_vien, sv.ten_sinh_vien, da.ten_do_an 
                    FROM do_an da
                    JOIN sinh_vien sv ON da.ma_sinh_vien = sv.ma_sinh_vien
                    ORDER BY da.ngay_nop DESC";
                DataTable dtTopics = DatabaseHelper.ExecuteQuery(recentTopicsQuery);
                
                Control[] cGrid = this.Controls.Find("dgvRecentTopics", true);
                if (cGrid.Length > 0 && cGrid[0] is DataGridView dgvRecent)
                {
                    dgvRecent.Rows.Clear();
                    foreach (DataRow row in dtTopics.Rows)
                    {
                        dgvRecent.Rows.Add(
                            row["ma_sinh_vien"].ToString(),
                            row["ten_sinh_vien"].ToString(),
                            row["ten_do_an"].ToString()
                        );
                    }
                }

                // Nạp các thông báo gần nhất từ thong_bao
                string notifyQuery = "SELECT TOP 5 tieu_de, noi_dung, ngay_dang FROM thong_bao ORDER BY ngay_dang DESC";
                DataTable dtNotify = DatabaseHelper.ExecuteQuery(notifyQuery);
                
                Control[] cFlp = this.Controls.Find("flpNotifications", true);
                if (cFlp.Length > 0 && cFlp[0] is FlowLayoutPanel flp)
                {
                    flp.Controls.Clear();
                    if (dtNotify.Rows.Count == 0)
                    {
                        Label lblNoNotify = new Label();
                        lblNoNotify.Text = "Không có thông báo mới.";
                        lblNoNotify.Font = new Font("Segoe UI", 10f, FontStyle.Italic);
                        lblNoNotify.ForeColor = ColorTranslator.FromHtml("#A0AEC0");
                        lblNoNotify.AutoSize = true;
                        flp.Controls.Add(lblNoNotify);
                    }
                    else
                    {
                        foreach (DataRow row in dtNotify.Rows)
                        {
                            flp.Controls.Add(CreateNotificationItem(
                                row["tieu_de"].ToString() ?? "",
                                row["noi_dung"].ToString() ?? "",
                                row["ngay_dang"] != DBNull.Value ? Convert.ToDateTime(row["ngay_dang"]).ToString("dd/MM/yyyy") : ""
                            ));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi load Dashboard: " + ex.Message);
            }
        }

        private Panel CreateNotificationItem(string title, string content, string date)
        {
            Panel pnl = new Panel();
            pnl.Size = new Size(270, 75);
            pnl.BackColor = Color.White;
            pnl.Margin = new Padding(0, 5, 0, 5);

            // Gạch dọc chỉ thị màu xanh
            Panel indicator = new Panel();
            indicator.BackColor = ColorTranslator.FromHtml("#3182CE");
            indicator.Dock = DockStyle.Left;
            indicator.Width = 4;
            pnl.Controls.Add(indicator);

            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            lblTitle.ForeColor = ColorTranslator.FromHtml("#2D3748");
            lblTitle.Location = new Point(12, 5);
            lblTitle.Size = new Size(250, 18);
            lblTitle.AutoEllipsis = true;

            Label lblContent = new Label();
            lblContent.Text = content;
            lblContent.Font = new Font("Segoe UI", 8.5f, FontStyle.Regular);
            lblContent.ForeColor = ColorTranslator.FromHtml("#718096");
            lblContent.Location = new Point(12, 23);
            lblContent.Size = new Size(250, 32);
            lblContent.AutoEllipsis = true;

            Label lblDate = new Label();
            lblDate.Text = date;
            lblDate.Font = new Font("Segoe UI", 8f, FontStyle.Italic);
            lblDate.ForeColor = ColorTranslator.FromHtml("#A0AEC0");
            lblDate.Location = new Point(12, 55);
            lblDate.Size = new Size(150, 15);

            pnl.Controls.Add(lblTitle);
            pnl.Controls.Add(lblContent);
            pnl.Controls.Add(lblDate);

            pnl.Paint += (s, e) => {
                using (Pen pen = new Pen(ColorTranslator.FromHtml("#EDF2F7"), 1))
                {
                    e.Graphics.DrawLine(pen, 0, pnl.Height - 1, pnl.Width, pnl.Height - 1);
                }
            };

            return pnl;
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

            // KPI METRICS CARDS ROW (3 cards, evenly spaced)
            int cardX = 40;
            this.Controls.Add(CreateKpiCard("lblKpiGroups", "👥", "0", "Số sinh viên đã nộp", "", cardX, 240, Color.Transparent, null));
            cardX += 320;
            this.Controls.Add(CreateKpiCard("lblKpiTotal", "📁", "0", "Tổng số đề tài", "", cardX, 240, Color.Transparent, null));
            cardX += 320;
            this.Controls.Add(CreateKpiCard("lblKpiProjects", "📋", "0", "Số đồ án", "", cardX, 240, Color.Transparent, null));

            // LEFT COLUMN (65%) - DANH SÁCH RÚT GỌN (DataGridView)
            Label lblGridTitle = new Label();
            lblGridTitle.Text = "📑 Danh sách 10 đề tài gần nhất";
            lblGridTitle.Font = new Font("Segoe UI", 14f, FontStyle.Bold);
            lblGridTitle.Location = new Point(40, 410);
            lblGridTitle.AutoSize = true;
            this.Controls.Add(lblGridTitle);

            CardPanel pnlGrid = new CardPanel();
            pnlGrid.Size = new Size(580, 300);
            pnlGrid.Location = new Point(40, 450);
            pnlGrid.BackColor = Color.White;
            pnlGrid.Padding = new Padding(2); // for border
            
            DataGridView dgvRecent = new DataGridView();
            dgvRecent.Name = "dgvRecentTopics";
            dgvRecent.Dock = DockStyle.Fill;
            dgvRecent.BorderStyle = BorderStyle.None;
            dgvRecent.BackgroundColor = Color.White;
            dgvRecent.RowHeadersVisible = false;
            dgvRecent.AllowUserToAddRows = false;
            dgvRecent.AllowUserToDeleteRows = false;
            dgvRecent.ReadOnly = true;
            dgvRecent.AllowUserToResizeRows = false;
            dgvRecent.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvRecent.GridColor = ColorTranslator.FromHtml("#EDF2F7");
            dgvRecent.RowTemplate.Height = 45;
            dgvRecent.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRecent.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#F7FAFC");
            dgvRecent.DefaultCellStyle.SelectionForeColor = ColorTranslator.FromHtml("#1A202C");
            dgvRecent.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            
            dgvRecent.EnableHeadersVisualStyles = false;
            dgvRecent.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvRecent.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvRecent.ColumnHeadersHeight = 40;
            dgvRecent.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dgvRecent.ColumnHeadersDefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#A0AEC0");
            dgvRecent.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvRecent.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.White;
            dgvRecent.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;

            DataGridViewTextBoxColumn colMaSV = new DataGridViewTextBoxColumn() { Name = "MaSV", HeaderText = "MÃ SV", Width = 100 };
            colMaSV.DefaultCellStyle.Padding = new Padding(15, 0, 15, 0);
            colMaSV.HeaderCell.Style.Padding = new Padding(15, 0, 15, 0);

            DataGridViewTextBoxColumn colTenSV = new DataGridViewTextBoxColumn() { Name = "TenSV", HeaderText = "TÊN SINH VIÊN", Width = 180 };
            colTenSV.DefaultCellStyle.Padding = new Padding(15, 0, 15, 0);
            colTenSV.HeaderCell.Style.Padding = new Padding(15, 0, 15, 0);

            DataGridViewTextBoxColumn colTenDeTai = new DataGridViewTextBoxColumn() { Name = "TenDeTai", HeaderText = "TÊN ĐỀ TÀI", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
            colTenDeTai.DefaultCellStyle.Padding = new Padding(15, 0, 15, 0);
            colTenDeTai.HeaderCell.Style.Padding = new Padding(15, 0, 15, 0);

            dgvRecent.Columns.Add(colMaSV);
            dgvRecent.Columns.Add(colTenSV);
            dgvRecent.Columns.Add(colTenDeTai);
            
            foreach (DataGridViewColumn col in dgvRecent.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            pnlGrid.Controls.Add(dgvRecent);
            this.Controls.Add(pnlGrid);

            // RIGHT COLUMN (35%) - THÔNG BÁO
            Label lblAnnounceTitle = new Label();
            lblAnnounceTitle.Text = "📢 Thông báo";
            lblAnnounceTitle.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
            lblAnnounceTitle.Location = new Point(650, 410);
            lblAnnounceTitle.AutoSize = true;
            this.Controls.Add(lblAnnounceTitle);

            CardPanel pnlAnnounce = new CardPanel();
            pnlAnnounce.Size = new Size(310, 300);
            pnlAnnounce.Location = new Point(650, 450);
            pnlAnnounce.BackColor = Color.White;
            pnlAnnounce.Padding = new Padding(15);
            
            FlowLayoutPanel flpNotifications = new FlowLayoutPanel();
            flpNotifications.Name = "flpNotifications";
            flpNotifications.Dock = DockStyle.Fill;
            flpNotifications.FlowDirection = FlowDirection.TopDown;
            flpNotifications.WrapContents = false;
            flpNotifications.AutoScroll = true;
            
            Label lblNoNotify = new Label();
            lblNoNotify.Name = "lblNoNotify";
            lblNoNotify.Text = "Đang chờ dữ liệu thông báo...";
            lblNoNotify.Font = new Font("Segoe UI", 10f, FontStyle.Italic);
            lblNoNotify.ForeColor = ColorTranslator.FromHtml("#A0AEC0");
            lblNoNotify.AutoSize = false;
            lblNoNotify.Size = new Size(250, 250);
            lblNoNotify.TextAlign = ContentAlignment.MiddleCenter;
            
            flpNotifications.Controls.Add(lblNoNotify);
            pnlAnnounce.Controls.Add(flpNotifications);
            this.Controls.Add(pnlAnnounce);
        }

        // HÀM TẠO THẺ KPI
        private CardPanel CreateKpiCard(string name, string icon, string num, string text, string badge, int x, int y, Color badgeColor, EventHandler onClick = null)
        {
            CardPanel pnl = new CardPanel();
            pnl.Size = new Size(280, 130); // 3 thẻ dàn trải cho 920px (280*3 + 2 khoảng trống 40px)
            pnl.Location = new Point(x, y);
            pnl.BackColor = Color.White;
            if (onClick != null)
            {
                pnl.Cursor = Cursors.Hand;
                pnl.Click += onClick;
            }

            Label lblIcon = new Label();
            lblIcon.Text = icon;
            lblIcon.Font = new Font("Segoe UI", 18f);
            lblIcon.Location = new Point(20, 20);
            lblIcon.AutoSize = true;
            if (onClick != null) lblIcon.Click += onClick;
            pnl.Controls.Add(lblIcon);

            Label lblNum = new Label();
            lblNum.Name = name; // Để dễ dàng gán dữ liệu từ CSDL
            lblNum.Text = num;
            lblNum.Font = new Font("Segoe UI", 26f, FontStyle.Bold);
            lblNum.Location = new Point(15, 50);
            lblNum.AutoSize = true;
            if (onClick != null) lblNum.Click += onClick;
            pnl.Controls.Add(lblNum);

            Label lblText = new Label();
            lblText.Text = text;
            lblText.Font = new Font("Segoe UI", 10f, FontStyle.Regular);
            lblText.ForeColor = ColorTranslator.FromHtml("#718096");
            lblText.Location = new Point(20, 95);
            lblText.AutoSize = true;
            if (onClick != null) lblText.Click += onClick;
            pnl.Controls.Add(lblText);

            if (!string.IsNullOrEmpty(badge))
            {
                Label lblBadge = new Label();
                lblBadge.Text = badge;
                lblBadge.Font = new Font("Segoe UI", 7.5f, FontStyle.Bold);
                lblBadge.ForeColor = badgeColor;
                lblBadge.Location = new Point(180, 25);
                lblBadge.AutoSize = true;
                if (onClick != null) lblBadge.Click += onClick;
                pnl.Controls.Add(lblBadge);
            }

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
