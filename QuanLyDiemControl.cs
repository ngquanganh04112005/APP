using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

#nullable disable

namespace TBUProject
{
    public class QuanLyDiemControl : UserControl
    {
        // Controls
        private Label lblTitle;
        private Label lblSubtitle;
        
        private RoundedPanel pnlToolbar;
        private ComboBox cbSubject;
        private ComboBox cbProgress;
        private ComboBox cbAcademicYear;
        private RoundedButton btnExport;

        private RoundedPanel pnlGridContainer;
        private DataGridView dgvThesisList;

        private Panel pnlPagination;
        private FlowLayoutPanel flpPagination;

        public QuanLyDiemControl()
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
            lblTitle.Text = "Quản lý điểm - Nhận xét";
            lblTitle.ForeColor = ColorTranslator.FromHtml("#1A202C");
            lblTitle.Font = new Font("Segoe UI", 16f, FontStyle.Bold);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(30, 30);

            lblSubtitle = new Label();
            lblSubtitle.Text = "Nhập điểm thành phần, điểm phản biện và theo dõi kết quả học tập của sinh viên.";
            lblSubtitle.ForeColor = ColorTranslator.FromHtml("#718096");
            lblSubtitle.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            lblSubtitle.AutoSize = true;
            lblSubtitle.Location = new Point(30, 60);

            // ==========================================
            // 2. Filter & Action Toolbar
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

            cbSubject = CreateComboBox(new string[] { "Tất cả học phần", "Đồ án cơ sở", "Đồ án chuyên ngành", "Khóa luận tốt nghiệp" });
            cbSubject.Location = new Point(15, 11);
            
            cbProgress = CreateComboBox(new string[] { "Tất cả tiến độ", "Chưa nhập điểm", "Đã nhập điểm" });
            cbProgress.Location = new Point(180, 11);

            cbAcademicYear = CreateComboBox(new string[] { "2023 - 2024", "2024 - 2025" });
            cbAcademicYear.Location = new Point(345, 11);
            cbAcademicYear.Width = 120;

            btnExport = new RoundedButton();
            btnExport.Text = "📄 Xuất bảng điểm";
            btnExport.BackColor = ColorTranslator.FromHtml("#3182CE"); // Blue color matching DuyetDeTai
            btnExport.ForeColor = Color.White;
            btnExport.FlatStyle = FlatStyle.Flat;
            btnExport.BorderRadius = 8;
            btnExport.FlatAppearance.BorderSize = 0;
            btnExport.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btnExport.Size = new Size(150, 35);
            btnExport.Location = new Point(pnlToolbar.Width - 165, 8);
            btnExport.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExport.Cursor = Cursors.Hand;

            pnlToolbar.Controls.Add(cbSubject);
            pnlToolbar.Controls.Add(cbProgress);
            pnlToolbar.Controls.Add(cbAcademicYear);
            pnlToolbar.Controls.Add(btnExport);

            // ==========================================
            // 3. Main Data Grid Area
            // ==========================================
            pnlGridContainer = new RoundedPanel();
            pnlGridContainer.BackColor = Color.White;
            pnlGridContainer.Location = new Point(30, 172);
            pnlGridContainer.Width = this.Width - 60;
            pnlGridContainer.Height = this.Height - 252; 
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
            dgvThesisList.RowTemplate.Height = 70; // Slightly taller for tags and multiline title
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

