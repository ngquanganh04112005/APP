using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

#nullable disable

namespace TBUProject
{
    public class DuyetDeTaiControl : UserControl
    {
        // Controls
        private Label lblTitle;
        private Label lblSubtitle;
        
        private RoundedPanel pnlToolbar;
        private ComboBox cbStatus;
        private ComboBox cbAcademicYear;
        private RoundedButton btnExport;

        private RoundedPanel pnlGridContainer;
        private DataGridView dgvThesisList;

        private Panel pnlPagination;
        private FlowLayoutPanel flpPagination;

        public DuyetDeTaiControl()
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
            lblTitle.Text = "Duyệt đề tài";
            lblTitle.ForeColor = ColorTranslator.FromHtml("#1A202C");
            lblTitle.Font = new Font("Segoe UI", 16f, FontStyle.Bold);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(30, 30);

            lblSubtitle = new Label();
            lblSubtitle.Text = "Xem và quản lý danh sách đề tài đồ án được sinh viên nộp trong học kỳ hiện tại.";
            lblSubtitle.ForeColor = ColorTranslator.FromHtml("#718096");
            lblSubtitle.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            lblSubtitle.AutoSize = true;
            lblSubtitle.Location = new Point(30, 60);

            // ==========================================
            // 2. Filter & Action Toolbar (Rounded Panel / GroupBox Style)
            // ==========================================
            pnlToolbar = new RoundedPanel();
            pnlToolbar.BackColor = Color.White;
            pnlToolbar.BorderRadius = 12;
            pnlToolbar.Location = new Point(30, 105);
            pnlToolbar.Width = this.Width - 60;
            pnlToolbar.Height = 52;
            pnlToolbar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            cbStatus = CreateComboBox(new string[] { "Tất cả trạng thái", "Chờ duyệt", "Đã duyệt" });
            cbStatus.Location = new Point(15, 11);
            
            cbAcademicYear = CreateComboBox(new string[] { "2023 - 2024", "2024 - 2025" });
            cbAcademicYear.Location = new Point(180, 11);

            btnExport = new RoundedButton();
            btnExport.Text = "📥 Xuất danh sách";
            btnExport.BackColor = ColorTranslator.FromHtml("#3182CE"); // Navy Blue
            btnExport.ForeColor = Color.White;
            btnExport.FlatStyle = FlatStyle.Flat;
            btnExport.BorderRadius = 8;
            btnExport.FlatAppearance.BorderSize = 0;
            btnExport.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btnExport.Size = new Size(150, 35);
            btnExport.Location = new Point(pnlToolbar.Width - 165, 8);
            btnExport.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExport.Cursor = Cursors.Hand;

            pnlToolbar.Controls.Add(cbStatus);
            pnlToolbar.Controls.Add(cbAcademicYear);
            pnlToolbar.Controls.Add(btnExport);

            // ==========================================
            // 3. Main Data Grid Area
            // ==========================================
            pnlGridContainer = new RoundedPanel();
            pnlGridContainer.BackColor = Color.White;
            pnlGridContainer.Location = new Point(30, 172);
            pnlGridContainer.Width = this.Width - 60;
            pnlGridContainer.Height = this.Height - 252; // Đã điều chỉnh cho vị trí mới
            pnlGridContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlGridContainer.Padding = new Padding(2); // Border padding

            dgvThesisList = new DataGridView();
            dgvThesisList.Dock = DockStyle.Fill;
            dgvThesisList.BorderStyle = BorderStyle.None;
            dgvThesisList.BackgroundColor = Color.White;
            dgvThesisList.RowHeadersVisible = false;
            dgvThesisList.AllowUserToAddRows = false;
            dgvThesisList.AllowUserToDeleteRows = false;
            dgvThesisList.AllowUserToResizeRows = false;
            dgvThesisList.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvThesisList.GridColor = ColorTranslator.FromHtml("#EDF2F7"); // Light Gray Gridlines
            dgvThesisList.RowTemplate.Height = 60; // Spacious height
            dgvThesisList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvThesisList.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#F7FAFC");
            dgvThesisList.DefaultCellStyle.SelectionForeColor = ColorTranslator.FromHtml("#1A202C");
            
            // Header Styling
            dgvThesisList.EnableHeadersVisualStyles = false;
            dgvThesisList.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvThesisList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvThesisList.ColumnHeadersHeight = 45;
            dgvThesisList.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dgvThesisList.ColumnHeadersDefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#A0AEC0");
            dgvThesisList.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            dgvThesisList.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.White;
            dgvThesisList.ColumnHeadersDefaultCellStyle.Padding = new Padding(18, 0, 0, 0); // Căn lề trái cho header khớp với nội dung ô

