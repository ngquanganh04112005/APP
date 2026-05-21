using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;

#nullable disable

namespace TBUProject
{
    public class QuanLyDoAnControl : UserControl
    {
        // Controls
        private Label lblTitle;
        private Label lblSubtitle;
        
        private RoundedPanel pnlToolbar;
        private RoundedButton btnAdd;
        private RoundedButton btnSelectFile;

        private RoundedPanel pnlGridContainer;
        private DataGridView dgvProjectList;

        private Panel pnlPagination;
        private FlowLayoutPanel flpPagination;
        private Label lblTotalRecords;
        private ComboBox cmbTypeFilter;

        private class ProjectMock
        {
            public string MaDeTai { get; set; }
            public string TenDeTai { get; set; }
            public string TenSV { get; set; }
            public string Loai { get; set; }
            public string NgayNop { get; set; }
        }

        private System.Collections.Generic.List<ProjectMock> mockProjectsList = new System.Collections.Generic.List<ProjectMock>()
        {
            new ProjectMock { MaDeTai = "DT001", TenDeTai = "Hệ thống quản lý điểm sinh viên bằng khuôn mặt", TenSV = "Nguyễn Quang Anh", Loai = "Đồ án", NgayNop = "19/05/2026" },
            new ProjectMock { MaDeTai = "DT002", TenDeTai = "Xây dựng ứng dụng di động Flutter quản lý dự án", TenSV = "Trần Văn Bình", Loai = "Khóa luận", NgayNop = "20/05/2026" },
            new ProjectMock { MaDeTai = "DT003", TenDeTai = "Phân tích bảo mật mạng không dây wifi", TenSV = "Lê Thị Mai", Loai = "Tiểu luận", NgayNop = "21/05/2026" }
        };

        // Data Management
        private int currentPage = 1;
        private int totalRecords = 0;
        private int pageSize = 10;
        private RoundedSearchBox txtSearch;

        public QuanLyDoAnControl()
        {
            InitializeCustomComponent();
            LoadDataForPage(1); // Load empty or DB data
        }

        private void InitializeCustomComponent()
        {
            // Main Settings
            this.BackColor = ColorTranslator.FromHtml("#F7FAFC"); // Light Gray Background
            this.Dock = DockStyle.Fill;
            this.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
            this.Size = new Size(1200, 700);
            this.Padding = new Padding(30);

            lblTitle = new Label();
            lblTitle.Text = "Quản lý Đồ án";
            lblTitle.ForeColor = ColorTranslator.FromHtml("#1A202C");
            lblTitle.Font = new Font("Segoe UI", 16f, FontStyle.Bold);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(30, 30);

            lblSubtitle = new Label();
            lblSubtitle.Text = "Theo dõi và quản lý thông tin chi tiết các đồ án, khóa luận, tiểu luận của sinh viên.";
            lblSubtitle.ForeColor = ColorTranslator.FromHtml("#718096");
            lblSubtitle.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            lblSubtitle.AutoSize = true;
            lblSubtitle.Location = new Point(30, 60);

            // ==========================================
            // Thanh Search bo góc với Placeholder
            // ==========================================
            txtSearch = new RoundedSearchBox();
            txtSearch.Size = new Size(350, 38);
            txtSearch.Location = new Point(this.Width - 440, 30);
            txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtSearch.TextChanged += (s, e) => {
                LoadDataForPage(1);
            };
            
            // Biểu tượng Notification
            Panel pnlBellContainer = new Panel();
            pnlBellContainer.Size = new Size(40, 40);
            pnlBellContainer.Location = new Point(this.Width - 75, 29);
            pnlBellContainer.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pnlBellContainer.BackColor = Color.Transparent;
            pnlBellContainer.Cursor = Cursors.Hand;
            
            pnlBellContainer.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, pnlBellContainer.Width - 1, pnlBellContainer.Height - 1);
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

            pnlBellContainer.MouseEnter += (s, e) => { pnlBellContainer.BackColor = ColorTranslator.FromHtml("#EDF2F7"); };
            pnlBellContainer.MouseLeave += (s, e) => { pnlBellContainer.BackColor = Color.White; };
            lblBell.MouseEnter += (s, e) => { pnlBellContainer.BackColor = ColorTranslator.FromHtml("#EDF2F7"); };
            lblBell.MouseLeave += (s, e) => { pnlBellContainer.BackColor = Color.White; };