            // Define Columns
            var colStudent = new DataGridViewTextBoxColumn() { Name = "StudentInfo", HeaderText = "THÔNG TIN SINH VIÊN", FillWeight = 25, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
            colStudent.HeaderCell.Style.Padding = new Padding(18, 0, 0, 0);
            dgvThesisList.Columns.Add(colStudent);

            var colThesis = new DataGridViewTextBoxColumn() { Name = "ThesisTitle", HeaderText = "TÊN ĐỀ TÀI", FillWeight = 35, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
            colThesis.HeaderCell.Style.Padding = new Padding(18, 0, 0, 0);
            dgvThesisList.Columns.Add(colThesis);

            var colProgress = new DataGridViewTextBoxColumn() { Name = "ProgressScore", HeaderText = "ĐIỂM\nTIẾN ĐỘ", FillWeight = 10, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
            colProgress.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvThesisList.Columns.Add(colProgress);

            var colDefense = new DataGridViewTextBoxColumn() { Name = "DefenseScore", HeaderText = "ĐIỂM\nĐỀ TÀI", FillWeight = 10, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
            colDefense.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvThesisList.Columns.Add(colDefense);

            var colStatus = new DataGridViewTextBoxColumn() { Name = "Status", HeaderText = "TRẠNG THÁI", FillWeight = 15, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
            colStatus.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvThesisList.Columns.Add(colStatus);

            var colActions = new DataGridViewTextBoxColumn() { Name = "Actions", HeaderText = "CHI TIẾT", FillWeight = 10, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
            colActions.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvThesisList.Columns.Add(colActions);
            
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
            pnlPagination.Height = 44;
            pnlPagination.Width = this.Width - 60;
            pnlPagination.Location = new Point(30, this.Height - 70);
            pnlPagination.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlPagination.Padding = new Padding(0, 4, 10, 4);

            flpPagination = new FlowLayoutPanel();
            flpPagination.FlowDirection = FlowDirection.LeftToRight;
            flpPagination.WrapContents = false;
            flpPagination.AutoSize = true;
            flpPagination.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flpPagination.Dock = DockStyle.Right;
            
            // Add pagination buttons
            flpPagination.Controls.Add(CreatePageButton("<", false));
            flpPagination.Controls.Add(CreatePageButton("1", true));
            flpPagination.Controls.Add(CreatePageButton("2", false));
            flpPagination.Controls.Add(CreatePageButton("3", false));
            
            Label lblEllipsis = new Label() { 
                Text = "...", 
                AutoSize = true, 
                ForeColor = ColorTranslator.FromHtml("#3182CE"),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                Margin = new Padding(4, 10, 4, 2) 
            };
            flpPagination.Controls.Add(lblEllipsis);
            
            flpPagination.Controls.Add(CreatePageButton("5", false));
            flpPagination.Controls.Add(CreatePageButton(">", false));

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
                btn.BackColor = ColorTranslator.FromHtml("#3182CE"); // Blue color matching DuyetDeTai
                btn.ForeColor = Color.White;
            }
            else
            {
                btn.BackColor = Color.White;
                btn.ForeColor = ColorTranslator.FromHtml("#3182CE"); // Blue text
            }
            
            // Remove border for "..."
            if (text == "...")
            {
                btn.FlatAppearance.BorderSize = 0;
                btn.BackColor = Color.Transparent;
            }

            return btn;
        }

        private void LoadDummyData()
        {
            // data format: StudentName|StudentID | ThesisTitle|Major | ProgressScore|DefenseScore | Status
            dgvThesisList.Rows.Add("Trần Văn Hải|SV20201234", "Ứng dụng AI trong nhận diện khuôn mặt|Ngành: CNTT", "8.5", "-", "Chờ\nchấm điểm");
            dgvThesisList.Rows.Add("Nguyễn Thị Linh|SV20205678", "Xây dựng hệ thống quản lý thư viện thông minh|Ngành: CNTT", "9.0", "8.8", "Đã nhập\nđiểm");
            dgvThesisList.Rows.Add("Lê Hoàng Minh|SV20209012", "Phân tích dữ liệu mạng xã hội dự đoán xu hướng|Ngành: CNTT", "7.5", "-", "Chờ\nchấm điểm");
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
                Rectangle avatarRect = new Rectangle(e.CellBounds.X + padLeft, e.CellBounds.Y + 15, 40, 40);
                
                using (SolidBrush bgBrush = new SolidBrush(GetAvatarColor(parts[0])))
                {
                    g.FillEllipse(bgBrush, avatarRect);
                }
                
                string initials = GetInitials(parts[0]);
                using (SolidBrush textBrush = new SolidBrush(GetAvatarTextColor(parts[0])))
                {
                    StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(initials, new Font("Segoe UI", 9.5f, FontStyle.Bold), textBrush, avatarRect, sf);
                }

                using (SolidBrush nameBrush = new SolidBrush(ColorTranslator.FromHtml("#1A202C")))
                using (SolidBrush idBrush = new SolidBrush(ColorTranslator.FromHtml("#A0AEC0")))
                {
                    g.DrawString(parts[0], new Font("Segoe UI", 9.5f, FontStyle.Bold), nameBrush, e.CellBounds.X + padLeft + 55, e.CellBounds.Y + 18);
                    g.DrawString(parts[1], new Font("Segoe UI", 8.5f, FontStyle.Regular), idBrush, e.CellBounds.X + padLeft + 55, e.CellBounds.Y + 38);
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
                    RectangleF titleRect = new RectangleF(e.CellBounds.X + padLeft, e.CellBounds.Y + 14, e.CellBounds.Width - padLeft - 15, 22);
                    g.DrawString(parts[0], new Font("Segoe UI", 9.5f, FontStyle.Bold), titleBrush, titleRect, sf);
                }

                // MAJOR NAME (No background, plain text)
                string major = parts[1];
                using (SolidBrush majorBrush = new SolidBrush(ColorTranslator.FromHtml("#A0AEC0")))
                {
                    g.DrawString(major, new Font("Segoe UI", 8.5f, FontStyle.Regular), majorBrush, e.CellBounds.X + padLeft, e.CellBounds.Y + 38);
                }

                e.Handled = true;
            }
            else if (e.ColumnIndex == 2 || e.ColumnIndex == 3)
            {
                // SCORES
                using (SolidBrush textBrush = new SolidBrush(ColorTranslator.FromHtml("#1A202C")))
                {
                    StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(e.Value?.ToString() ?? "-", new Font("Segoe UI", 9.5f, FontStyle.Bold), textBrush, e.CellBounds, sf);
                }
                e.Handled = true;
            }
            else if (e.ColumnIndex == 4 && e.Value != null)
            {
                // STATUS (Multiline pill)
                string status = e.Value.ToString();
                bool isDone = status.Contains("Đã nhập");
                Color badgeBgColor = isDone ? ColorTranslator.FromHtml("#C6F6D5") : ColorTranslator.FromHtml("#FEFCBF");
                Color badgeTextColor = isDone ? ColorTranslator.FromHtml("#22543D") : ColorTranslator.FromHtml("#B7791F");
                Color dotColor = isDone ? ColorTranslator.FromHtml("#38A169") : ColorTranslator.FromHtml("#D69E2E");

                int badgeWidth = 90;
                int badgeHeight = 42; // Taller for two lines
                Rectangle badgeRect = new Rectangle(
                    e.CellBounds.X + (e.CellBounds.Width - badgeWidth) / 2, 
                    e.CellBounds.Y + (e.CellBounds.Height - badgeHeight) / 2, 
                    badgeWidth, badgeHeight);

                using (GraphicsPath path = GetRoundedPath(badgeRect, 20)) // Fully rounded pill
                using (SolidBrush bgBrush = new SolidBrush(badgeBgColor))
                {
                    g.FillPath(bgBrush, path);
                }

                // Draw dot
                using (SolidBrush dotBrush = new SolidBrush(dotColor))
                {
                    g.FillEllipse(dotBrush, badgeRect.X + 12, badgeRect.Y + 18, 5, 5);
                }

                // Draw text
                using (SolidBrush textBrush = new SolidBrush(badgeTextColor))
                {
                    StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    // Shift text rect slightly right to account for dot
                    Rectangle textRect = new Rectangle(badgeRect.X + 8, badgeRect.Y, badgeRect.Width - 8, badgeRect.Height);
                    g.DrawString(status, new Font("Segoe UI", 8.5f, FontStyle.Bold), textBrush, textRect, sf);
                }
                
                e.Handled = true;
            }
            else if (e.ColumnIndex == 5)
            {
                // ACTIONS
                using (SolidBrush textBrush = new SolidBrush(ColorTranslator.FromHtml("#4A5568")))
                {
                    StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString("Xem chi tiết", new Font("Segoe UI", 9f, FontStyle.Bold), textBrush, e.CellBounds, sf);
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
            if (name.StartsWith("H") || name.Contains("Hải")) return ColorTranslator.FromHtml("#EBF8FF"); // Light Blue
            if (name.StartsWith("L") || name.Contains("Linh")) return ColorTranslator.FromHtml("#F0FFF4"); // Light Green
            if (name.StartsWith("M") || name.Contains("Minh")) return ColorTranslator.FromHtml("#FAF5FF"); // Light Purple
            return ColorTranslator.FromHtml("#EDF2F7"); // Default Gray
        }

        private Color GetAvatarTextColor(string name)
        {
            if (name.StartsWith("H") || name.Contains("Hải")) return ColorTranslator.FromHtml("#2B6CB0"); // Blue text
            if (name.StartsWith("L") || name.Contains("Linh")) return ColorTranslator.FromHtml("#2F855A"); // Green text
            if (name.StartsWith("M") || name.Contains("Minh")) return ColorTranslator.FromHtml("#6B46C1"); // Purple text
            return ColorTranslator.FromHtml("#4A5568"); // Default Dark Gray text
        }

        private string GetInitials(string name)
        {
            string[] parts = name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                string lastWord = parts[parts.Length - 1];
                if (lastWord.Length > 0) return lastWord.Substring(0, 1).ToUpper();
            }
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
                    using (SolidBrush brush = new SolidBrush(this.BackColor))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
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

                    Color fillOuter = isHovered && this.BackColor != Color.Transparent ? ControlPaint.Light(this.BackColor, 0.2f) : this.BackColor;

                    using (SolidBrush brush = new SolidBrush(fillOuter))
                    {
                        e.Graphics.FillPath(brush, path);
                    }

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
