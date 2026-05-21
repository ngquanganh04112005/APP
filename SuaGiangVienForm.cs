using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace TBUProject
{
    public class SuaGiangVienForm : Form
    {
        public string MaGV { get; private set; } = "";
        public string TenGV { get; private set; } = "";
        public string ChucVu { get; private set; } = "";
        public string Email { get; private set; } = "";

        private TextBox txtMaGV;
        private TextBox txtTenGV;
        private ComboBox cbChucVu;
        private TextBox txtEmail;

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

        public SuaGiangVienForm(string currentMaGV, string currentTenGV, string currentChucVu, string currentEmail)
        {
            this.Text = "Sửa Thông Tin Giảng Viên";
            this.Size = new Size(460, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None; // Remove default OS borders
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5f);

            // Apply Rounded Corners to the Form
            this.Load += (s, e) => {
                this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 16, 16));
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
            lblTitle.Text = "Sửa Thông Tin Giảng Viên";
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

            // Mã GV
            AddLabel("MÃ GIẢNG VIÊN *", 25, 78);
            txtMaGV = new TextBox() { Font = new Font("Segoe UI", 10.5f), Text = currentMaGV, ReadOnly = true, BackColor = ColorTranslator.FromHtml("#EDF2F7") };
            CreateRoundedControl(txtMaGV, 25, 100, 410, 36);

            // Tên GV
            AddLabel("HỌ VÀ TÊN *", 25, 153);
            txtTenGV = new TextBox() { Font = new Font("Segoe UI", 10.5f), Text = currentTenGV };
            CreateRoundedControl(txtTenGV, 25, 175, 410, 36);

            // Chức vụ
            AddLabel("CHỨC VỤ", 25, 228);
            cbChucVu = new ComboBox() { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10.5f) };
            cbChucVu.Items.AddRange(new string[] { "Giảng viên", "Trưởng khoa", "Phó trưởng khoa", "Trưởng bộ môn" });
            if (cbChucVu.Items.Contains(currentChucVu))
                cbChucVu.SelectedItem = currentChucVu;
            else
                cbChucVu.SelectedIndex = 0;
            CreateRoundedControl(cbChucVu, 25, 250, 410, 36);

            // Email
            AddLabel("EMAIL", 25, 303);
            txtEmail = new TextBox() { Font = new Font("Segoe UI", 10.5f), Text = currentEmail };
            CreateRoundedControl(txtEmail, 25, 325, 410, 36);

            // Action Buttons
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
            btnSave.Text = "Cập nhật";
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.BackColor = ColorTranslator.FromHtml("#3182CE"); // Primary blue
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
            lbl.ForeColor = ColorTranslator.FromHtml("#718096");
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
            using (Pen borderPen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1.5f))
            {
                e.Graphics.DrawRectangle(borderPen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaGV.Text) || string.IsNullOrWhiteSpace(txtTenGV.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Mã Giảng Viên và Tên Giảng Viên!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MaGV = txtMaGV.Text.Trim();
            TenGV = txtTenGV.Text.Trim();
            ChucVu = cbChucVu.SelectedItem?.ToString() ?? "";
            Email = txtEmail.Text.Trim();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