            // Define Columns
            dgvThesisList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "StudentInfo", HeaderText = "THÔNG TIN SINH VIÊN", FillWeight = 20, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvThesisList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ThesisTitle", HeaderText = "TÊN ĐỀ TÀI", FillWeight = 37, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvThesisList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "SubmittedDate", HeaderText = "NGÀY NỘP", FillWeight = 15, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvThesisList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Status", HeaderText = "TRẠNG THÁI", FillWeight = 15, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvThesisList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Actions", HeaderText = "CHI TIẾT", FillWeight = 10, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvThesisList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Margin", HeaderText = "", FillWeight = 3, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            
            // Set explicit padding for StudentInfo to align with the left edge (where avatar starts)
            dgvThesisList.Columns["StudentInfo"].HeaderCell.Style.Padding = new Padding(18, 0, 0, 0);
            
            // Disable sorting to keep data consistent with manual render
            foreach (DataGridViewColumn col in dgvThesisList.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            // Custom Cell Painting
            dgvThesisList.CellPainting += DgvThesisList_CellPainting;

            pnlGridContainer.Controls.Add(dgvThesisList);

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
            flpPagination.Controls.Add(CreatePageButton(">", false));

            flpPagination.PerformLayout(); // Calculate width
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
            cb.SelectedIndex = 0;
            cb.Width = 150;
            cb.Font = new Font("Segoe UI", 10f, FontStyle.Regular);
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
                btn.BackColor = ColorTranslator.FromHtml("#3182CE"); // Professional Blue
                btn.ForeColor = Color.White;
            }
            else
            {
                btn.BackColor = Color.White;
                btn.ForeColor = ColorTranslator.FromHtml("#3182CE"); // Blue text
            }
            return btn;
        }

        private void LoadDummyData()
        {
            // data format: StudentName|StudentID | ThesisTitle|Major | Date|Time | Status
            dgvThesisList.Rows.Add("Trần Bình An|1810239", "Ứng dụng AI trong bảo trì dự báo rủi ro công nghiệp|Ngành: CNTT", "24 Th10, 2023|10:30 AM", "Chờ duyệt", "");
            dgvThesisList.Rows.Add("Trần Trường Sinh|1810255", "Blockchain trong minh bạch chuỗi cung ứng|Ngành: CNTT", "23 Th10, 2023|02:15 PM", "Đã duyệt", "");
            dgvThesisList.Rows.Add("Lệ Phi Vũ|1915022", "Tối ưu hóa công cụ gợi ý thương mại điện tử|Ngành: CNTT", "22 Th10, 2023|09:45 AM", "Chờ duyệt", "");
        }

        private void DgvThesisList_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
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
                // STUDENT INFO
                // 1. Draw Avatar Circle
                Rectangle avatarRect = new Rectangle(e.CellBounds.X + padLeft, e.CellBounds.Y + 10, 40, 40);
                
                using (SolidBrush bgBrush = new SolidBrush(GetAvatarColor(parts[0])))
                {
                    g.FillEllipse(bgBrush, avatarRect);
                }
                
                // 2. Draw Initials
                string initials = GetInitials(parts[0]);
                using (SolidBrush textBrush = new SolidBrush(GetAvatarTextColor(parts[0])))
                {
                    StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(initials, new Font("Segoe UI", 9.5f, FontStyle.Bold), textBrush, avatarRect, sf);
                }