            pnlBellContainer.Controls.Add(lblBell);

            // ==========================================
            // 2. Toolbar
            // ==========================================
            pnlToolbar = new RoundedPanel();
            pnlToolbar.BackColor = Color.White;
            pnlToolbar.BorderRadius = 12;
            pnlToolbar.Location = new Point(30, 105);
            pnlToolbar.Width = this.Width - 60;
            pnlToolbar.Height = 52;
            pnlToolbar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            cmbTypeFilter = CreateComboBox(new string[] { "Tất cả đề tài", "Đồ án", "Khóa luận", "Tiểu luận" });
            cmbTypeFilter.Location = new Point(15, 8);
            cmbTypeFilter.Width = 180;
            cmbTypeFilter.SelectedIndexChanged += (s, e) => {
                LoadDataForPage(1);
            };

            btnSelectFile = new RoundedButton();
            btnSelectFile.Text = "📥 Nhập dữ liệu";
            btnSelectFile.BackColor = ColorTranslator.FromHtml("#3182CE"); 
            btnSelectFile.ForeColor = Color.White;
            btnSelectFile.FlatStyle = FlatStyle.Flat;
            btnSelectFile.BorderRadius = 8;
            btnSelectFile.FlatAppearance.BorderSize = 0;
            btnSelectFile.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btnSelectFile.Size = new Size(130, 35);
            btnSelectFile.Location = new Point(pnlToolbar.Width - 305, 8);
            btnSelectFile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSelectFile.Cursor = Cursors.Hand;
            btnSelectFile.Click += (s, e) => {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "All Files|*.*";
                    ofd.Title = "Chọn file đồ án";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show($"Đã chọn file:\n{ofd.FileName}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            };

            btnAdd = new RoundedButton();
            btnAdd.Text = "+ Thêm đồ án";
            btnAdd.BackColor = ColorTranslator.FromHtml("#38A169"); // Green primary action
            btnAdd.ForeColor = Color.White;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.BorderRadius = 8;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btnAdd.Size = new Size(150, 35);
            btnAdd.Location = new Point(pnlToolbar.Width - 165, 8);
            btnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAdd.Cursor = Cursors.Hand;
            btnAdd.Click += (s, e) => {
                using (var form = new AddEditProjectForm(isEdit: false))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            EnsureStudentExists(form.MaSV, form.TenSV);
                            string gvId = ResolveGiangVienId(form.GiangVienHD);
                            string loaiId = ResolveLoaiDoAnId(form.LoaiDeTai);
                            
                            DateTime? nopDate = null;
                            if (DateTime.TryParseExact(form.NgayNop, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime tempDate))
                                nopDate = tempDate;
                            else if (DateTime.TryParse(form.NgayNop, out DateTime tempDate2))
                                nopDate = tempDate2;

                            string insertQuery = "INSERT INTO do_an (ma_do_an, ten_do_an, ma_sinh_vien, ma_giang_vien_huong_dan, ma_loai, ngay_nop, file_dinh_kem) VALUES (@ma, @ten, @maSV, @gvId, @loaiId, @ngay, @file)";
                            var parameters = new Microsoft.Data.SqlClient.SqlParameter[]
                            {
                                new Microsoft.Data.SqlClient.SqlParameter("@ma", form.MaDeTai),
                                new Microsoft.Data.SqlClient.SqlParameter("@ten", form.TenDeTai),
                                new Microsoft.Data.SqlClient.SqlParameter("@maSV", form.MaSV),
                                new Microsoft.Data.SqlClient.SqlParameter("@gvId", string.IsNullOrEmpty(gvId) ? DBNull.Value : (object)gvId),
                                new Microsoft.Data.SqlClient.SqlParameter("@loaiId", string.IsNullOrEmpty(loaiId) ? DBNull.Value : (object)loaiId),
                                new Microsoft.Data.SqlClient.SqlParameter("@ngay", nopDate.HasValue ? (object)nopDate.Value : DBNull.Value),
                                new Microsoft.Data.SqlClient.SqlParameter("@file", string.IsNullOrEmpty(form.FileDinhKem) ? DBNull.Value : (object)form.FileDinhKem)
                            };

                            int rows = DatabaseHelper.ExecuteNonQuery(insertQuery, parameters);
                            if (rows > 0)
                            {
                                MessageBox.Show("Đã thêm đồ án thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadDataForPage(1);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi thêm đồ án:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            };

            pnlToolbar.Controls.Add(cmbTypeFilter);
            pnlToolbar.Controls.Add(btnSelectFile);
            pnlToolbar.Controls.Add(btnAdd);

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

            dgvProjectList = new DataGridView();
            dgvProjectList.Dock = DockStyle.Fill;
            dgvProjectList.BorderStyle = BorderStyle.None;
            dgvProjectList.BackgroundColor = Color.White;
            dgvProjectList.RowHeadersVisible = false;
            dgvProjectList.AllowUserToAddRows = false;
            dgvProjectList.AllowUserToDeleteRows = false;
            dgvProjectList.ReadOnly = true;
            dgvProjectList.AllowUserToResizeRows = false;
            dgvProjectList.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvProjectList.GridColor = ColorTranslator.FromHtml("#EDF2F7"); // Light Gray Gridlines
            dgvProjectList.RowTemplate.Height = 50;
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
            dgvProjectList.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvProjectList.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.White;

            // Define Columns (STT, Mã đồ án, Tên đồ án, Tên sinh viên, Loại, Ngày nộp)
            var colSTT = new DataGridViewTextBoxColumn() { Name = "STT", HeaderText = "STT", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells };
            colSTT.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvProjectList.Columns.Add(colSTT);
            dgvProjectList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "MaDeTai", HeaderText = "MÃ ĐỒ ÁN", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, FillWeight = 15 });
            dgvProjectList.Columns.Add(new DataGridViewLinkColumn() { Name = "TenDeTai", HeaderText = "TÊN ĐỒ ÁN", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, FillWeight = 28, LinkBehavior = LinkBehavior.HoverUnderline, LinkColor = ColorTranslator.FromHtml("#3182CE"), ActiveLinkColor = ColorTranslator.FromHtml("#2B6CB0") });
            dgvProjectList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "TenSV", HeaderText = "TÊN SINH VIÊN", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, FillWeight = 25 });
            dgvProjectList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Loai", HeaderText = "LOẠI", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, FillWeight = 17 });
            dgvProjectList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "NgayNop", HeaderText = "NGÀY NỘP", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, FillWeight = 15 });
            var colActions = new DataGridViewTextBoxColumn() { Name = "Actions", HeaderText = "THAO TÁC", AutoSizeMode = DataGridViewAutoSizeColumnMode.None, Width = 90 };
            colActions.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvProjectList.Columns.Add(colActions);
            
