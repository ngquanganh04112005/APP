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
        private ComboBox cbClass;
        private ComboBox cbKhoa;
        private RoundedButton btnImport;
        private RoundedButton btnAdd;

        private RoundedPanel pnlGridContainer;
        private DataGridView dgvStudents;

        private Panel pnlPagination;
        private FlowLayoutPanel flpPagination;
        
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalRecords = 0;
        private RoundedSearchBox txtSearch;

        public DuyetDeTaiControl()
        {
            InitializeCustomComponent();
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
            lblTitle.Text = "Quản lý Sinh viên";
            lblTitle.ForeColor = ColorTranslator.FromHtml("#1A202C");
            lblTitle.Font = new Font("Segoe UI", 16f, FontStyle.Bold);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(30, 30);

            lblSubtitle = new Label();
            lblSubtitle.Text = "Xem, thêm, sửa, xóa thông tin sinh viên trong hệ thống.";
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
            
            // Biểu tượng Notification (Chuông với viền tròn)
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
            // 2. Filter & Action Toolbar
            // ==========================================
            pnlToolbar = new RoundedPanel();
            pnlToolbar.BackColor = Color.White;
            pnlToolbar.BorderRadius = 12;
            pnlToolbar.Location = new Point(30, 105);
            pnlToolbar.Width = this.Width - 60;
            pnlToolbar.Height = 52;
            pnlToolbar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            cbClass = CreateComboBox(new string[] { "Tất cả các lớp" });
            cbClass.Location = new Point(15, 11);
            cbClass.SelectedIndexChanged += (s, e) => LoadDataForPage(1);

            cbKhoa = CreateComboBox(new string[] { "Tất cả các khóa" });
            cbKhoa.Location = new Point(180, 11);
            cbKhoa.Visible = false;

            btnImport = new RoundedButton();
            btnImport.Text = "📥 Nhập dữ liệu";
            btnImport.BackColor = ColorTranslator.FromHtml("#3182CE"); 
            btnImport.ForeColor = Color.White;
            btnImport.FlatStyle = FlatStyle.Flat;
            btnImport.BorderRadius = 8;
            btnImport.FlatAppearance.BorderSize = 0;
            btnImport.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btnImport.Size = new Size(130, 35);
            btnImport.Location = new Point(pnlToolbar.Width - 305, 8);
            btnImport.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnImport.Cursor = Cursors.Hand;
            btnImport.Click += (s, e) => {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm|CSV Files|*.csv";
                    ofd.Title = "Chọn file dữ liệu để Import";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show($"Đang xử lý import từ file:\n{ofd.FileName}", "Import Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            };

            btnAdd = new RoundedButton();
            btnAdd.Text = "+ Thêm sinh viên";
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
                using (var form = new ThemSinhVienForm())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            string insertQuery = "INSERT INTO sinh_vien (ma_sinh_vien, ten_sinh_vien, ten_lop, ngay_sinh, gioi_tinh) VALUES (@ma, @ten, @lop, @ns, @gt)";
                            
                            DateTime? nsDate = null;
                            if (DateTime.TryParseExact(form.NgaySinh, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime tempDate))
                                nsDate = tempDate;
                            else if (DateTime.TryParse(form.NgaySinh, out DateTime tempDate2))
                                nsDate = tempDate2;

                            var parameters = new Microsoft.Data.SqlClient.SqlParameter[]
                            {
                                new Microsoft.Data.SqlClient.SqlParameter("@ma", form.MaSV),
                                new Microsoft.Data.SqlClient.SqlParameter("@ten", form.TenSV),
                                new Microsoft.Data.SqlClient.SqlParameter("@lop", string.IsNullOrEmpty(form.Lop) ? DBNull.Value : form.Lop),
                                new Microsoft.Data.SqlClient.SqlParameter("@ns", nsDate.HasValue ? (object)nsDate.Value : DBNull.Value),
                                new Microsoft.Data.SqlClient.SqlParameter("@gt", string.IsNullOrEmpty(form.GioiTinh) ? (object)form.GioiTinh : DBNull.Value)
                            };

                            int rows = DatabaseHelper.ExecuteNonQuery(insertQuery, parameters);
                            if (rows > 0)
                            {
                                MessageBox.Show("Đã lưu thông tin sinh viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadFilters();
                                LoadDataForPage(1);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi lưu sinh viên:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            };

            pnlToolbar.Controls.Add(cbClass);
            pnlToolbar.Controls.Add(cbKhoa);
            pnlToolbar.Controls.Add(btnImport);
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

            dgvStudents = new DataGridView();
            dgvStudents.Dock = DockStyle.Fill;
            dgvStudents.BorderStyle = BorderStyle.None;
            dgvStudents.BackgroundColor = Color.White;
            dgvStudents.RowHeadersVisible = false;
            dgvStudents.AllowUserToAddRows = false;
            dgvStudents.AllowUserToDeleteRows = false;
            dgvStudents.ReadOnly = true;
            dgvStudents.AllowUserToResizeRows = false;
            dgvStudents.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvStudents.GridColor = ColorTranslator.FromHtml("#EDF2F7"); // Light Gray Gridlines
            dgvStudents.RowTemplate.Height = 50; // Unified height
            dgvStudents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvStudents.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#F7FAFC");
            dgvStudents.DefaultCellStyle.SelectionForeColor = ColorTranslator.FromHtml("#1A202C");
            dgvStudents.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            
            // Header Styling
            dgvStudents.EnableHeadersVisualStyles = false;
            dgvStudents.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvStudents.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvStudents.ColumnHeadersHeight = 45;
            dgvStudents.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dgvStudents.ColumnHeadersDefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#A0AEC0");
            dgvStudents.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvStudents.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.White;
            dgvStudents.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;

            // Define Columns
            DataGridViewTextBoxColumn colSTT = new DataGridViewTextBoxColumn() { Name = "STT", HeaderText = "STT", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells };
            colSTT.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DataGridViewTextBoxColumn colMaSV = new DataGridViewTextBoxColumn() { Name = "MaSV", HeaderText = "MÃ SINH VIÊN", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, FillWeight = 15 };

            DataGridViewTextBoxColumn colTenSV = new DataGridViewTextBoxColumn() { Name = "TenSV", HeaderText = "TÊN SINH VIÊN", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, FillWeight = 35 };

            DataGridViewTextBoxColumn colLop = new DataGridViewTextBoxColumn() { Name = "Lop", HeaderText = "LỚP", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, FillWeight = 10 };
            DataGridViewTextBoxColumn colNgaySinh = new DataGridViewTextBoxColumn() { Name = "NgaySinh", HeaderText = "NGÀY SINH", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, FillWeight = 15 };
            DataGridViewTextBoxColumn colGioiTinh = new DataGridViewTextBoxColumn() { Name = "GioiTinh", HeaderText = "GIỚI TÍNH", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, FillWeight = 10 };
            DataGridViewTextBoxColumn colActions = new DataGridViewTextBoxColumn() { Name = "Actions", HeaderText = "THAO TÁC", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, FillWeight = 15 };
            colActions.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvStudents.Columns.Add(colSTT);
            dgvStudents.Columns.Add(colMaSV);
            dgvStudents.Columns.Add(colTenSV);
            dgvStudents.Columns.Add(colLop);
            dgvStudents.Columns.Add(colNgaySinh);
            dgvStudents.Columns.Add(colGioiTinh);
            dgvStudents.Columns.Add(colActions);
            
            // Disable sorting to keep data consistent with manual render
            foreach (DataGridViewColumn col in dgvStudents.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            // Custom Cell Painting
            dgvStudents.CellPainting += DgvStudents_CellPainting;
            
            // Handle Actions Click
            dgvStudents.CellClick += (s, e) => {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvStudents.Columns[e.ColumnIndex].Name == "Actions")
                {
                    Rectangle cellRect = dgvStudents.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                    Point clickPoint = dgvStudents.PointToClient(Cursor.Position);
                    
                    int centerX = cellRect.X + cellRect.Width / 2;
                    int centerY = cellRect.Y + cellRect.Height / 2;
                    
                    Rectangle editRect = new Rectangle(centerX - 35, centerY - 15, 30, 30);
                    Rectangle delRect = new Rectangle(centerX + 5, centerY - 15, 30, 30);

                    if (editRect.Contains(clickPoint))
                    {
                        var row = dgvStudents.Rows[e.RowIndex];
                        string currentMaSV = row.Cells["MaSV"].Value?.ToString() ?? "";
                        string currentTenSV = row.Cells["TenSV"].Value?.ToString() ?? "";
                        string currentLop = row.Cells["Lop"].Value?.ToString() ?? "";
                        string currentGioiTinh = row.Cells["GioiTinh"].Value?.ToString() ?? "";
                        string currentNgaySinh = row.Cells["NgaySinh"].Value?.ToString() ?? "";
                        
                        using (var form = new SuaSinhVienForm(currentMaSV, currentTenSV, currentLop, currentGioiTinh, currentNgaySinh))
                        {
                            if (form.ShowDialog() == DialogResult.OK)
                            {
                                try
                                {
                                    string updateQuery = "UPDATE sinh_vien SET ten_sinh_vien = @ten, ten_lop = @lop, ngay_sinh = @ns, gioi_tinh = @gt WHERE ma_sinh_vien = @ma";
                                    
                                    DateTime? nsDate = null;
                                    if (DateTime.TryParseExact(form.NgaySinh, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime tempDate))
                                        nsDate = tempDate;
                                    else if (DateTime.TryParse(form.NgaySinh, out DateTime tempDate2))
                                        nsDate = tempDate2;

                                    var parameters = new Microsoft.Data.SqlClient.SqlParameter[]
                                    {
                                        new Microsoft.Data.SqlClient.SqlParameter("@ma", form.MaSV),
                                        new Microsoft.Data.SqlClient.SqlParameter("@ten", form.TenSV),
                                        new Microsoft.Data.SqlClient.SqlParameter("@lop", string.IsNullOrEmpty(form.Lop) ? DBNull.Value : form.Lop),
                                        new Microsoft.Data.SqlClient.SqlParameter("@ns", nsDate.HasValue ? (object)nsDate.Value : DBNull.Value),
                                        new Microsoft.Data.SqlClient.SqlParameter("@gt", string.IsNullOrEmpty(form.GioiTinh) ? (object)form.GioiTinh : DBNull.Value)
                                    };

                                    int rows = DatabaseHelper.ExecuteNonQuery(updateQuery, parameters);
                                    if (rows > 0)
                                    {
                                        MessageBox.Show("Đã cập nhật sinh viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        LoadFilters();
                                        LoadDataForPage(currentPage);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Lỗi cập nhật sinh viên:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                    else if (delRect.Contains(clickPoint))
                    {
                        var result = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == DialogResult.Yes)
                        {
                            try
                            {
                                string currentMaSV = dgvStudents.Rows[e.RowIndex].Cells["MaSV"].Value?.ToString() ?? "";
                                string deleteQuery = "DELETE FROM sinh_vien WHERE ma_sinh_vien = @ma";
                                var parameters = new Microsoft.Data.SqlClient.SqlParameter[]
                                {
                                    new Microsoft.Data.SqlClient.SqlParameter("@ma", currentMaSV)
                                };
                                int rows = DatabaseHelper.ExecuteNonQuery(deleteQuery, parameters);
                                if (rows > 0)
                                {
                                    MessageBox.Show("Đã xóa sinh viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    LoadFilters();
                                    LoadDataForPage(currentPage);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Lỗi xóa sinh viên (Sinh viên này có thể đang có đồ án liên kết, cần xóa đồ án trước):\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            };

            pnlGridContainer.Controls.Add(dgvStudents);

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
            
            pnlPagination.Controls.Add(flpPagination);

            // ==========================================
            // Add all to UserControl
            // ==========================================
            this.Controls.Add(pnlPagination);
            
            // Khởi tạo bộ lọc và dữ liệu trang đầu tiên
            LoadFilters();
            LoadDataForPage(1);
            this.Controls.Add(pnlGridContainer);
            this.Controls.Add(pnlToolbar);
            this.Controls.Add(pnlBellContainer);
            this.Controls.Add(txtSearch);
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

        public void UpdatePagination(int totalPages, int current)
        {
            flpPagination.Controls.Clear();
            
            if (totalPages <= 0) totalPages = 1;

            // Nút "Trang trước" ( < )
            RoundedButton btnPrev = CreatePageButton("<", false);
            btnPrev.Enabled = current > 1;
            btnPrev.Click += (s, e) => { if (current > 1) LoadDataForPage(current - 1); };
            flpPagination.Controls.Add(btnPrev);

            // Các nút số trang
            for (int i = 1; i <= totalPages; i++)
            {
                int pageNum = i;
                RoundedButton btnPage = CreatePageButton(pageNum.ToString(), pageNum == current);
                btnPage.Click += (s, e) => { LoadDataForPage(pageNum); };
                flpPagination.Controls.Add(btnPage);
            }

            // Nút "Trang sau" ( > )
            RoundedButton btnNext = CreatePageButton(">", false);
            btnNext.Enabled = current < totalPages;
            btnNext.Click += (s, e) => { if (current < totalPages) LoadDataForPage(current + 1); };
            flpPagination.Controls.Add(btnNext);

            flpPagination.PerformLayout();
            flpPagination.Location = new Point(pnlPagination.Width - flpPagination.Width, 5); 
        }

        private void LoadFilters()
        {
            try
            {
                string currentClass = cbClass.SelectedItem?.ToString() ?? "Tất cả các lớp";
                cbClass.Items.Clear();
                cbClass.Items.Add("Tất cả các lớp");
                
                var dt = DatabaseHelper.ExecuteQuery("SELECT DISTINCT ten_lop FROM sinh_vien WHERE ten_lop IS NOT NULL");
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    cbClass.Items.Add(row["ten_lop"].ToString() ?? "");
                }
                
                if (cbClass.Items.Contains(currentClass))
                    cbClass.SelectedItem = currentClass;
                else
                    cbClass.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi load filters: " + ex.Message);
            }
        }

        private void LoadDataForPage(int page)
        {
            currentPage = page;
            string selectedClass = cbClass.SelectedItem?.ToString() ?? "Tất cả các lớp";
            string searchText = (txtSearch != null && txtSearch.Text != "Tìm kiếm...") ? txtSearch.Text.Trim() : "";

            try
            {
                string countQuery = "SELECT COUNT(*) FROM sinh_vien WHERE 1=1";
                string selectQuery = @"
                    SELECT ma_sinh_vien, ten_sinh_vien, ten_lop, ngay_sinh, gioi_tinh 
                    FROM (
                        SELECT ma_sinh_vien, ten_sinh_vien, ten_lop, ngay_sinh, gioi_tinh,
                               ROW_NUMBER() OVER (ORDER BY ma_sinh_vien) as RowNum
                        FROM sinh_vien
                        WHERE 1=1";
                
                var paramsList = new System.Collections.Generic.List<Microsoft.Data.SqlClient.SqlParameter>();

                if (selectedClass != "Tất cả các lớp")
                {
                    countQuery += " AND ten_lop = @tenLop";
                    selectQuery += " AND ten_lop = @tenLop";
                    paramsList.Add(new Microsoft.Data.SqlClient.SqlParameter("@tenLop", selectedClass));
                }

                if (!string.IsNullOrEmpty(searchText))
                {
                    countQuery += " AND (ma_sinh_vien LIKE @search OR ten_sinh_vien LIKE @search)";
                    selectQuery += " AND (ma_sinh_vien LIKE @search OR ten_sinh_vien LIKE @search)";
                    paramsList.Add(new Microsoft.Data.SqlClient.SqlParameter("@search", "%" + searchText + "%"));
                }

                selectQuery += ") AS RowResults WHERE RowNum BETWEEN @StartRow AND @EndRow";

                object totalObj = DatabaseHelper.ExecuteScalar(countQuery, paramsList.ToArray()) ?? 0;
                totalRecords = Convert.ToInt32(totalObj);

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

                dgvStudents.Rows.Clear();
                int seq = (currentPage - 1) * pageSize + 1;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    string ms = row["ma_sinh_vien"].ToString() ?? "";
                    string ten = row["ten_sinh_vien"].ToString() ?? "";
                    string lop = row["ten_lop"].ToString() ?? "";
                    string ns = row["ngay_sinh"] != DBNull.Value ? Convert.ToDateTime(row["ngay_sinh"]).ToString("dd/MM/yyyy") : "";
                    string gt = row["gioi_tinh"].ToString() ?? "";
                    
                    dgvStudents.Rows.Add(seq.ToString(), ms, ten, lop, ns, gt, "");
                    seq++;
                }

                UpdatePagination(totalPages, currentPage);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy dữ liệu sinh viên:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvStudents_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return; // Ignore headers

            // Paint default background and selection
            e.PaintBackground(e.CellBounds, (e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected);
            
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            string colName = dgvStudents.Columns[e.ColumnIndex].Name;

            if (colName == "STT")
            {
                using (SolidBrush textBrush = new SolidBrush(ColorTranslator.FromHtml("#4A5568")))
                {
                    StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString((e.RowIndex + 1).ToString(), new Font("Segoe UI", 9f), textBrush, e.CellBounds, sf);
                }
                e.Handled = true;
            }
            else if (colName == "TenSV" && e.Value != null) // TÊN SINH VIÊN
            {
                string name = e.Value.ToString();
                int padLeft = 5;

                using (SolidBrush nameBrush = new SolidBrush(ColorTranslator.FromHtml("#1A202C")))
                {
                    StringFormat sf = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                    RectangleF textRect = new RectangleF(e.CellBounds.X + padLeft, e.CellBounds.Y, e.CellBounds.Width - padLeft - 5, e.CellBounds.Height);
                    g.DrawString(name, new Font("Segoe UI", 9.5f, FontStyle.Bold), nameBrush, textRect, sf);
                }
                e.Handled = true;
            }
            else if (colName == "Actions") // ACTIONS
            {
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
            else if (e.Value != null)
            {
                // Draw normal text for other columns (MaSV, Lop, NgaySinh, GioiTinh)
                using (SolidBrush textBrush = new SolidBrush(ColorTranslator.FromHtml("#4A5568")))
                {
                    g.DrawString(e.Value.ToString(), e.CellStyle.Font ?? new Font("Segoe UI", 9.5f), textBrush, e.CellBounds.X + 5, e.CellBounds.Y + (e.CellBounds.Height - 15) / 2);
                }
                e.Handled = true;
            }

            // Draw grid line for all cells
            using (Pen gridPen = new Pen(dgvStudents.GridColor, 1))
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

        // ==========================================
        // Inner Class: Rounded Search Box (giống Dashboard)
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
                        textBox.Text = "Tìm kiếm...";
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