                // 3. Draw Name and ID
                using (SolidBrush nameBrush = new SolidBrush(ColorTranslator.FromHtml("#1A202C")))
                using (SolidBrush idBrush = new SolidBrush(ColorTranslator.FromHtml("#A0AEC0")))
                {
                    g.DrawString(parts[0], new Font("Segoe UI", 9.5f, FontStyle.Bold), nameBrush, e.CellBounds.X + padLeft + 55, e.CellBounds.Y + 12);
                    g.DrawString("MSSV: " + parts[1], new Font("Segoe UI", 8.5f, FontStyle.Regular), idBrush, e.CellBounds.X + padLeft + 55, e.CellBounds.Y + 32);
                }
                e.Handled = true;
            }
            else if (e.ColumnIndex == 1 && parts.Length >= 2)
            {
                // THESIS TITLE (Single line with ellipsis)
                using (SolidBrush titleBrush = new SolidBrush(ColorTranslator.FromHtml("#1A202C")))
                {
                    StringFormat sf = new StringFormat
                    {
                        Trimming = StringTrimming.EllipsisCharacter,
                        FormatFlags = StringFormatFlags.NoWrap
                    };
                    RectangleF titleRect = new RectangleF(e.CellBounds.X + padLeft, e.CellBounds.Y + 12, e.CellBounds.Width - padLeft - 15, 20);
                    g.DrawString(parts[0], new Font("Segoe UI", 9.5f, FontStyle.Bold), titleBrush, titleRect, sf);
                }

                // MAJOR
                using (SolidBrush majorBrush = new SolidBrush(ColorTranslator.FromHtml("#A0AEC0")))
                {
                    g.DrawString(parts[1], new Font("Segoe UI", 8.5f, FontStyle.Regular), majorBrush, e.CellBounds.X + padLeft, e.CellBounds.Y + 32);
                }
                e.Handled = true;
            }
            else if (e.ColumnIndex == 2 && parts.Length >= 2)
            {
                // SUBMITTED DATE
                using (SolidBrush dateBrush = new SolidBrush(ColorTranslator.FromHtml("#4A5568")))
                using (SolidBrush timeBrush = new SolidBrush(ColorTranslator.FromHtml("#A0AEC0")))
                {
                    g.DrawString(parts[0], new Font("Segoe UI", 9.5f, FontStyle.Regular), dateBrush, e.CellBounds.X + padLeft, e.CellBounds.Y + 12);
                    g.DrawString(parts[1], new Font("Segoe UI", 8.5f, FontStyle.Regular), timeBrush, e.CellBounds.X + padLeft, e.CellBounds.Y + 32);
                }
                e.Handled = true;
            }
            else if (e.ColumnIndex == 3 && e.Value != null)
            {
                // STATUS
                string status = e.Value.ToString();
                Color badgeBgColor = status == "Đã duyệt" ? ColorTranslator.FromHtml("#C6F6D5") : ColorTranslator.FromHtml("#FEFCBF");
                Color badgeTextColor = status == "Đã duyệt" ? ColorTranslator.FromHtml("#22543D") : ColorTranslator.FromHtml("#B7791F");
                Color dotColor = status == "Đã duyệt" ? ColorTranslator.FromHtml("#38A169") : ColorTranslator.FromHtml("#D69E2E");

                int badgeWidth = status == "Đã duyệt" ? 85 : 90;
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
            else if (e.ColumnIndex == 4)
            {
                // ACTIONS
                using (SolidBrush textBrush = new SolidBrush(ColorTranslator.FromHtml("#4A5568")))
                {
                    g.DrawString("Xem chi tiết", new Font("Segoe UI", 9f, FontStyle.Bold), textBrush, e.CellBounds.X + padLeft, e.CellBounds.Y + 22);
                }
                e.Handled = true;
            }
            
            // Draw grid line for all cells
            using (Pen gridPen = new Pen(dgvThesisList.GridColor, 1))
            {
                g.DrawLine(gridPen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);
            }
        }

        // --- Helper Methods ---

        private Color GetAvatarColor(string name)
        {
            if (name.StartsWith("H")) return ColorTranslator.FromHtml("#EBF8FF"); // Light Blue
            if (name.StartsWith("T")) return ColorTranslator.FromHtml("#F0FFF4"); // Light Green
            if (name.StartsWith("L")) return ColorTranslator.FromHtml("#FAF5FF"); // Light Purple
            return ColorTranslator.FromHtml("#EDF2F7"); // Default Gray
        }

        private Color GetAvatarTextColor(string name)
        {
            if (name.StartsWith("H")) return ColorTranslator.FromHtml("#2B6CB0"); // Blue text
            if (name.StartsWith("T")) return ColorTranslator.FromHtml("#2F855A"); // Green text
            if (name.StartsWith("L")) return ColorTranslator.FromHtml("#6B46C1"); // Purple text
            return ColorTranslator.FromHtml("#4A5568"); // Default Dark Gray text
        }

        private string GetInitials(string name)
        {
            string[] parts = name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2) return (parts[0][0].ToString() + parts[parts.Length-1][0].ToString()).ToUpper();
            if (parts.Length == 1 && parts[0].Length > 0) return parts[0].Substring(0, 1).ToUpper();
            return "U";
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

                    // Vẽ viền nếu không phải active/primary color
                    if (this.BackColor == Color.White)
                    {
                        using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1))
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