            // Disable sorting to keep data consistent with manual render
            foreach (DataGridViewColumn col in dgvProjectList.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            // Custom Cell Painting
            dgvProjectList.CellPainting += DgvProjectList_CellPainting;

            // Handle Actions Click
            dgvProjectList.CellClick += (s, e) => {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    string colName = dgvProjectList.Columns[e.ColumnIndex].Name;

                    if (colName == "TenDeTai")
                    {
                        var row = dgvProjectList.Rows[e.RowIndex];
                        string maDeTai = row.Cells["MaDeTai"].Value?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(maDeTai))
                        {
                            using (var detailForm = new ChiTietDoAnForm(maDeTai))
                            {
                                detailForm.ShowDialog();
                            }
                        }
                    }
                    else if (colName == "Actions")
                    {
                        Rectangle cellRect = dgvProjectList.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                        Point clickPoint = dgvProjectList.PointToClient(Cursor.Position);
                        
                        int centerX = cellRect.X + cellRect.Width / 2;
                        int centerY = cellRect.Y + cellRect.Height / 2;
                        
                        Rectangle editRect = new Rectangle(centerX - 35, centerY - 15, 30, 30);
                        Rectangle delRect = new Rectangle(centerX + 5, centerY - 15, 30, 30);

                        if (editRect.Contains(clickPoint))
                        {
                            var row = dgvProjectList.Rows[e.RowIndex];
                            string currentMaDeTai = row.Cells["MaDeTai"].Value?.ToString() ?? "";

                            // Mở ChiTietDoAnForm với nút "Chỉnh sửa" tích hợp bên trong
                            if (!string.IsNullOrEmpty(currentMaDeTai))
                            {
                                using (var detailForm = new ChiTietDoAnForm(currentMaDeTai))
                                {
                                    detailForm.ShowDialog();
                                    // Nếu có thay đổi (DialogResult.OK), reload lại danh sách
                                    if (detailForm.DialogResult == DialogResult.OK)
                                    {
                                        LoadDataForPage(currentPage);
                                    }
                                }
                            }
                        }
                        else if (delRect.Contains(clickPoint))
                        {
                            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa đồ án này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.Yes)
                            {
                                try
                                {
                                    string currentMaDeTai = dgvProjectList.Rows[e.RowIndex].Cells["MaDeTai"].Value?.ToString() ?? "";
                                    string deleteQuery = "DELETE FROM do_an WHERE ma_do_an = @ma";
                                    var parameters = new Microsoft.Data.SqlClient.SqlParameter[]
                                    {
                                        new Microsoft.Data.SqlClient.SqlParameter("@ma", currentMaDeTai)
                                    };
                                    int rows = DatabaseHelper.ExecuteNonQuery(deleteQuery, parameters);
                                    if (rows > 0)
                                    {
                                        MessageBox.Show("Đã xóa đồ án thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        LoadDataForPage(currentPage);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Lỗi xóa đồ án:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                }
            };

            pnlGridContainer.Controls.Add(dgvProjectList);

            // ==========================================
            // 4. Bottom Pagination & Status Bar
            // ==========================================
            pnlPagination = new Panel();
            pnlPagination.Height = 44;
            pnlPagination.Width = this.Width - 60;
            pnlPagination.Location = new Point(30, this.Height - 70);
            pnlPagination.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlPagination.Padding = new Padding(0, 4, 10, 4);

            lblTotalRecords = new Label();
            lblTotalRecords.Text = "Tổng số bản ghi: 0";
            lblTotalRecords.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            lblTotalRecords.ForeColor = ColorTranslator.FromHtml("#4A5568");
            lblTotalRecords.AutoSize = true;
            lblTotalRecords.Location = new Point(0, 12);
            pnlPagination.Controls.Add(lblTotalRecords);

            flpPagination = new FlowLayoutPanel();
            flpPagination.FlowDirection = FlowDirection.LeftToRight;
            flpPagination.WrapContents = false;
            flpPagination.AutoSize = true;
            flpPagination.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flpPagination.Dock = DockStyle.Right;
            
            pnlPagination.Controls.Add(flpPagination);

            // ==========================================
            // Add all to UserControl
            // ==========================================
            this.Controls.Add(pnlPagination);
            this.Controls.Add(pnlGridContainer);
            this.Controls.Add(pnlToolbar);
            this.Controls.Add(pnlBellContainer);
            this.Controls.Add(txtSearch);
            this.Controls.Add(lblSubtitle);
            this.Controls.Add(lblTitle);
        }

        private static void EnsureStudentExists(string maSV, string tenSV)
        {
            try
            {
                object result = DatabaseHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM sinh_vien WHERE ma_sinh_vien = @ma",
                    new Microsoft.Data.SqlClient.SqlParameter[] { new Microsoft.Data.SqlClient.SqlParameter("@ma", maSV) });
                if (Convert.ToInt32(result) == 0)
                {
                    DatabaseHelper.ExecuteNonQuery(
                        "INSERT INTO sinh_vien (ma_sinh_vien, ten_sinh_vien, ten_lop, ngay_sinh, gioi_tinh) VALUES (@ma, @ten, @lop, @ns, @gt)",
                        new Microsoft.Data.SqlClient.SqlParameter[] {
                            new Microsoft.Data.SqlClient.SqlParameter("@ma", maSV),
                            new Microsoft.Data.SqlClient.SqlParameter("@ten", string.IsNullOrEmpty(tenSV) ? "Sinh viên chưa cập nhật" : tenSV),
                            new Microsoft.Data.SqlClient.SqlParameter("@lop", "CNTT-K15"),
                            new Microsoft.Data.SqlClient.SqlParameter("@ns", DateTime.Now),
                            new Microsoft.Data.SqlClient.SqlParameter("@gt", "Nam")
                        });
                }
            }
            catch {}
        }

        private static string ResolveGiangVienId(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            try
            {
                object idObj = DatabaseHelper.ExecuteScalar(
                    "SELECT ma_giang_vien FROM giang_vien WHERE ma_giang_vien = @input",
                    new Microsoft.Data.SqlClient.SqlParameter[] { new Microsoft.Data.SqlClient.SqlParameter("@input", input) });
                if (idObj != null) return idObj.ToString() ?? "";

                idObj = DatabaseHelper.ExecuteScalar(
                    "SELECT ma_giang_vien FROM giang_vien WHERE ten_giang_vien = @input",
                    new Microsoft.Data.SqlClient.SqlParameter[] { new Microsoft.Data.SqlClient.SqlParameter("@input", input) });
                if (idObj != null) return idObj.ToString() ?? "";

                string newId = "GV" + Guid.NewGuid().ToString("N").Substring(0, 5).ToUpper();
                DatabaseHelper.ExecuteNonQuery(
                    "INSERT INTO giang_vien (ma_giang_vien, ten_giang_vien, ma_chuc_vu, mat_khau) VALUES (@id, @ten, @chucVu, '123456')",
                    new Microsoft.Data.SqlClient.SqlParameter[] {
                        new Microsoft.Data.SqlClient.SqlParameter("@id", newId),
                        new Microsoft.Data.SqlClient.SqlParameter("@ten", input),
                        new Microsoft.Data.SqlClient.SqlParameter("@chucVu", "CV03")
                    });
                return newId;
            }
            catch
            {
                return "";
            }
        }

        private static string ResolveLoaiDoAnId(string loai)
        {
            if (string.IsNullOrEmpty(loai)) return "";
            string name = loai.Trim().ToLower();
            if (name.Contains("khóa luận") || name.Contains("khoá luận")) return "LDA02";
            if (name.Contains("tiểu luận") || name.Contains("tiêu luận")) return "LDA03";
            return "LDA01";
        }

        private void LoadDataForPage(int page)
        {
            currentPage = page;
            dgvProjectList.Rows.Clear();
            flpPagination.Controls.Clear();

            string selectedType = cmbTypeFilter?.SelectedItem?.ToString() ?? "Tất cả đề tài";
            string searchText = (txtSearch != null && txtSearch.Text != "Tìm kiếm...") ? txtSearch.Text.Trim() : "";

            try
            {
                string countQuery = @"
                    SELECT COUNT(*) 
                    FROM do_an d
                    INNER JOIN sinh_vien s ON d.ma_sinh_vien = s.ma_sinh_vien
                    LEFT JOIN giang_vien g ON d.ma_giang_vien_huong_dan = g.ma_giang_vien
                    LEFT JOIN loai_do_an l ON d.ma_loai = l.ma_loai
                    WHERE 1=1";
                
                string selectQuery = @"
                    SELECT ma_do_an, ten_do_an, ten_sinh_vien, ten_loai, ngay_nop
                    FROM (
                        SELECT d.ma_do_an, d.ten_do_an, s.ten_sinh_vien, l.ten_loai, d.ngay_nop,
                               ROW_NUMBER() OVER (ORDER BY d.ma_do_an) as RowNum
                        FROM do_an d
                        INNER JOIN sinh_vien s ON d.ma_sinh_vien = s.ma_sinh_vien
                        LEFT JOIN giang_vien g ON d.ma_giang_vien_huong_dan = g.ma_giang_vien
                        LEFT JOIN loai_do_an l ON d.ma_loai = l.ma_loai
                        WHERE 1=1";

                var paramsList = new System.Collections.Generic.List<Microsoft.Data.SqlClient.SqlParameter>();

                if (selectedType != "Tất cả đề tài")
                {
                    countQuery += " AND l.ten_loai = @loai";
                    selectQuery += " AND l.ten_loai = @loai";
                    paramsList.Add(new Microsoft.Data.SqlClient.SqlParameter("@loai", selectedType));
                }

                if (!string.IsNullOrEmpty(searchText))
                {
                    countQuery += " AND (d.ma_do_an LIKE @search OR d.ten_do_an LIKE @search OR s.ten_sinh_vien LIKE @search OR g.ten_giang_vien LIKE @search)";
                    selectQuery += " AND (d.ma_do_an LIKE @search OR d.ten_do_an LIKE @search OR s.ten_sinh_vien LIKE @search OR g.ten_giang_vien LIKE @search)";
                    paramsList.Add(new Microsoft.Data.SqlClient.SqlParameter("@search", "%" + searchText + "%"));
                }

                selectQuery += ") AS RowResults WHERE RowNum BETWEEN @StartRow AND @EndRow";

                object totalObj = DatabaseHelper.ExecuteScalar(countQuery, paramsList.ToArray()) ?? 0;
                totalRecords = Convert.ToInt32(totalObj);
                lblTotalRecords.Text = $"Tổng số bản ghi: {totalRecords}";

                if (totalRecords == 0) return;

                int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                if (currentPage > totalPages && totalPages > 0) currentPage = totalPages;

                int startRow = (currentPage - 1) * pageSize + 1;
                int endRow = currentPage * pageSize;

                var pageParams = new System.Collections.Generic.List<Microsoft.Data.SqlClient.SqlParameter>();
                foreach (var p in paramsList)
                {
                    pageParams.Add(new Microsoft.Data.SqlClient.SqlParameter(p.ParameterName, p.Value));
                }
                pageParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@StartRow", startRow));
                pageParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@EndRow", endRow));

                var dt = DatabaseHelper.ExecuteQuery(selectQuery, pageParams.ToArray());

                int seq = startRow;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    string ma = row["ma_do_an"].ToString() ?? "";
                    string ten = row["ten_do_an"].ToString() ?? "";
                    string sv = row["ten_sinh_vien"].ToString() ?? "";
                    string loai = row["ten_loai"].ToString() ?? "";
                    string ngay = row["ngay_nop"] != DBNull.Value ? Convert.ToDateTime(row["ngay_nop"]).ToString("dd/MM/yyyy") : "";
                    dgvProjectList.Rows.Add(seq.ToString(), ma, ten, sv, loai, ngay, "");
                    seq++;
                }

                RoundedButton btnPrev = CreatePageButton("<", false);
                btnPrev.Enabled = currentPage > 1;
                btnPrev.Click += (s, e) => { if (currentPage > 1) LoadDataForPage(currentPage - 1); };
                flpPagination.Controls.Add(btnPrev);

                for (int i = 1; i <= totalPages; i++)
                {
                    int pageNum = i;
                    if (i == 1 || i == totalPages || (i >= currentPage - 1 && i <= currentPage + 1))
                    {
                        RoundedButton btnPage = CreatePageButton(pageNum.ToString(), pageNum == currentPage);
                        btnPage.Click += (s, e) => { LoadDataForPage(pageNum); };
                        flpPagination.Controls.Add(btnPage);
                    }
                    else if (i == currentPage - 2 || i == currentPage + 2)
                    {
                        Label lblEllipsis = new Label() { 
                            Text = "...", 
                            AutoSize = true, 
                            ForeColor = ColorTranslator.FromHtml("#3182CE"),
                            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                            Margin = new Padding(4, 10, 4, 2) 
                        };
                        flpPagination.Controls.Add(lblEllipsis);
                    }
                }

                RoundedButton btnNext = CreatePageButton(">", false);
                btnNext.Enabled = currentPage < totalPages;
                btnNext.Click += (s, e) => { if (currentPage < totalPages) LoadDataForPage(currentPage + 1); };
                flpPagination.Controls.Add(btnNext);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy dữ liệu đồ án:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                btn.BackColor = ColorTranslator.FromHtml("#3182CE");
                btn.ForeColor = Color.White;
            }
            else
            {
                btn.BackColor = Color.White;
                btn.ForeColor = ColorTranslator.FromHtml("#3182CE");
            }
            
            if (text == "...")
            {
                btn.FlatAppearance.BorderSize = 0;
                btn.BackColor = Color.Transparent;
            }

            return btn;
        }

        private ComboBox CreateComboBox(string[] items)
        {
            ComboBox cb = new ComboBox();
            cb.DropDownStyle = ComboBoxStyle.DropDownList;
            cb.Items.AddRange(items);
            if(cb.Items.Count > 0) cb.SelectedIndex = 0;
            cb.Width = 150;
            cb.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            cb.ForeColor = ColorTranslator.FromHtml("#4A5568");
            cb.BackColor = Color.White;
            cb.FlatStyle = FlatStyle.Flat; 
            return cb;
        }

        private void DgvProjectList_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return; // Ignore headers

            // Paint default background and selection
            e.PaintBackground(e.CellBounds, (e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            string colName = dgvProjectList.Columns[e.ColumnIndex].Name;

            if (colName == "Actions")
            {
                // ACTIONS (Edit & Delete Buttons)
                int centerX = e.CellBounds.X + e.CellBounds.Width / 2;
                int centerY = e.CellBounds.Y + e.CellBounds.Height / 2;

                // Edit Button (Blue)
                Rectangle editRect = new Rectangle(centerX - 35, centerY - 15, 30, 30);
                using (GraphicsPath path = GetRoundedPath(editRect, 6))
                using (SolidBrush editBg = new SolidBrush(ColorTranslator.FromHtml("#EBF8FF")))
                using (SolidBrush editFg = new SolidBrush(ColorTranslator.FromHtml("#3182CE")))
                {
                    g.FillPath(editBg, path);
                    StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString("✏", new Font("Segoe UI Emoji", 10f), editFg, editRect, sf);
                }

                // Delete Button (Red)
                Rectangle delRect = new Rectangle(centerX + 5, centerY - 15, 30, 30);
                using (GraphicsPath path = GetRoundedPath(delRect, 6))
                using (SolidBrush delBg = new SolidBrush(ColorTranslator.FromHtml("#FFF5F5")))
                using (SolidBrush delFg = new SolidBrush(ColorTranslator.FromHtml("#E53E3E")))
                {
                    g.FillPath(delBg, path);
                    StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString("🗑", new Font("Segoe UI Emoji", 10f), delFg, delRect, sf);
                }

                e.Handled = true;
            }
            else if (colName == "STT")
            {
                using (SolidBrush textBrush = new SolidBrush(ColorTranslator.FromHtml("#4A5568")))
                {
                    StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString((e.RowIndex + 1).ToString(), new Font("Segoe UI", 9f), textBrush, e.CellBounds, sf);
                }
                e.Handled = true;
            }
            else
            {
                // Normal Text Cells
                if (e.Value != null)
                {
                    using (SolidBrush textBrush = new SolidBrush(ColorTranslator.FromHtml("#4A5568")))
                    {
                        StringFormat sf = new StringFormat() { 
                            Alignment = StringAlignment.Near, 
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.EllipsisCharacter,
                            FormatFlags = StringFormatFlags.NoWrap
                        };
                        RectangleF textRect = new RectangleF(e.CellBounds.X + 5, e.CellBounds.Y, e.CellBounds.Width - 10, e.CellBounds.Height);
                        Font f = new Font("Segoe UI", 9.5f, colName == "TenDeTai" ? FontStyle.Bold : FontStyle.Regular);
                        if(colName == "TenDeTai") textBrush.Color = ColorTranslator.FromHtml("#1A202C");

                        g.DrawString(e.Value.ToString(), f, textBrush, textRect, sf);
                    }
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
        
        // ==========================================
        // Inner Class: Rounded Search Box
        // ==========================================
        private class RoundedSearchBox : UserControl
        {
            private TextBox textBox;

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public int BorderRadius { get; set; } = 8;
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public Color BorderColor { get; set; } = ColorTranslator.FromHtml("#E2E8F0");

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public override string Text { get => textBox.Text; set => textBox.Text = value; }
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public override Color ForeColor { get => base.ForeColor; set { base.ForeColor = value; textBox.ForeColor = value; } }

            public RoundedSearchBox()
            {
                textBox = new TextBox();
                textBox.BorderStyle = BorderStyle.None;
                textBox.Location = new Point(42, 11);
                textBox.Width = this.Width - 55;
                textBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                this.BackColor = Color.White;
                this.Padding = new Padding(12, 10, 12, 10);
                this.Size = new Size(350, 38);
                this.Controls.Add(textBox);

                textBox.Text = "Tìm kiếm...";
                textBox.ForeColor = Color.Gray;

                textBox.GotFocus += (s, e) => {
                    if (textBox.Text == "Tìm kiếm...") {
                        textBox.Text = "";
                        textBox.ForeColor = Color.Black;
                    }
                    BorderColor = ColorTranslator.FromHtml("#3182CE");
                    this.Invalidate();
                };
                textBox.LostFocus += (s, e) => {
                    if (string.IsNullOrWhiteSpace(textBox.Text)) {
                        textBox.Text = "Tìm theo tên đề tài, SV, GV...";
                        textBox.ForeColor = Color.Gray;
                    }
                    BorderColor = ColorTranslator.FromHtml("#E2E8F0");
                    this.Invalidate();
                };
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

                    using (Pen iconPen = new Pen(ColorTranslator.FromHtml("#A0AEC0"), 2f))
                    {
                        e.Graphics.DrawEllipse(iconPen, 15, 12, 11, 11);
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
            public event EventHandler TextChanged { add => textBox.TextChanged += value; remove => textBox.TextChanged -= value; }
        }
    }
}
