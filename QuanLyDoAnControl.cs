using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

#nullable disable

namespace TBUProject
{
    public class QuanLyDoAnControl : UserControl
    {
        // Controls
        private Label lblTitle;
        private Label lblSubtitle;
        
        private RoundedPanel pnlToolbar;
        private ComboBox cbSemester;
        private ComboBox cbDepartment;
        private ComboBox cbStatus;
        private RoundedButton btnExport;

        private RoundedPanel pnlGridContainer;
        private DataGridView dgvProjectList;

        private Panel pnlPagination;
        private FlowLayoutPanel flpPagination;

        public QuanLyDoAnControl()
        {
            InitializeCustomComponent();
            LoadDummyData();
        }

        private void InitializeCustomComponent()
        {
            // Main Settings
            this.BackColor = ColorTranslator.FromHtml("#F7FAFC"); // Light Gray Background
            this.Dock = DockStyle.Fill;
            this.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
            this.Size = new Size(1000, 700);
            this.Padding = new Padding(30);

            lblTitle = new Label();
            lblTitle.Text = "Quản lý danh sách Đồ án";
            lblTitle.ForeColor = ColorTranslator.FromHtml("#1A202C");
            lblTitle.Font = new Font("Segoe UI", 16f, FontStyle.Bold);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(30, 30);

            lblSubtitle = new Label();
            lblSubtitle.Text = "Theo dõi vòng đời, thông tin chi tiết và kho lưu trữ tài liệu của các đồ án đang thực hiện.";
            lblSubtitle.ForeColor = ColorTranslator.FromHtml("#718096");
            lblSubtitle.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            lblSubtitle.AutoSize = true;
            lblSubtitle.Location = new Point(30, 65);

            // ==========================================
            // 2. Filter & Action Toolbar (Rounded Panel / GroupBox Style)
            // ==========================================
            pnlToolbar = new RoundedPanel();
            pnlToolbar.BackColor = Color.White;
            pnlToolbar.BorderRadius = 12;
            pnlToolbar.Location = new Point(30, 110);
            pnlToolbar.Width = this.Width - 60;
            pnlToolbar.Height = 52;
            pnlToolbar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            cbSemester = CreateComboBox(new string[] { "Tất cả học kỳ" });
            cbSemester.Location = new Point(15, 11);
            cbSemester.Width = 120;

            cbDepartment = CreateComboBox(new string[] { "Bộ môn" });
            cbDepartment.Location = new Point(150, 11);
            cbDepartment.Width = 100;

            cbStatus = CreateComboBox(new string[] { "Đang thực hiện", "Đã hoàn thành", "Tạm dừng" });
            cbStatus.Location = new Point(265, 11);
            cbStatus.Width = 150;

            btnExport = new RoundedButton();
            btnExport.Text = "Xuất dữ liệu";
            btnExport.BackColor = ColorTranslator.FromHtml("#3182CE"); // Navy Blue
            btnExport.ForeColor = Color.White;
            btnExport.FlatStyle = FlatStyle.Flat;
            btnExport.BorderRadius = 8;
            btnExport.FlatAppearance.BorderSize = 0;
            btnExport.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btnExport.Size = new Size(110, 35);
            btnExport.Location = new Point(pnlToolbar.Width - 125, 8);
            btnExport.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExport.Cursor = Cursors.Hand;

            pnlToolbar.Controls.Add(cbSemester);
            pnlToolbar.Controls.Add(cbDepartment);
            pnlToolbar.Controls.Add(cbStatus);
            pnlToolbar.Controls.Add(btnExport);

            // ==========================================
            // 3. Main Data Grid Area
            // ==========================================
            pnlGridContainer = new RoundedPanel();
            pnlGridContainer.BackColor = Color.White;
            pnlGridContainer.Location = new Point(30, 177);
            pnlGridContainer.Width = this.Width - 60;
            pnlGridContainer.Height = this.Height - 257; 
            pnlGridContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlGridContainer.Padding = new Padding(2); // Border padding

            dgvProjectList = new DataGridView();
            dgvProjectList.Dock = DockStyle.Fill;
            dgvProjectList.BorderStyle = BorderStyle.None;
            dgvProjectList.BackgroundColor = Color.White;
            dgvProjectList.RowHeadersVisible = false;
            dgvProjectList.AllowUserToAddRows = false;
            dgvProjectList.AllowUserToDeleteRows = false;
            dgvProjectList.AllowUserToResizeRows = false;
            dgvProjectList.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvProjectList.GridColor = ColorTranslator.FromHtml("#EDF2F7"); // Light Gray Gridlines
            dgvProjectList.RowTemplate.Height = 70; // Spacious height for double lines
            dgvProjectList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProjectList.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#F7FAFC");
            dgvProjectList.DefaultCellStyle.SelectionForeColor = ColorTranslator.FromHtml("#1A202C");
            
            // Header Styling
            dgvProjectList.EnableHeadersVisualStyles = false;
            dgvProjectList.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvProjectList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvProjectList.ColumnHeadersHeight = 45;
            dgvProjectList.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dgvProjectList.ColumnHeadersDefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#A0AEC0");
            dgvProjectList.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            dgvProjectList.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.White;
            dgvProjectList.ColumnHeadersDefaultCellStyle.Padding = new Padding(18, 0, 0, 0);

            // Define Columns
            dgvProjectList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ProjectCode", HeaderText = "MÃ ĐỒ ÁN / SV", FillWeight = 30, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvProjectList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "TopicMajor", HeaderText = "TÊN ĐỀ TÀI & CHUYÊN NGÀNH", FillWeight = 45, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvProjectList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Status", HeaderText = "TRẠNG THÁI", FillWeight = 20, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvProjectList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Actions", HeaderText = "CHI TIẾT", FillWeight = 15, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            
            // Disable sorting
            foreach (DataGridViewColumn col in dgvProjectList.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            // Custom Cell Painting
            dgvProjectList.CellPainting += DgvProjectList_CellPainting;

            pnlGridContainer.Controls.Add(dgvProjectList);

            // ==========================================
            // 4. Bottom Pagination Bar
            // ==========================================
            pnlPagination = new Panel();
            pnlPagination.Height = 40;
            pnlPagination.Width = this.Width - 60;
            pnlPagination.Location = new Point(30, this.Height - 70);
            pnlPagination.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            flpPagination = new FlowLayoutPanel();
            flpPagination.FlowDirection = FlowDirection.LeftToRight;
            flpPagination.WrapContents = false;
            flpPagination.AutoSize = true;
            flpPagination.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            
            // Add pagination buttons
            flpPagination.Controls.Add(CreatePageButton("<", false));
            flpPagination.Controls.Add(CreatePageButton("1", true));
            flpPagination.Controls.Add(CreatePageButton("2", false));
            flpPagination.Controls.Add(CreatePageButton("3", false));
            
            Label lblEllipsis = new Label() { Text = "...", AutoSize = true, Margin = new Padding(2, 10, 2, 2) };
            flpPagination.Controls.Add(lblEllipsis);
            
            flpPagination.Controls.Add(CreatePageButton("5", false));
            flpPagination.Controls.Add(CreatePageButton(">", false));

            flpPagination.PerformLayout(); 
            flpPagination.Location = new Point(pnlPagination.Width - flpPagination.Width, 5); 
            flpPagination.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            pnlPagination.Controls.Add(flpPagination);

            // ==========================================
            // Add all to UserControl
            // ==========================================
            this.Controls.Add(pnlPagination);
            this.Controls.Add(pnlGridContainer);
            this.Controls.Add(pnlToolbar);
            this.Controls.Add(lblSubtitle);
            this.Controls.Add(lblTitle);
        }

        private ComboBox CreateComboBox(string[] items)
        {
            ComboBox cb = new ComboBox();
            cb.DropDownStyle = ComboBoxStyle.DropDownList;
            cb.Items.AddRange(items);
            if (items.Length > 0) cb.SelectedIndex = 0;
            cb.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            cb.ForeColor = ColorTranslator.FromHtml("#4A5568");
            cb.BackColor = Color.White;
            cb.FlatStyle = FlatStyle.Flat; 
            return cb;
        }

        private RoundedButton CreatePageButton(string text, bool isActive)
        {
            RoundedButton btn = new RoundedButton();
            btn.Text = text;
            btn.Size = new Size(32, 32);
            btn.FlatStyle = FlatStyle.Flat;
            btn.BorderRadius = 8;
            btn.Font = new Font("Segoe UI", 9.5f, isActive ? FontStyle.Bold : FontStyle.Regular);
            btn.Margin = new Padding(2);
            btn.Cursor = Cursors.Hand;
            
            if (isActive)
            {
                btn.BackColor = ColorTranslator.FromHtml("#3182CE"); // Solid Blue
                btn.ForeColor = Color.White;
                btn.BorderColor = ColorTranslator.FromHtml("#3182CE");
            }
            else
            {
                btn.BackColor = Color.White;
                btn.ForeColor = ColorTranslator.FromHtml("#3182CE"); // Blue text
                btn.BorderColor = ColorTranslator.FromHtml("#E2E8F0");
            }
            return btn;
        }

        private void LoadDummyData()
        {
            // Format: Code|StudentName-ID | TopicName|Major | Status
            dgvProjectList.Rows.Add("DA_2023_001|Trần Văn Bình - 2011001", "Nghiên cứu ứng dụng AI trong chẩn đoán y...|Ngành: CNTT", "Đang thực hiện", "");
            dgvProjectList.Rows.Add("DA_2023_045|Nguyễn Thị Mai - 2011045", "Hệ thống quản lý chuỗi cung ứng Blockchain|Ngành: CNTT", "Đã hoàn thành", "");
            dgvProjectList.Rows.Add("DA_2023_082|Lê Hoàng Nam - 2011082", "Phát triển ứng dụng Mobile thương mại điệ...|Ngành: CNTT", "Tạm dừng", "");
            dgvProjectList.Rows.Add("DA_2023_115|Phạm Thu Hà - 2011115", "Phân tích dữ liệu mạng xã hội cho Marketing|Ngành: CNTT", "Đang thực hiện", "");
        }

        private void DgvProjectList_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return; // Ignore headers

            // Paint default background and selection
            e.PaintBackground(e.CellBounds, (e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected);

            string[] parts = e.Value?.ToString()?.Split('|') ?? new string[0];
            
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int padLeft = 15; // Left padding for cell content

            if (e.ColumnIndex == 0 && parts.Length >= 2)
            {
                // PROJECT CODE & STUDENT
                using (SolidBrush codeBrush = new SolidBrush(ColorTranslator.FromHtml("#1A202C")))
                using (SolidBrush studentBrush = new SolidBrush(ColorTranslator.FromHtml("#718096")))
                {
                    g.DrawString(parts[0], new Font("Segoe UI", 9.5f, FontStyle.Bold), codeBrush, e.CellBounds.X + padLeft, e.CellBounds.Y + 15);
                    g.DrawString(parts[1], new Font("Segoe UI", 8.5f, FontStyle.Regular), studentBrush, e.CellBounds.X + padLeft, e.CellBounds.Y + 38);
                }
                e.Handled = true;
            }
            else if (e.ColumnIndex == 1 && parts.Length >= 2)
            {
                // TOPIC & MAJOR
                using (SolidBrush topicBrush = new SolidBrush(ColorTranslator.FromHtml("#1A202C")))
                {
                    StringFormat sf = new StringFormat
                    {
                        Trimming = StringTrimming.EllipsisCharacter,
                        FormatFlags = StringFormatFlags.NoWrap
                    };
                    RectangleF titleRect = new RectangleF(e.CellBounds.X + padLeft, e.CellBounds.Y + 15, e.CellBounds.Width - padLeft - 5, 20);
                    g.DrawString(parts[0], new Font("Segoe UI", 9.5f, FontStyle.Bold), topicBrush, titleRect, sf);
                }

                // MAJOR
                string major = parts[1];
                using (SolidBrush majorBrush = new SolidBrush(ColorTranslator.FromHtml("#A0AEC0")))
                {
                    g.DrawString(major, new Font("Segoe UI", 8.5f, FontStyle.Regular), majorBrush, e.CellBounds.X + padLeft, e.CellBounds.Y + 38);
                }

                e.Handled = true;
            }
            else if (e.ColumnIndex == 2 && e.Value != null)
            {
                // STATUS
                string status = e.Value.ToString();
                Color badgeBgColor = Color.Transparent;
                Color badgeTextColor = ColorTranslator.FromHtml("#3182CE");
                Color dotColor = ColorTranslator.FromHtml("#3182CE");

                if (status == "Đã hoàn thành")
                {
                    badgeBgColor = ColorTranslator.FromHtml("#F0FFF4");
                    badgeTextColor = ColorTranslator.FromHtml("#38A169");
                    dotColor = ColorTranslator.FromHtml("#38A169");
                }
                else if (status == "Tạm dừng")
                {
                    badgeBgColor = ColorTranslator.FromHtml("#F7FAFC");
                    badgeTextColor = ColorTranslator.FromHtml("#718096");
                    dotColor = ColorTranslator.FromHtml("#A0AEC0");
                }
                else // Đang thực hiện
                {
                    badgeBgColor = ColorTranslator.FromHtml("#EBF8FF");
                    badgeTextColor = ColorTranslator.FromHtml("#3182CE");
                    dotColor = ColorTranslator.FromHtml("#3182CE");
                }

                // Measure text for badge
                SizeF textSize = g.MeasureString(status, new Font("Segoe UI", 8.5f, FontStyle.Bold));
                int badgeWidth = (int)textSize.Width + 28; // Space for dot and padding
                int badgeHeight = 26;
                Rectangle badgeRect = new Rectangle(e.CellBounds.X + padLeft, e.CellBounds.Y + (e.CellBounds.Height - badgeHeight) / 2, badgeWidth, badgeHeight);

                using (GraphicsPath path = GetRoundedPath(badgeRect, 13))
                using (SolidBrush bgBrush = new SolidBrush(badgeBgColor))
                {
                    g.FillPath(bgBrush, path);
                }

                // Draw dot
                using (SolidBrush dotBrush = new SolidBrush(dotColor))
                {
                    g.FillEllipse(dotBrush, badgeRect.X + 10, badgeRect.Y + 10, 6, 6);
                }

                // Draw text
                using (SolidBrush textBrush = new SolidBrush(badgeTextColor))
                {
                    g.DrawString(status, new Font("Segoe UI", 8.5f, FontStyle.Bold), textBrush, badgeRect.X + 22, badgeRect.Y + 4);
                }
                
                e.Handled = true;
            }
            else if (e.ColumnIndex == 3)
            {
                // ACTIONS
                using (SolidBrush textBrush = new SolidBrush(ColorTranslator.FromHtml("#4A5568")))
                {
                    g.DrawString("Xem chi tiết", new Font("Segoe UI", 9f, FontStyle.Bold), textBrush, e.CellBounds.X + padLeft, e.CellBounds.Y + 22);
                }
                e.Handled = true;
            }
            
            // Draw grid line for all cells
            using (Pen gridPen = new Pen(dgvProjectList.GridColor, 1))
            {
                g.DrawLine(gridPen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);
            }
        }

        // --- Helper Methods ---
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

        // ==========================================
        // Inner Class: Rounded Panel
        // ==========================================
        private class RoundedPanel : Panel
        {
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public int BorderRadius { get; set; } = 12;

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

                using (GraphicsPath path = GetRoundedPath(rect, BorderRadius))
                {
                    // Draw White Background
                    using (SolidBrush brush = new SolidBrush(this.BackColor))
                    {
                        e.Graphics.FillPath(brush, path);
                    }

                    // Draw Light Gray Border
                    using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1.5f))
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

        // ==========================================
        // Inner Class: Rounded Button
        // ==========================================
        private class RoundedButton : Button
        {
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public int BorderRadius { get; set; } = 8;
            
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public Color BorderColor { get; set; } = Color.Transparent;

            private bool isHovered = false;

            public RoundedButton()
            {
                this.MouseEnter += (s, e) => { isHovered = true; this.Invalidate(); };
                this.MouseLeave += (s, e) => { isHovered = false; this.Invalidate(); };
                this.FlatAppearance.BorderSize = 0;
                this.FlatStyle = FlatStyle.Flat;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

                using (GraphicsPath path = GetRoundedPath(rect, BorderRadius))
                {
                    this.Region = new Region(path);

                    // Hover effect
                    Color fillOuter = isHovered ? ControlPaint.Light(this.BackColor, 0.2f) : this.BackColor;

                    using (SolidBrush brush = new SolidBrush(fillOuter))
                    {
                        e.Graphics.FillPath(brush, path);
                    }

                    // Draw Border
                    Color drawBorderColor = BorderColor != Color.Transparent ? BorderColor : 
                                            (this.BackColor == Color.White ? ColorTranslator.FromHtml("#E2E8F0") : Color.Transparent);

                    if (drawBorderColor != Color.Transparent)
                    {
                        using (Pen pen = new Pen(drawBorderColor, 1))
                        {
                            e.Graphics.DrawPath(pen, path);
                        }
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

    }
}
