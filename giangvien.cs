using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

#nullable disable

namespace TBUProject
{
    public class GiangVienControl : UserControl
    {
        // Controls
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblTotalRecords;
        private ComboBox cmbRoleFilter;
        
        private RoundedPanel pnlToolbar;
        private RoundedButton btnImport;
        private RoundedButton btnAdd;

        private RoundedPanel pnlGridContainer;
        private DataGridView dgvAdvisors;

        private Panel pnlPagination;
        private FlowLayoutPanel flpPagination;

        // Data Management
        private int currentPage = 1;
        private int totalRecords = 0;
        private int pageSize = 10;
        private RoundedSearchBox txtSearch;

        private class AdvisorMock {
            public string MaGV { get; set; }
            public string TenGV { get; set; }
            public string ChucVu { get; set; }
            public AdvisorMock(string ma, string ten, string chucvu) {
                MaGV = ma; TenGV = ten; ChucVu = chucvu;
            }
        }

        private List<AdvisorMock> mockAdvisorsList = new List<AdvisorMock>() {
            new AdvisorMock("GV001", "Nguyễn Văn A", "Trưởng bộ môn"),
            new AdvisorMock("GV002", "Trần Thị B", "Phó bộ môn"),
            new AdvisorMock("GV003", "Lê Hoàng C", "Giảng viên"),
        };

        public GiangVienControl()
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
            this.Size = new Size(1000, 700);
            this.Padding = new Padding(30);

            lblTitle = new Label();
            lblTitle.Text = "Quản lý Giảng viên";
            lblTitle.ForeColor = ColorTranslator.FromHtml("#1A202C");
            lblTitle.Font = new Font("Segoe UI", 16f, FontStyle.Bold);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(30, 30);

            lblSubtitle = new Label();
            lblSubtitle.Text = "Quản lý danh sách giảng viên, cập nhật thông tin và theo dõi tiến độ đồ án.";
            lblSubtitle.ForeColor = ColorTranslator.FromHtml("#718096");
            lblSubtitle.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            lblSubtitle.AutoSize = true;
            lblSubtitle.Location = new Point(30, 60);

            // ==========================================
            // Thanh Search bo góc với Placeholder (giống Dashboard)
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

            cmbRoleFilter = CreateComboBox(new string[] { "Tất cả chức vụ", "Trưởng bộ môn", "Phó bộ môn", "Giảng viên" });
            cmbRoleFilter.Location = new Point(15, 10);
            cmbRoleFilter.Width = 180;
            cmbRoleFilter.SelectedIndexChanged += (s, e) => {
                LoadDataForPage(1);
            };

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
            btnAdd.Text = "+ Thêm giảng viên";
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
                using (var form = new ThemGiangVienForm())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            string chucVuId = GetOrCreateChucVuId(form.ChucVu);
                            string insertQuery = "INSERT INTO giang_vien (ma_giang_vien, ten_giang_vien, ma_chuc_vu, email, mat_khau) VALUES (@ma, @ten, @chucVuId, @email, '123456')";
                            var parameters = new Microsoft.Data.SqlClient.SqlParameter[]
                            {
                                new Microsoft.Data.SqlClient.SqlParameter("@ma", form.MaGV),
                                new Microsoft.Data.SqlClient.SqlParameter("@ten", form.TenGV),
                                new Microsoft.Data.SqlClient.SqlParameter("@chucVuId", string.IsNullOrEmpty(chucVuId) ? DBNull.Value : (object)chucVuId),
                                new Microsoft.Data.SqlClient.SqlParameter("@email", string.IsNullOrEmpty(form.Email) ? DBNull.Value : (object)form.Email)
                            };

