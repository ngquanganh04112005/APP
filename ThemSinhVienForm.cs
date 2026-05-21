using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace TBUProject
{
    public class ThemSinhVienForm : Form
    {
        public string MaSV { get; private set; } = "";
        public string TenSV { get; private set; } = "";
        public string Lop { get; private set; } = "";
        public string GioiTinh { get; private set; } = "";
        public string NgaySinh { get; private set; } = "";

        private TextBox txtMaSV;
        private TextBox txtTenSV;
        private ComboBox cbLop;
        private ComboBox cbGioiTinh;
        private TextBox txtNgaySinh;

        // P/Invoke for rounded corners and window dragging
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

        public ThemSinhVienForm()
        {
            this.Text = "Thêm Sinh Viên Mới";
            this.Size = new Size(460, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None; // Remove default OS borders
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5f);

            // Apply Rounded Corners to the Form
            this.Load += (s, e) => {
                this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 16, 16));
            };

            // Drag handler for the entire form
            this.MouseDown += (s, e) => {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            };

            // Custom Header Panel for Title
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
            lblTitle.Text = "Thêm Sinh Viên Mới";
            lblTitle.Font = new Font("Segoe UI", 13f, FontStyle.Bold);
            lblTitle.ForeColor = ColorTranslator.FromHtml("#2D3748");
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(25, 18);
            pnlHeader.Controls.Add(lblTitle);

            // Close button in header
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

            // Mã SV
            AddLabel("MÃ SINH VIÊN *", 25, 78);
            txtMaSV = new TextBox() { Font = new Font("Segoe UI", 10.5f) };
            CreateRoundedControl(txtMaSV, 25, 100, 410, 36);

            // Tên SV
            AddLabel("HỌ VÀ TÊN *", 25, 153);
            txtTenSV = new TextBox() { Font = new Font("Segoe UI", 10.5f) };
            CreateRoundedControl(txtTenSV, 25, 175, 410, 36);

            // Lớp
            AddLabel("LỚP", 25, 228);
            cbLop = new ComboBox() { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10.5f) };
            try
            {
                var dt = DatabaseHelper.ExecuteQuery("SELECT DISTINCT ten_lop FROM sinh_vien WHERE ten_lop IS NOT NULL");
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    string className = row["ten_lop"].ToString() ?? "";
                    if (!string.IsNullOrEmpty(className)) cbLop.Items.Add(className);
                }
                if (cbLop.Items.Count > 0) cbLop.SelectedIndex = 0;
                else
                {
                    cbLop.Items.AddRange(new string[] { "CNTT-K15", "CNTT-K16", "CNTT-K17" });
                    cbLop.SelectedIndex = 0;
                }
            }
            catch
            {
                cbLop.Items.AddRange(new string[] { "CNTT-K15", "CNTT-K16", "CNTT-K17" });
                cbLop.SelectedIndex = 0;
            }
            CreateRoundedControl(cbLop, 25, 250, 195, 36);

            // Giới tính
            AddLabel("GIỚI TÍNH", 240, 228);
            cbGioiTinh = new ComboBox() { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10.5f) };
            cbGioiTinh.Items.AddRange(new string[] { "Nam", "Nữ", "Khác" });
            cbGioiTinh.SelectedIndex = 0;
            CreateRoundedControl(cbGioiTinh, 240, 250, 195, 36);

            // Ngày sinh
            AddLabel("NGÀY SINH", 25, 303);
            txtNgaySinh = new TextBox() { Font = new Font("Segoe UI", 10.5f) };
            CreateRoundedControl(txtNgaySinh, 25, 325, 410, 36);

            // Footer / Action Buttons
            Button btnCancel = new Button();
            btnCancel.Text = "Hủy";
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 1;
            btnCancel.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#E2E8F0");
            btnCancel.BackColor = Color.White;
            btnCancel.ForeColor = ColorTranslator.FromHtml("#4A5568");
            btnCancel.Size = new Size(95, 38);
            btnCancel.Location = new Point(230, 415);
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            btnCancel.Paint += (s, e) => {
                btnCancel.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnCancel.Width, btnCancel.Height, 8, 8));
            };
            this.Controls.Add(btnCancel);

            Button btnSave = new Button();
            btnSave.Text = "Lưu lại";
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.BackColor = ColorTranslator.FromHtml("#3182CE"); // Consistent primary blue
            btnSave.ForeColor = Color.White;
            btnSave.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btnSave.Size = new Size(110, 38);
            btnSave.Location = new Point(335, 415);
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
            lbl.ForeColor = ColorTranslator.FromHtml("#718096"); // Subdued label color
            lbl.AutoSize = true;
            lbl.Location = new Point(x, y);
            this.Controls.Add(lbl);
        }

        private Panel CreateRoundedControl(Control control, int x, int y, int width, int height)
        {
            Panel container = new Panel();
            container.Size = new Size(width, height);
            container.Location = new Point(x, y);
            container.BackColor = Color.White;

            // Paint custom rounded border for input controls
            container.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, container.Width - 1, container.Height - 1);
                using (GraphicsPath path = CreateRoundRectPath(rect, 8))
                {
                    using (SolidBrush brush = new SolidBrush(Color.White))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                    using (Pen pen = new Pen(ColorTranslator.FromHtml("#CBD5E0"), 1.5f))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };

            // Custom positioning for inputs inside the wrapper
            control.Location = new Point(8, (height - control.Height) / 2);
            control.Width = width - 16;

            if (control is TextBox txt)
            {
                txt.BorderStyle = BorderStyle.None;
            }
            else if (control is ComboBox cb)
            {
                cb.FlatStyle = FlatStyle.Flat;
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
            // Draw a subtle border around the borderless form to prevent merging with white backgrounds
            using (Pen borderPen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1.5f))
            {
                e.Graphics.DrawRectangle(borderPen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaSV.Text) || string.IsNullOrWhiteSpace(txtTenSV.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Mã Sinh Viên và Tên Sinh Viên!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MaSV = txtMaSV.Text.Trim();
            TenSV = txtTenSV.Text.Trim();
            Lop = cbLop.SelectedItem?.ToString() ?? "";
            GioiTinh = cbGioiTinh.SelectedItem?.ToString() ?? "";
            NgaySinh = txtNgaySinh.Text.Trim();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
