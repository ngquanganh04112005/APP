using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

#nullable disable

namespace TBUProject
{
    public class ChiTietDoAnForm : Form
    {
        private string maDoAn;
        private bool hasChanges = false;

        // Editable fields
        private ComboBox cmbLoai;
        private RoundedTextBox txtMaSV;
        private RoundedTextBox txtTenSV;
        private RoundedTextBox txtLopSV;
        private RoundedTextBox txtSdtSV;
        private RoundedTextBox txtEmailSV;
        private RoundedTextBox txtMaGV;
        private RoundedTextBox txtTenGV;
        private RoundedTextBox txtChucVu;
        private RoundedTextBox txtSdtGV;
        private RoundedTextBox txtEmailGV;

        public ChiTietDoAnForm(string maDoAn)
        {
            this.maDoAn = maDoAn;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Chi Tiết Đồ Án";
            this.Size = new Size(920, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = ColorTranslator.FromHtml("#F7FAFC");
            this.Font = new Font("Segoe UI", 10f, FontStyle.Regular);
        }

        private void LoadData()
        {
            this.Controls.Clear();

            string query = @"
                SELECT d.ma_do_an, d.ten_do_an, d.ngay_nop, d.file_dinh_kem,
                       s.ma_sinh_vien, s.ten_sinh_vien, s.ten_lop, s.email as email_sv, s.so_dien_thoai as sdt_sv,
                       g.ma_giang_vien, g.ten_giang_vien, g.email as email_gv, g.so_dien_thoai as sdt_gv,
                       c.ten_chuc_vu,
                       l.ten_loai
                FROM do_an d
                INNER JOIN sinh_vien s ON d.ma_sinh_vien = s.ma_sinh_vien
                LEFT JOIN giang_vien g ON d.ma_giang_vien_huong_dan = g.ma_giang_vien
                LEFT JOIN chuc_vu c ON g.ma_chuc_vu = c.ma_chuc_vu
                LEFT JOIN loai_do_an l ON d.ma_loai = l.ma_loai
                WHERE d.ma_do_an = @maDoAn";

            var parameters = new SqlParameter[] { new SqlParameter("@maDoAn", maDoAn) };

            DataTable dt = null;
            try { dt = DatabaseHelper.ExecuteQuery(query, parameters); }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy dữ liệu:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dt == null || dt.Rows.Count == 0)
            {
                Label err = new Label() { Text = "Không tìm thấy thông tin đồ án.", ForeColor = Color.Red, AutoSize = true, Location = new Point(20, 20) };
                this.Controls.Add(err);
                return;
            }

            DataRow row = dt.Rows[0];

            string tenDoAn   = GetStr(row, "ten_do_an");
            string loaiDoAn  = GetStr(row, "ten_loai", "Đồ án");
            string ngayNop   = row["ngay_nop"] != DBNull.Value ? Convert.ToDateTime(row["ngay_nop"]).ToString("dd/MM/yyyy") : "";
            string tenSV     = GetStr(row, "ten_sinh_vien", "Chưa cập nhật");
            string maSV      = GetStr(row, "ma_sinh_vien", "Chưa cập nhật");
            string lopSV     = GetStr(row, "ten_lop", "Chưa cập nhật");
            string emailSV   = GetStr(row, "email_sv", "Chưa cập nhật");
            string sdtSV     = GetStr(row, "sdt_sv", "Chưa cập nhật");
            string maGV      = GetStr(row, "ma_giang_vien", "Chưa cập nhật");
            string tenGV     = GetStr(row, "ten_giang_vien", "Chưa phân công");
            string chucVu    = GetStr(row, "ten_chuc_vu", "Giảng viên");
            string emailGV   = GetStr(row, "email_gv", "Chưa cập nhật");
            string sdtGV     = GetStr(row, "sdt_gv", "Chưa cập nhật");

            // ==============================
            // HEADER
            // ==============================
            Panel pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 90;
            pnlHeader.BackColor = ColorTranslator.FromHtml("#0F2042");
            this.Controls.Add(pnlHeader);

            // Read-only title in header
            Label lblTenDoAn = new Label();
            lblTenDoAn.Text = tenDoAn;
            lblTenDoAn.Font = new Font("Segoe UI", 16f, FontStyle.Bold);
            lblTenDoAn.ForeColor = Color.White;
            lblTenDoAn.BackColor = Color.Transparent;
            lblTenDoAn.Location = new Point(27, 0);
            lblTenDoAn.Size = new Size(this.Width - 80, 90);
            lblTenDoAn.TextAlign = ContentAlignment.MiddleLeft;
            lblTenDoAn.UseMnemonic = false;
            pnlHeader.Controls.Add(lblTenDoAn);

            // ==============================
            // SCROLLABLE CONTENT AREA
            // ==============================
            Panel pnlContent = new Panel();
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.AutoScroll = true;
            pnlContent.Padding = new Padding(30, 20, 30, 20);
            pnlContent.BackColor = ColorTranslator.FromHtml("#F7FAFC");
            this.Controls.Add(pnlContent);
            pnlContent.BringToFront();

            int y = 10;

            // ---- Card: Thông Tin Chung ----
            RoundedPanel pnlInfo = CreateCard("📋  Thông Tin Đồ Án", 830, 100, pnlContent, ref y);

            // Row 1: Mã Đồ Án  |  Loại
            AddFieldLabel(pnlInfo, "Mã Đồ Án:", 20, 50);
            RoundedTextBox txtMaDoAnView = CreateTextBox(pnlInfo, maDoAn, 165, 47, 180, 32);
            txtMaDoAnView.ReadOnly = true;
            txtMaDoAnView.BackColor = Color.White;

            AddFieldLabel(pnlInfo, "Loại Đề Tài:", 380, 50);
            cmbLoai = new ComboBox();
            cmbLoai.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLoai.Items.AddRange(new string[] { "Đồ án", "Khóa luận", "Tiểu luận" });
            cmbLoai.Font = new Font("Segoe UI", 10f);
            cmbLoai.BackColor = Color.White;
            cmbLoai.ForeColor = ColorTranslator.FromHtml("#2D3748");
            cmbLoai.FlatStyle = FlatStyle.Flat;
            cmbLoai.Location = new Point(475, 47);
            cmbLoai.Size = new Size(180, 32);
            if (cmbLoai.Items.Contains(loaiDoAn)) cmbLoai.SelectedItem = loaiDoAn;
            else if (cmbLoai.Items.Count > 0) cmbLoai.SelectedIndex = 0;
            pnlInfo.Controls.Add(cmbLoai);

            // ---- Card: Sinh Viên ----
            RoundedPanel pnlStudent = CreateCard("🎓  Sinh Viên Thực Hiện", 400, 260, pnlContent, ref y);

            AddFieldLabel(pnlStudent, "Họ và Tên:", 20, 50);
            txtTenSV = CreateTextBox(pnlStudent, tenSV, 145, 47, 235, 30);

            AddFieldLabel(pnlStudent, "Mã Sinh Viên:", 20, 95);
            txtMaSV = CreateTextBox(pnlStudent, maSV, 145, 92, 235, 30);

            AddFieldLabel(pnlStudent, "Lớp:", 20, 140);
            txtLopSV = CreateTextBox(pnlStudent, lopSV, 145, 137, 235, 30);

            AddFieldLabel(pnlStudent, "Số Điện Thoại:", 20, 185);
            txtSdtSV = CreateTextBox(pnlStudent, sdtSV, 145, 182, 235, 30);

            AddFieldLabel(pnlStudent, "Email:", 20, 225);
            txtEmailSV = CreateTextBox(pnlStudent, emailSV, 145, 222, 235, 30);

            // ---- Card: Giảng Viên ----
            int gvX = 440;
            // Place side by side with student card
            int savedY = y;
            RoundedPanel pnlLecturer = new RoundedPanel();
            pnlLecturer.Width = 400;
            pnlLecturer.Height = 260;
            pnlLecturer.BackColor = Color.White;
            pnlLecturer.BorderRadius = 10;
            pnlLecturer.Location = new Point(gvX, savedY - 260 - 15);
            pnlContent.Controls.Add(pnlLecturer);

            AddCardTitle(pnlLecturer, "👨‍🏫  Giảng Viên Hướng Dẫn");

            AddFieldLabel(pnlLecturer, "Họ và Tên:", 20, 50);
            txtTenGV = CreateTextBox(pnlLecturer, tenGV, 145, 47, 235, 30);

            AddFieldLabel(pnlLecturer, "Mã Giảng Viên:", 20, 95);
            txtMaGV = CreateTextBox(pnlLecturer, maGV, 145, 92, 235, 30);
            txtMaGV.ReadOnly = true;
            txtMaGV.BackColor = Color.White;

            AddFieldLabel(pnlLecturer, "Chức Vụ:", 20, 140);
            txtChucVu = CreateTextBox(pnlLecturer, chucVu, 145, 137, 235, 30);

            AddFieldLabel(pnlLecturer, "Số Điện Thoại:", 20, 185);
            txtSdtGV = CreateTextBox(pnlLecturer, sdtGV, 145, 182, 235, 30);

            AddFieldLabel(pnlLecturer, "Email:", 20, 225);
            txtEmailGV = CreateTextBox(pnlLecturer, emailGV, 145, 222, 235, 30);

            // ---- Footer Buttons ----
            Panel pnlFooter = new Panel();
            pnlFooter.Width = 830;
            pnlFooter.Height = 55;
            pnlFooter.Location = new Point(0, y);
            pnlContent.Controls.Add(pnlFooter);
            y += 65;

            RoundedButton btnSave = new RoundedButton();
            btnSave.Text = "💾  Lưu Thay Đổi";
            btnSave.Size = new Size(150, 42);
            btnSave.BackColor = ColorTranslator.FromHtml("#38A169");
            btnSave.ForeColor = Color.White;
            btnSave.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            btnSave.BorderRadius = 10;
            btnSave.Cursor = Cursors.Hand;
            btnSave.Location = new Point(540, 5);
            btnSave.Click += BtnSave_Click;
            pnlFooter.Controls.Add(btnSave);

            RoundedButton btnClose = new RoundedButton();
            btnClose.Text = "Đóng";
            btnClose.Size = new Size(110, 42);
            btnClose.BackColor = ColorTranslator.FromHtml("#3182CE"); // Ocean Blue
            btnClose.ForeColor = Color.White;
            btnClose.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            btnClose.BorderRadius = 10;
            btnClose.Cursor = Cursors.Hand;
            btnClose.Location = new Point(705, 5);
            btnClose.Click += (s, e) => {
                this.DialogResult = hasChanges ? DialogResult.OK : DialogResult.Cancel;
                this.Close();
            };
            pnlFooter.Controls.Add(btnClose);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Resolve IDs
                string loaiId = ResolveLoaiDoAnId(cmbLoai.SelectedItem?.ToString() ?? "");
                string maSVValue = txtMaSV.Text.Trim();
                string tenSVValue = txtTenSV.Text.Trim();
                EnsureStudentExists(maSVValue, tenSVValue);
                UpdateStudentInfo(maSVValue, tenSVValue, txtLopSV.Text.Trim(), txtSdtSV.Text.Trim(), txtEmailSV.Text.Trim());

                string maGVValue = txtMaGV.Text.Trim();
                UpdateGiangVienInfo(maGVValue, txtTenGV.Text.Trim(), txtSdtGV.Text.Trim(), txtEmailGV.Text.Trim());

                string updateQuery = @"UPDATE do_an SET 
                    ma_sinh_vien = @maSV, 
                    ma_giang_vien_huong_dan = @maGV, 
                    ma_loai = @loai 
                    WHERE ma_do_an = @ma";

                var sqlParams = new SqlParameter[]
                {
                    new SqlParameter("@maSV", maSVValue),
                    new SqlParameter("@maGV", string.IsNullOrEmpty(maGVValue) ? DBNull.Value : (object)maGVValue),
                    new SqlParameter("@loai", string.IsNullOrEmpty(loaiId)   ? DBNull.Value : (object)loaiId),
                    new SqlParameter("@ma",   maDoAn)
                };

                int rows = DatabaseHelper.ExecuteNonQuery(updateQuery, sqlParams);
                if (rows > 0)
                {
                    MessageBox.Show("Đã lưu thay đổi thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    hasChanges = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu dữ liệu:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---- Helpers ----

        private static string GetStr(DataRow row, string col, string fallback = "")
        {
            return (row[col] != DBNull.Value && !string.IsNullOrWhiteSpace(row[col].ToString()))
                ? row[col].ToString() : fallback;
        }

        private RoundedTextBox CreateTextBox(Panel parent, string value, int x, int y, int width, int height)
        {
            RoundedTextBox tb = new RoundedTextBox();
            tb.Text = value;
            tb.Location = new Point(x, y);
            tb.Size = new Size(width, height);
            parent.Controls.Add(tb);
            return tb;
        }

        private void AddFieldLabel(Panel parent, string text, int x, int y)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            lbl.ForeColor = ColorTranslator.FromHtml("#718096");
            lbl.Location = new Point(x, y + 5);
            lbl.AutoSize = true;
            parent.Controls.Add(lbl);
        }

        private RoundedPanel CreateCard(string title, int width, int height, Panel parent, ref int y)
        {
            RoundedPanel pnl = new RoundedPanel();
            pnl.Width = width;
            pnl.Height = height;
            pnl.BackColor = Color.White;
            pnl.BorderRadius = 10;
            pnl.Location = new Point(0, y);
            parent.Controls.Add(pnl);
            y += height + 15;

            AddCardTitle(pnl, title);
            return pnl;
        }

        private void AddCardTitle(Panel pnl, string title)
        {
            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
            lblTitle.ForeColor = ColorTranslator.FromHtml("#2D3748");
            lblTitle.Location = new Point(20, 12);
            lblTitle.AutoSize = true;
            pnl.Controls.Add(lblTitle);

            Panel line = new Panel();
            line.BackColor = ColorTranslator.FromHtml("#E2E8F0");
            line.Height = 1;
            line.Width = pnl.Width - 40;
            line.Location = new Point(20, 38);
            pnl.Controls.Add(line);
        }

        private static void UpdateStudentInfo(string maSV, string tenSV, string lop, string sdt, string email)
        {
            try
            {
                DatabaseHelper.ExecuteNonQuery(
                    "UPDATE sinh_vien SET ten_sinh_vien=@ten, ten_lop=@lop, so_dien_thoai=@sdt, email=@email WHERE ma_sinh_vien=@ma",
                    new SqlParameter[] {
                        new SqlParameter("@ten",   tenSV),
                        new SqlParameter("@lop",   lop),
                        new SqlParameter("@sdt",   string.IsNullOrEmpty(sdt)   ? DBNull.Value : (object)sdt),
                        new SqlParameter("@email", string.IsNullOrEmpty(email) ? DBNull.Value : (object)email),
                        new SqlParameter("@ma",    maSV)
                    });
            }
            catch { }
        }

        private static void UpdateGiangVienInfo(string maGV, string tenGV, string sdt, string email)
        {
            if (string.IsNullOrEmpty(maGV)) return;
            try
            {
                DatabaseHelper.ExecuteNonQuery(
                    "UPDATE giang_vien SET ten_giang_vien=@ten, so_dien_thoai=@sdt, email=@email WHERE ma_giang_vien=@ma",
                    new SqlParameter[] {
                        new SqlParameter("@ten",   tenGV),
                        new SqlParameter("@sdt",   string.IsNullOrEmpty(sdt)   ? DBNull.Value : (object)sdt),
                        new SqlParameter("@email", string.IsNullOrEmpty(email) ? DBNull.Value : (object)email),
                        new SqlParameter("@ma",    maGV)
                    });
            }
            catch { }
        }

        private static void EnsureStudentExists(string maSV, string tenSV)
        {
            try
            {
                object result = DatabaseHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM sinh_vien WHERE ma_sinh_vien = @ma",
                    new SqlParameter[] { new SqlParameter("@ma", maSV) });
                if (Convert.ToInt32(result) == 0)
                {
                    DatabaseHelper.ExecuteNonQuery(
                        "INSERT INTO sinh_vien (ma_sinh_vien, ten_sinh_vien, ten_lop, ngay_sinh, gioi_tinh) VALUES (@ma, @ten, @lop, @ns, @gt)",
                        new SqlParameter[] {
                            new SqlParameter("@ma",  maSV),
                            new SqlParameter("@ten", string.IsNullOrEmpty(tenSV) ? "Sinh viên chưa cập nhật" : tenSV),
                            new SqlParameter("@lop", "CNTT-K15"),
                            new SqlParameter("@ns",  DateTime.Now),
                            new SqlParameter("@gt",  "Nam")
                        });
                }
            }
            catch { }
        }

        private static string ResolveLoaiDoAnId(string loai)
        {
            if (string.IsNullOrEmpty(loai)) return "";
            string name = loai.Trim().ToLower();
            if (name.Contains("khóa luận") || name.Contains("khoá luận")) return "LDA02";
            if (name.Contains("tiểu luận") || name.Contains("tiêu luận")) return "LDA03";
            return "LDA01";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (hasChanges && this.DialogResult != DialogResult.OK)
                this.DialogResult = DialogResult.OK;
        }

        // ---- Inner custom controls ----

        private class RoundedPanel : Panel
        {
            public int BorderRadius { get; set; } = 10;
            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                using (GraphicsPath path = GetPath(rect, BorderRadius))
                {
                    this.Region = new Region(path);
                    using (SolidBrush b = new SolidBrush(this.BackColor))
                        e.Graphics.FillPath(b, path);
                    using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1f))
                        e.Graphics.DrawPath(pen, path);
                }
            }
            private GraphicsPath GetPath(Rectangle r, int rad)
            {
                GraphicsPath p = new GraphicsPath();
                int d = rad * 2;
                p.AddArc(r.X, r.Y, d, d, 180, 90);
                p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
                p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
                p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
                p.CloseFigure();
                return p;
            }
        }

        private class RoundedButton : Button
        {
            public int BorderRadius { get; set; } = 8;
            private bool hovered = false;
            public RoundedButton()
            {
                this.FlatStyle = FlatStyle.Flat;
                this.FlatAppearance.BorderSize = 0;
                this.MouseEnter += (s, e) => { hovered = true; Invalidate(); };
                this.MouseLeave += (s, e) => { hovered = false; Invalidate(); };
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, Width, Height);
                using (GraphicsPath path = GetPath(rect, BorderRadius))
                {
                    this.Region = new Region(path);
                    Color fill = hovered ? ControlPaint.Light(this.BackColor, 0.15f) : this.BackColor;
                    using (SolidBrush b = new SolidBrush(fill))
                        e.Graphics.FillPath(b, path);
                    TextRenderer.DrawText(e.Graphics, this.Text, this.Font, rect, this.ForeColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
            }
            private GraphicsPath GetPath(Rectangle r, int rad)
            {
                GraphicsPath p = new GraphicsPath();
                int d = rad * 2;
                p.AddArc(r.X, r.Y, d, d, 180, 90);
                p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
                p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
                p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
                p.CloseFigure();
                return p;
            }
        }

        private class RoundedTextBox : UserControl
        {
            private TextBox textBox;
            public int BorderRadius { get; set; } = 8;
            public Color BorderColor { get; set; } = ColorTranslator.FromHtml("#CBD5E0");
            
            public override string Text 
            { 
                get => textBox.Text; 
                set => textBox.Text = value ?? ""; 
            }
            
            public bool ReadOnly 
            {
                get => textBox.ReadOnly;
                set
                {
                    textBox.ReadOnly = value;
                    textBox.BackColor = Color.White;
                }
            }

            public override Color BackColor
            {
                get => base.BackColor;
                set
                {
                    base.BackColor = value;
                    if (textBox != null) textBox.BackColor = value;
                }
            }

            public RoundedTextBox()
            {
                textBox = new TextBox();
                textBox.BorderStyle = BorderStyle.None;
                textBox.Font = new Font("Segoe UI", 10f, FontStyle.Regular);
                textBox.ForeColor = ColorTranslator.FromHtml("#2D3748");
                textBox.BackColor = Color.White;
                textBox.Location = new Point(10, 7);
                textBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                this.BackColor = Color.White;
                this.Padding = new Padding(10, 7, 10, 7);
                this.Size = new Size(200, 30);
                textBox.Width = this.Width - 20;
                this.Controls.Add(textBox);

                textBox.GotFocus += (s, e) => { BorderColor = ColorTranslator.FromHtml("#3182CE"); this.Invalidate(); };
                textBox.LostFocus += (s, e) => { BorderColor = ColorTranslator.FromHtml("#CBD5E0"); this.Invalidate(); };
            }

            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);
                textBox.Width = this.Width - 20;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                using (GraphicsPath path = GetRoundedPath(rect, BorderRadius))
                {
                    using (SolidBrush brush = new SolidBrush(this.BackColor))
                        e.Graphics.FillPath(brush, path);
                    using (Pen pen = new Pen(BorderColor, 1.2f))
                        e.Graphics.DrawPath(pen, path);
                }
            }

            private GraphicsPath GetRoundedPath(Rectangle r, int rad)
            {
                GraphicsPath p = new GraphicsPath();
                int d = rad * 2;
                p.AddArc(r.X, r.Y, d, d, 180, 90);
                p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
                p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
                p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
                p.CloseFigure();
                return p;
            }
        }
    }
}
