using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace TBUProject
{
    public class AddEditProjectForm : Form
    {
        public string MaDeTai { get; private set; } = "";
        public string TenDeTai { get; private set; } = "";
        public string LoaiDeTai { get; private set; } = "";
        public string MaSV { get; private set; } = "";
        public string TenSV { get; private set; } = "";
        public string GiangVienHD { get; private set; } = "";
        public string NgayNop { get; private set; } = "";
        public string UrlTrienKhai { get; private set; } = "";
        public string FileDinhKem { get; private set; } = "";

        private TextBox txtMaDeTai;
        private TextBox txtTenDeTai;
        private ComboBox cbLoaiDeTai;
        private TextBox txtMaSV;
        private TextBox txtTenSV;
        private TextBox txtGiangVienHD;
        private TextBox txtNgayNop;
        private TextBox txtUrlTrienKhai;
        private TextBox txtFileDinhKem;

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        public AddEditProjectForm(bool isEdit = false)
        {
            this.Text = isEdit ? "Sửa Đồ Án" : "Thêm Đồ Án Mới";
            this.Size = new Size(500, 720); // Taller to fit all fields
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5f);

            this.Load += (s, e) => {
                this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 16, 16));
            };

            // Header Panel
            Panel pnlHeader = new Panel();
            pnlHeader.Height = 60;
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.BackColor = ColorTranslator.FromHtml("#F7FAFC");
            pnlHeader.MouseDown += (s, e) => {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            };

            Label lblTitle = new Label();
            lblTitle.Text = this.Text;
            lblTitle.Font = new Font("Segoe UI", 13f, FontStyle.Bold);
            lblTitle.ForeColor = ColorTranslator.FromHtml("#2D3748");
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(25, 18);
            pnlHeader.Controls.Add(lblTitle);

            Button btnClose = new Button();
            btnClose.Text = "✕";
            btnClose.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            btnClose.ForeColor = ColorTranslator.FromHtml("#A0AEC0");
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Size = new Size(35, 35);
            btnClose.Location = new Point(this.Width - 45, 12);
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            pnlHeader.Controls.Add(btnClose);
            this.Controls.Add(pnlHeader);

            // Setup Fields
            int startY = 80;
            int gap = 65;

            // Row 1: Tên đề tài (Full Width)
            AddLabel("TÊN ĐỀ TÀI *", 25, startY);
            txtTenDeTai = new TextBox() { Font = new Font("Segoe UI", 10.5f) };
            CreateRoundedControl(txtTenDeTai, 25, startY + 22, 450, 36);

            // Row 2: Mã đề tài (Left) & Loại (Right)
            startY += gap;
            AddLabel("MÃ ĐỀ TÀI *", 25, startY);
            txtMaDeTai = new TextBox() { Font = new Font("Segoe UI", 10.5f) };
            CreateRoundedControl(txtMaDeTai, 25, startY + 22, 215, 36);

            AddLabel("LOẠI ĐỀ TÀI", 260, startY);
            cbLoaiDeTai = new ComboBox() { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10.5f) };
            cbLoaiDeTai.Items.AddRange(new string[] { "Tiểu luận", "Khoá luận", "Đồ án" });
            cbLoaiDeTai.SelectedIndex = 0;
            CreateRoundedControl(cbLoaiDeTai, 260, startY + 22, 215, 36);

            // Row 3: Mã SV (Left) & Tên SV (Right)
            startY += gap;
            AddLabel("MÃ SINH VIÊN *", 25, startY);
            txtMaSV = new TextBox() { Font = new Font("Segoe UI", 10.5f) };
            CreateRoundedControl(txtMaSV, 25, startY + 22, 215, 36);

            AddLabel("TÊN SINH VIÊN *", 260, startY);
            txtTenSV = new TextBox() { Font = new Font("Segoe UI", 10.5f) };
            CreateRoundedControl(txtTenSV, 260, startY + 22, 215, 36);

            // Row 4: Giảng viên HD (Full Width)
            startY += gap;
            AddLabel("GIẢNG VIÊN HƯỚNG DẪN", 25, startY);
            txtGiangVienHD = new TextBox() { Font = new Font("Segoe UI", 10.5f) };
            CreateRoundedControl(txtGiangVienHD, 25, startY + 22, 450, 36);

            // Row 5: Ngày nộp (Full Width)
            startY += gap;
            AddLabel("NGÀY NỘP", 25, startY);
            txtNgayNop = new TextBox() { Font = new Font("Segoe UI", 10.5f) };
            CreateRoundedControl(txtNgayNop, 25, startY + 22, 450, 36);

            // Row 6: URL Triển khai (Full Width)
            startY += gap;
            AddLabel("URL TRIỂN KHAI", 25, startY);
            txtUrlTrienKhai = new TextBox() { Font = new Font("Segoe UI", 10.5f) };
            CreateRoundedControl(txtUrlTrienKhai, 25, startY + 22, 450, 36);

            // Row 7: File Đính Kèm (Text + Button)
            startY += gap;
            AddLabel("FILE ĐÍNH KÈM", 25, startY);
            txtFileDinhKem = new TextBox() { Font = new Font("Segoe UI", 10.5f), ReadOnly = true };
            CreateRoundedControl(txtFileDinhKem, 25, startY + 22, 350, 36);

            Button btnChooseFile = new Button();
            btnChooseFile.Text = "Chọn";
            btnChooseFile.FlatStyle = FlatStyle.Flat;
            btnChooseFile.FlatAppearance.BorderSize = 0;
            btnChooseFile.BackColor = ColorTranslator.FromHtml("#EDF2F7");
            btnChooseFile.ForeColor = ColorTranslator.FromHtml("#4A5568");
            btnChooseFile.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            btnChooseFile.Size = new Size(90, 36);
            btnChooseFile.Location = new Point(385, startY + 22);
            btnChooseFile.Cursor = Cursors.Hand;
            btnChooseFile.Click += (s, e) => {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Tài liệu|*.pdf;*.docx;*.zip;*.rar|Tất cả các file|*.*";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        txtFileDinhKem.Text = ofd.FileName;
                    }
                }
            };
            btnChooseFile.Paint += (s, e) => {
                btnChooseFile.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnChooseFile.Width, btnChooseFile.Height, 8, 8));
            };
            this.Controls.Add(btnChooseFile);

            // Action Buttons (Bottom)
            int bottomY = this.Height - 65;
            Button btnCancel = new Button();
            btnCancel.Text = "Hủy";
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 1;
            btnCancel.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#E2E8F0");
            btnCancel.BackColor = Color.White;
            btnCancel.ForeColor = ColorTranslator.FromHtml("#4A5568");
            btnCancel.Size = new Size(95, 40);
            btnCancel.Location = new Point(270, bottomY);
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            btnCancel.Paint += (s, e) => {
                btnCancel.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnCancel.Width, btnCancel.Height, 8, 8));
            };
            this.Controls.Add(btnCancel);

            Button btnSave = new Button();
            btnSave.Text = "Lưu Đồ Án";
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.BackColor = ColorTranslator.FromHtml("#3182CE");
            btnSave.ForeColor = Color.White;
            btnSave.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btnSave.Size = new Size(110, 40);
            btnSave.Location = new Point(375, bottomY);
            btnSave.Cursor = Cursors.Hand;
            btnSave.Click += BtnSave_Click;
            btnSave.Paint += (s, e) => {
                btnSave.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnSave.Width, btnSave.Height, 8, 8));
            };
            this.Controls.Add(btnSave);
        }

        private void AddLabel(string text, int x, int y)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            lbl.ForeColor = ColorTranslator.FromHtml("#718096");
            lbl.AutoSize = true;
            lbl.Location = new Point(x, y);
            this.Controls.Add(lbl);
        }

        private Panel CreateRoundedControl(Control control, int x, int y, int width, int height, bool isDateTimePicker = false)
        {
            Panel container = new Panel();
            container.Size = new Size(width, height);
            container.Location = new Point(x, y);
            container.BackColor = Color.White;

            if (isDateTimePicker) {
                container.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, container.Width, container.Height, 8, 8));
            }

            container.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, container.Width - 1, container.Height - 1);
                using (GraphicsPath path = CreateRoundRectPath(rect, 8))
                {
                    using (SolidBrush brush = new SolidBrush(Color.White))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                    if(!isDateTimePicker) {
                        using (Pen pen = new Pen(ColorTranslator.FromHtml("#CBD5E0"), 1.5f))
                        {
                            e.Graphics.DrawPath(pen, path);
                        }
                    } else {
                        using (Pen pen = new Pen(ColorTranslator.FromHtml("#CBD5E0"), 2f))
                        {
                            e.Graphics.DrawPath(pen, path);
                        }
                    }
                }
            };

            if (isDateTimePicker) {
                control.Location = new Point(-2, (height - control.Height) / 2 - 1);
                control.Width = width + 4;
            } else {
                control.Location = new Point(8, (height - control.Height) / 2);
                control.Width = width - 16;
                if (control is TextBox txt) txt.BorderStyle = BorderStyle.None;
                else if (control is ComboBox cb) cb.FlatStyle = FlatStyle.Flat;
            }

            container.Controls.Add(control);
            this.Controls.Add(container);
            return container;
        }

        private GraphicsPath CreateRoundRectPath(Rectangle rect, int radius)
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Pen borderPen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1.5f))
            {
                e.Graphics.DrawRectangle(borderPen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaDeTai.Text) || string.IsNullOrWhiteSpace(txtTenDeTai.Text) || string.IsNullOrWhiteSpace(txtMaSV.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Mã đề tài, Tên đề tài và Mã sinh viên!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MaDeTai = txtMaDeTai.Text.Trim();
            TenDeTai = txtTenDeTai.Text.Trim();
            LoaiDeTai = cbLoaiDeTai.SelectedItem?.ToString() ?? "";
            MaSV = txtMaSV.Text.Trim();
            TenSV = txtTenSV.Text.Trim();
            GiangVienHD = txtGiangVienHD.Text.Trim();
            NgayNop = txtNgayNop.Text.Trim();
            UrlTrienKhai = txtUrlTrienKhai.Text.Trim();
            FileDinhKem = txtFileDinhKem.Text.Trim();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public void LoadData(string maDeTai, string tenDeTai, string tenSV, string loai, string ngayNop)
        {
            txtMaDeTai.Text = maDeTai;
            txtMaDeTai.ReadOnly = true;
            txtMaDeTai.BackColor = ColorTranslator.FromHtml("#EDF2F7");
            
            txtTenDeTai.Text = tenDeTai;
            txtTenSV.Text = tenSV;
            
            if (cbLoaiDeTai.Items.Contains(loai))
                cbLoaiDeTai.SelectedItem = loai;
                
            txtNgayNop.Text = ngayNop;
        }
    }
}