                            int rows = DatabaseHelper.ExecuteNonQuery(insertQuery, parameters);
                            if (rows > 0)
                            {
                                MessageBox.Show("Đã lưu thông tin giảng viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadDataForPage(1);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi lưu giảng viên:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            };

            pnlToolbar.Controls.Add(cmbRoleFilter);
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

            dgvAdvisors = new DataGridView();
            dgvAdvisors.Dock = DockStyle.Fill;
            dgvAdvisors.BorderStyle = BorderStyle.None;
            dgvAdvisors.BackgroundColor = Color.White;
            dgvAdvisors.RowHeadersVisible = false;
            dgvAdvisors.AllowUserToAddRows = false;
            dgvAdvisors.AllowUserToDeleteRows = false;
            dgvAdvisors.ReadOnly = true;
            dgvAdvisors.AllowUserToResizeRows = false;
            dgvAdvisors.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvAdvisors.GridColor = ColorTranslator.FromHtml("#EDF2F7"); // Light Gray Gridlines
            dgvAdvisors.RowTemplate.Height = 50;
            dgvAdvisors.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAdvisors.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#F7FAFC");
            dgvAdvisors.DefaultCellStyle.SelectionForeColor = ColorTranslator.FromHtml("#1A202C");
            
            // Header Styling
            dgvAdvisors.EnableHeadersVisualStyles = false;
            dgvAdvisors.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvAdvisors.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvAdvisors.ColumnHeadersHeight = 45;
            dgvAdvisors.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dgvAdvisors.ColumnHeadersDefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#A0AEC0");
            dgvAdvisors.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvAdvisors.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.White;

            // Define Columns
            var colSTT = new DataGridViewTextBoxColumn() { Name = "STT", HeaderText = "STT", FillWeight = 10, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells };
            colSTT.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvAdvisors.Columns.Add(colSTT);

            var colInfo = new DataGridViewTextBoxColumn() { Name = "AdvisorInfo", HeaderText = "MÃ GIẢNG VIÊN", FillWeight = 35, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
            colInfo.HeaderCell.Style.Padding = new Padding(30, 0, 0, 0);
            dgvAdvisors.Columns.Add(colInfo);

            var colRole = new DataGridViewTextBoxColumn() { Name = "Role", HeaderText = "TÊN GIẢNG VIÊN", FillWeight = 25, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
            dgvAdvisors.Columns.Add(colRole);

            var colContact = new DataGridViewTextBoxColumn() { Name = "Contact", HeaderText = "CHỨC VỤ", FillWeight = 25, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
            dgvAdvisors.Columns.Add(colContact);

            var colActions = new DataGridViewTextBoxColumn() { Name = "Actions", HeaderText = "THAO TÁC", FillWeight = 15, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
            colActions.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvAdvisors.Columns.Add(colActions);
            
            // Disable sorting to keep data consistent with manual render
            foreach (DataGridViewColumn col in dgvAdvisors.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            // Custom Cell Painting
            dgvAdvisors.CellPainting += DgvAdvisors_CellPainting;

            // Handle Actions Click
            dgvAdvisors.CellClick += (s, e) => {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvAdvisors.Columns[e.ColumnIndex].Name == "Actions")
                {
                    Rectangle cellRect = dgvAdvisors.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                    Point clickPoint = dgvAdvisors.PointToClient(Cursor.Position);
                    
                    int centerX = cellRect.X + cellRect.Width / 2;
                    int centerY = cellRect.Y + cellRect.Height / 2;
                    
                    Rectangle editRect = new Rectangle(centerX - 35, centerY - 15, 30, 30);
                    Rectangle delRect = new Rectangle(centerX + 5, centerY - 15, 30, 30);

                    if (editRect.Contains(clickPoint))
                    {
                        var row = dgvAdvisors.Rows[e.RowIndex];
                        string currentMaGV = row.Cells["AdvisorInfo"].Value?.ToString() ?? "";
                        string currentTenGV = row.Cells["Role"].Value?.ToString() ?? "";
                        string currentChucVu = row.Cells["Contact"].Value?.ToString() ?? "";
                        string currentEmail = "";
                        try
                        {
                            object emailObj = DatabaseHelper.ExecuteScalar("SELECT email FROM giang_vien WHERE ma_giang_vien = @ma", 
                                new Microsoft.Data.SqlClient.SqlParameter[] { new Microsoft.Data.SqlClient.SqlParameter("@ma", currentMaGV) });
                            currentEmail = emailObj?.ToString() ?? "";
                        }
                        catch {}
                        
                        using (var form = new SuaGiangVienForm(currentMaGV, currentTenGV, currentChucVu, currentEmail))
                        {
                            if (form.ShowDialog() == DialogResult.OK)
                            {
                                try
                                {
                                    string chucVuId = GetOrCreateChucVuId(form.ChucVu);
                                    string updateQuery = "UPDATE giang_vien SET ten_giang_vien = @ten, ma_chuc_vu = @chucVuId, email = @email WHERE ma_giang_vien = @ma";
                                    var parameters = new Microsoft.Data.SqlClient.SqlParameter[]
                                    {
                                        new Microsoft.Data.SqlClient.SqlParameter("@ma", form.MaGV),
                                        new Microsoft.Data.SqlClient.SqlParameter("@ten", form.TenGV),
                                        new Microsoft.Data.SqlClient.SqlParameter("@chucVuId", string.IsNullOrEmpty(chucVuId) ? DBNull.Value : (object)chucVuId),
                                        new Microsoft.Data.SqlClient.SqlParameter("@email", string.IsNullOrEmpty(form.Email) ? DBNull.Value : (object)form.Email)
                                    };

                                    int rows = DatabaseHelper.ExecuteNonQuery(updateQuery, parameters);
                                    if (rows > 0)
                                    {
                                        MessageBox.Show("Đã cập nhật giảng viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        LoadDataForPage(currentPage);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Lỗi cập nhật giảng viên:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                    else if (delRect.Contains(clickPoint))
                    {
                        var result = MessageBox.Show("Bạn có chắc chắn muốn xóa giảng viên này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == DialogResult.Yes)
                        {
                            try
                            {
                                string currentMaGV = dgvAdvisors.Rows[e.RowIndex].Cells["AdvisorInfo"].Value?.ToString() ?? "";
                                string deleteQuery = "DELETE FROM giang_vien WHERE ma_giang_vien = @ma";
                                var parameters = new Microsoft.Data.SqlClient.SqlParameter[]
                                {
                                    new Microsoft.Data.SqlClient.SqlParameter("@ma", currentMaGV)
                                };
                                int rows = DatabaseHelper.ExecuteNonQuery(deleteQuery, parameters);
                                if (rows > 0)
                                {
                                    MessageBox.Show("Đã xóa giảng viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    LoadDataForPage(currentPage);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Lỗi xóa giảng viên (Giảng viên này có thể đang hướng dẫn đồ án, cần cập nhật đồ án trước):\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            };

            pnlGridContainer.Controls.Add(dgvAdvisors);

            // ==========================================
            // 4. Bottom Pagination Bar
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

            flpPagination = new FlowLayoutPanel();
            flpPagination.FlowDirection = FlowDirection.LeftToRight;
            flpPagination.WrapContents = false;
            flpPagination.AutoSize = true;
            flpPagination.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flpPagination.Dock = DockStyle.Right;
            
            pnlPagination.Controls.Add(lblTotalRecords);
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

        private static string GetOrCreateChucVuId(string tenChucVu)
        {
            if (string.IsNullOrEmpty(tenChucVu)) return "";
            try
            {
                object result = DatabaseHelper.ExecuteScalar(
                    "SELECT ma_chuc_vu FROM chuc_vu WHERE ten_chuc_vu = @ten",
                    new Microsoft.Data.SqlClient.SqlParameter[] { new Microsoft.Data.SqlClient.SqlParameter("@ten", tenChucVu) });
                if (result != null) return result.ToString() ?? "";

                string newId = "CV" + Guid.NewGuid().ToString("N").Substring(0, 5).ToUpper();
                DatabaseHelper.ExecuteNonQuery(
                    "INSERT INTO chuc_vu (ma_chuc_vu, ten_chuc_vu) VALUES (@id, @ten)",
                    new Microsoft.Data.SqlClient.SqlParameter[] {
                        new Microsoft.Data.SqlClient.SqlParameter("@id", newId),
                        new Microsoft.Data.SqlClient.SqlParameter("@ten", tenChucVu)
                    });
                return newId;
            }
            catch
            {
                return "";
            }
        }

        private void LoadDataForPage(int page)
        {
            currentPage = page;
            dgvAdvisors.Rows.Clear();
            flpPagination.Controls.Clear();

            string selectedRole = cmbRoleFilter?.SelectedItem?.ToString() ?? "Tất cả chức vụ";
            string searchText = (txtSearch != null && txtSearch.Text != "Tìm kiếm...") ? txtSearch.Text.Trim() : "";

            try
            {
                string countQuery = @"
                    SELECT COUNT(*) 
                    FROM giang_vien g
                    LEFT JOIN chuc_vu c ON g.ma_chuc_vu = c.ma_chuc_vu
                    WHERE 1=1";
                
                string selectQuery = @"
                    SELECT ma_giang_vien, ten_giang_vien, ten_chuc_vu 
                    FROM (
                        SELECT g.ma_giang_vien, g.ten_giang_vien, c.ten_chuc_vu,
                               ROW_NUMBER() OVER (ORDER BY g.ma_giang_vien) as RowNum
                        FROM giang_vien g
                        LEFT JOIN chuc_vu c ON g.ma_chuc_vu = c.ma_chuc_vu
                        WHERE 1=1";

                var paramsList = new System.Collections.Generic.List<Microsoft.Data.SqlClient.SqlParameter>();

                if (selectedRole != "Tất cả chức vụ")
                {
                    countQuery += " AND c.ten_chuc_vu = @role";
                    selectQuery += " AND c.ten_chuc_vu = @role";
                    paramsList.Add(new Microsoft.Data.SqlClient.SqlParameter("@role", selectedRole));
                }

                if (!string.IsNullOrEmpty(searchText))
                {
                    countQuery += " AND (g.ma_giang_vien LIKE @search OR g.ten_giang_vien LIKE @search)";
                    selectQuery += " AND (g.ma_giang_vien LIKE @search OR g.ten_giang_vien LIKE @search)";
                    paramsList.Add(new Microsoft.Data.SqlClient.SqlParameter("@search", "%" + searchText + "%"));
                }

                selectQuery += ") AS RowResults WHERE RowNum BETWEEN @StartRow AND @EndRow";

                object totalObj = DatabaseHelper.ExecuteScalar(countQuery, paramsList.ToArray()) ?? 0;
                totalRecords = Convert.ToInt32(totalObj);

                if (lblTotalRecords != null)
                {
                    lblTotalRecords.Text = $"Tổng số bản ghi: {totalRecords}";
                }

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

                foreach (System.Data.DataRow row in dt.Rows)
                {
                    string ma = row["ma_giang_vien"].ToString() ?? "";
                    string ten = row["ten_giang_vien"].ToString() ?? "";
                    string cv = row["ten_chuc_vu"].ToString() ?? "";
                    dgvAdvisors.Rows.Add("", ma, ten, cv, "");
                }

                flpPagination.Controls.Add(CreatePageButton("<", false));
                for (int i = 1; i <= totalPages; i++)
                {
                    if (i == 1 || i == totalPages || (i >= page - 1 && i <= page + 1))
                    {
                        flpPagination.Controls.Add(CreatePageButton(i.ToString(), i == page));
                    }
                    else if (i == page - 2 || i == page + 2)
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
                flpPagination.Controls.Add(CreatePageButton(">", false));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy dữ liệu giảng viên:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            else
            {
                btn.Click += (s, e) => {
                    int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                    if (text == "<") { if (currentPage > 1) LoadDataForPage(currentPage - 1); }
                    else if (text == ">") { if (currentPage < totalPages) LoadDataForPage(currentPage + 1); }
                    else { LoadDataForPage(int.Parse(text)); }
                };
            }

            return btn;
        }

        private void DgvAdvisors_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return; // Ignore headers

            // Paint default background and selection
            e.PaintBackground(e.CellBounds, (e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            string colName = dgvAdvisors.Columns[e.ColumnIndex].Name;
            int padLeft = (colName == "AdvisorInfo") ? 35 : 5; // Left padding for cell content

            if (colName == "STT")
            {
                using (SolidBrush textBrush = new SolidBrush(ColorTranslator.FromHtml("#4A5568")))
                {
                    StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString((e.RowIndex + 1).ToString(), new Font("Segoe UI", 9f), textBrush, e.CellBounds, sf);
                }
                e.Handled = true;
            }
            else if (colName == "AdvisorInfo")
            {
                string[] parts = e.Value?.ToString()?.Split('|') ?? new string[0];
                if (parts.Length >= 2)
                {
                    // ADVISOR INFO
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
                        g.DrawString(parts[0], new Font("Segoe UI", 9.5f, FontStyle.Bold), nameBrush, e.CellBounds.X + padLeft + 55, e.CellBounds.Y + 15);
                        g.DrawString(parts[1], new Font("Segoe UI", 8.5f, FontStyle.Regular), idBrush, e.CellBounds.X + padLeft + 55, e.CellBounds.Y + 35);
                    }
                }
                else if (e.Value != null)
                {
                    using (SolidBrush textBrush = new SolidBrush(ColorTranslator.FromHtml("#4A5568")))
                    {
                        StringFormat sf = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                        RectangleF textRect = new RectangleF(e.CellBounds.X + padLeft, e.CellBounds.Y, e.CellBounds.Width - padLeft - 5, e.CellBounds.Height);
                        g.DrawString(e.Value.ToString(), new Font("Segoe UI", 9.5f, FontStyle.Regular), textBrush, textRect, sf);
                    }
                }
                e.Handled = true;
            }
            else if (colName == "Role" || colName == "Contact")
            {
                // Normal Text Columns (Role, Contact)
                if (e.Value != null)
                {
                    using (SolidBrush textBrush = new SolidBrush(ColorTranslator.FromHtml("#4A5568")))
                    {
                        StringFormat sf = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                        RectangleF textRect = new RectangleF(e.CellBounds.X + 5, e.CellBounds.Y, e.CellBounds.Width - 10, e.CellBounds.Height);
                        
                        FontStyle style = (colName == "Role") ? FontStyle.Bold : FontStyle.Regular;
                        if (colName == "Role") textBrush.Color = ColorTranslator.FromHtml("#1A202C");
                        
                        g.DrawString(e.Value.ToString(), new Font("Segoe UI", 9.5f, style), textBrush, textRect, sf);
                    }
                }
                e.Handled = true;
            }
            else if (colName == "Actions")
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
            
            // Draw grid line for all cells
            using (Pen gridPen = new Pen(dgvAdvisors.GridColor, 1))
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
                        textBox.Text = "Tìm kiếm theo tên, mã...";
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
