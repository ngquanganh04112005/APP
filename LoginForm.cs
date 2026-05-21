using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

namespace TBUProject
{
    public class LoginForm : Form
    {
        // Các thành phần của Panel trái
        private BrandPanel leftPanel;
        private Label lblLogo;
        private Label lblHeading;
        private Label lblDescription;
        private Label lblFooter;

        // Các thành phần của Panel phải
        private Panel rightPanel;
        private CardPanel cardContainer;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblEmail;
        private RoundedTextBox txtEmail;
        private Label lblPassword;
        private LinkLabel lnkForgotPassword;
        private RoundedTextBox txtPassword;
        private CheckBox chkRememberMe;
        private RoundedButton btnSignIn;
        private Label lblTerms;

        public LoginForm()
        {
            InitializeCustomComponent();
        }

        private void InitializeCustomComponent()
        {
            // Thiết lập Form chính
            this.Text = "Đăng Nhập Hệ Thống Quản Lý Đồ Án - TBU";
            this.ClientSize = new Size(950, 650);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
            this.BackColor = Color.White;

            // 1. Panel trái (Brand Identity - 42% width)
            leftPanel = new BrandPanel();
            leftPanel.Dock = DockStyle.Left;
            leftPanel.Width = 400;

            // Tải ảnh nền cho panel trái
            try
            {
                string[] extensions = { ".png", ".jpg", ".jpeg" };
                string[] searchDirs = { AppDomain.CurrentDomain.BaseDirectory, System.IO.Directory.GetCurrentDirectory() };
                
                bool found = false;
                foreach (var dir in searchDirs)
                {
                    foreach (var ext in extensions)
                    {
                        string path = System.IO.Path.Combine(dir, "tbu_background" + ext);
                        if (System.IO.File.Exists(path))
                        {
                            leftPanel.BackgroundImageFile = Image.FromFile(path);
                            leftPanel.Invalidate(); // Ép vẽ lại panel
                            found = true;
                            break;
                        }
                    }
                    if (found) break;
                }
            }
            catch { /* Fallback to gradient if image fails to load */ }

            // Logo ("UniPortal")
            lblLogo = new Label();
            lblLogo.Text = "🎓 TBU";
            lblLogo.ForeColor = Color.White;
            lblLogo.Font = new Font("Segoe UI", 14f, FontStyle.Bold);
            lblLogo.AutoSize = true;
            lblLogo.Location = new Point(40, 40);
            lblLogo.BackColor = Color.Transparent;

            // Main Heading
            lblHeading = new Label();
            lblHeading.Text = "Hệ Thống Quản Lý Đồ Án";
            lblHeading.ForeColor = Color.White;
            lblHeading.Font = new Font("Segoe UI", 24f, FontStyle.Bold);
            lblHeading.AutoSize = false;
            lblHeading.Size = new Size(330, 100); // Đủ rộng để xuống dòng
            lblHeading.Location = new Point(35, 220);
            lblHeading.BackColor = Color.Transparent;

            // Description
            lblDescription = new Label();
            lblDescription.Text = "Nền tảng quản lý thông minh – Đồng hành cùng Giảng viên nâng tầm chất lượng đồ án và khóa luận tốt nghiệp tại Đại học Thái Bình.";
            lblDescription.ForeColor = ColorTranslator.FromHtml("#A0AEC0");
            lblDescription.Font = new Font("Segoe UI", 10f, FontStyle.Regular);
            lblDescription.AutoSize = false;
            lblDescription.Size = new Size(320, 100); // Cho phép xuống dòng
            lblDescription.Location = new Point(40, 320);
            lblDescription.BackColor = Color.Transparent;

            // Footer
            lblFooter = new Label();
            lblFooter.Text = "© 2026 Thái Bình University  •  Support";
            lblFooter.ForeColor = ColorTranslator.FromHtml("#A0AEC0");
            lblFooter.Font = new Font("Segoe UI", 8.5f, FontStyle.Regular);
            lblFooter.AutoSize = true;
            lblFooter.Location = new Point(40, 600);
            lblFooter.BackColor = Color.Transparent;

            // Thêm các thành phần vào Panel trái
            leftPanel.Controls.Add(lblLogo);
            leftPanel.Controls.Add(lblHeading);
            leftPanel.Controls.Add(lblDescription);
            leftPanel.Controls.Add(lblFooter);

            // 2. Panel phải (Login Form Area - 58% width)
            rightPanel = new Panel();
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.BackColor = ColorTranslator.FromHtml("#F7FAFC");

            // Căn giữa Container Card
            cardContainer = new CardPanel();
            cardContainer.Size = new Size(400, 500);
            cardContainer.Location = new Point((550 - 400) / 2, (650 - 500) / 2); // 550 là chiều rộng còn lại (950-400)
            cardContainer.BackColor = Color.White;

            // Title
            lblTitle = new Label();
            lblTitle.Text = "ĐẠI HỌC THÁI BÌNH";
            lblTitle.ForeColor = ColorTranslator.FromHtml("#1A202C");
            lblTitle.Font = new Font("Segoe UI", 20f, FontStyle.Bold);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(40, 40);

            // Subtitle
            lblSubtitle = new Label();
            lblSubtitle.Text = "Đăng nhập để tiếp tục.";
            lblSubtitle.ForeColor = ColorTranslator.FromHtml("#718096");
            lblSubtitle.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            lblSubtitle.AutoSize = true;
            lblSubtitle.Location = new Point(40, 85);

            // Field 1: Email/ID Label
            lblEmail = new Label();
            lblEmail.Text = "Mã Giảng Viên";
            lblEmail.ForeColor = ColorTranslator.FromHtml("#4A5568");
            lblEmail.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(40, 150);

            // Field 1: Email/ID TextBox (Custom Rounded)
            txtEmail = new RoundedTextBox();
            txtEmail.LeftIconType = "user";
            txtEmail.PlaceholderText = "Mã Giảng Viên";
            txtEmail.Location = new Point(40, 175);
            txtEmail.Size = new Size(320, 38);

            // Field 2: Password Label
            lblPassword = new Label();
            lblPassword.Text = "Mật Khẩu";
            lblPassword.ForeColor = ColorTranslator.FromHtml("#4A5568");
            lblPassword.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(40, 230);
 
            // Field 2: Password TextBox (Custom Rounded)
            txtPassword = new RoundedTextBox();
            txtPassword.LeftIconType = "lock";
            txtPassword.PlaceholderText = "Mật Khẩu";
            txtPassword.Location = new Point(40, 255);
            txtPassword.Size = new Size(320, 38);
            txtPassword.UseSystemPasswordChar = true; // Luôn ẩn ký tự để bảo mật khi người dùng gõ
            txtPassword.ShowEyeIcon = true; // Hiện biểu tượng con mắt

            // LinkLabel: Forgot Password?
            lnkForgotPassword = new LinkLabel();
            lnkForgotPassword.Text = "Quên Mật Khẩu?";
            lnkForgotPassword.LinkColor = ColorTranslator.FromHtml("#3182CE");
            lnkForgotPassword.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
            lnkForgotPassword.AutoSize = true;
            lnkForgotPassword.Location = new Point(255, 310); // Di chuyển xuống dưới
            lnkForgotPassword.LinkBehavior = LinkBehavior.HoverUnderline;
            lnkForgotPassword.BackColor = Color.Transparent;

            // Checkbox: Remember Me
            chkRememberMe = new CheckBox();
            chkRememberMe.Text = "Ghi nhớ đăng nhập";
            chkRememberMe.ForeColor = ColorTranslator.FromHtml("#718096");
            chkRememberMe.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
            chkRememberMe.AutoSize = true;
            chkRememberMe.Location = new Point(40, 310);

            // Button: Sign In (Custom Rounded)
            btnSignIn = new RoundedButton();
            btnSignIn.Text = "Đăng Nhập";
            btnSignIn.BackColor = Color.FromArgb(37, 99, 235); // Royal Blue
            btnSignIn.ForeColor = Color.White;
            btnSignIn.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            btnSignIn.FlatStyle = FlatStyle.Flat;
            btnSignIn.FlatAppearance.BorderSize = 0;
            btnSignIn.Location = new Point(40, 355);
            btnSignIn.Size = new Size(320, 48);
            btnSignIn.Cursor = Cursors.Hand;
            btnSignIn.BorderRadius = 10; // Bo góc mượt hơn cho button
            btnSignIn.Click += BtnSignIn_Click;

            // Footer Text in Right Panel
            lblTerms = new Label();
            lblTerms.Text = "Đồng ý với điều khoản & dịch vụ";
            lblTerms.ForeColor = ColorTranslator.FromHtml("#A0AEC0");
            lblTerms.Font = new Font("Segoe UI", 8.5f, FontStyle.Regular);
            lblTerms.TextAlign = ContentAlignment.MiddleCenter;
            lblTerms.AutoSize = false;
            lblTerms.Size = new Size(320, 60); // Tăng chiều cao để wrap text dài
            lblTerms.Location = new Point(40, 415);

            // Thêm các thành phần vào Container Card
            cardContainer.Controls.Add(lblTitle);
            cardContainer.Controls.Add(lblSubtitle);
            cardContainer.Controls.Add(lblEmail);
            cardContainer.Controls.Add(txtEmail);
            cardContainer.Controls.Add(lblPassword);
            cardContainer.Controls.Add(lnkForgotPassword);
            cardContainer.Controls.Add(txtPassword);
            cardContainer.Controls.Add(chkRememberMe);
            cardContainer.Controls.Add(btnSignIn);
            cardContainer.Controls.Add(lblTerms);

            // Thêm cardContainer vào Right Panel
            rightPanel.Controls.Add(cardContainer);

            // Thêm Left Panel và Right Panel vào Form
            this.Controls.Add(rightPanel); // Dock.Fill được thêm trước
            this.Controls.Add(leftPanel);  // Dock.Left đè lên phần Fill

            // Ngăn tự động focus vào textbox để hiện placeholder ngay từ đầu
            this.ActiveControl = null;
        }

        private void BtnSignIn_Click(object? sender, EventArgs e)
        {
            string maGV = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(maGV) || maGV == "Mã Giảng Viên")
            {
                MessageBox.Show("Vui lòng nhập Mã Giảng Viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(password) || password == "Mật Khẩu")
            {
                MessageBox.Show("Vui lòng nhập Mật Khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Thực hiện kiểm tra thông tin trong CSDL
                string query = "SELECT ma_giang_vien, ten_giang_vien, ma_chuc_vu FROM giang_vien WHERE ma_giang_vien = @maGV AND mat_khau = @password";
                var parameters = new Microsoft.Data.SqlClient.SqlParameter[]
                {
                    new Microsoft.Data.SqlClient.SqlParameter("@maGV", maGV),
                    new Microsoft.Data.SqlClient.SqlParameter("@password", password)
                };

                var dt = DatabaseHelper.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    string tenGV = dt.Rows[0]["ten_giang_vien"].ToString() ?? "Giảng viên";
                    string maChucVu = dt.Rows[0]["ma_chuc_vu"].ToString() ?? "";
                    
                    // Lấy tên chức vụ từ ma_chuc_vu
                    string tenChucVu = "Giảng viên";
                    if (!string.IsNullOrEmpty(maChucVu))
                    {
                        var dtChucVu = DatabaseHelper.ExecuteQuery(
                            "SELECT ten_chuc_vu FROM chuc_vu WHERE ma_chuc_vu = @maChucVu",
                            new Microsoft.Data.SqlClient.SqlParameter[] { new Microsoft.Data.SqlClient.SqlParameter("@maChucVu", maChucVu) }
                        );
                        if (dtChucVu.Rows.Count > 0)
                        {
                            tenChucVu = dtChucVu.Rows[0]["ten_chuc_vu"].ToString() ?? "Giảng viên";
                        }
                    }

                    this.Hide();
                    using (var menuForm = new MenuForm(maGV, tenGV, tenChucVu))
                    {
                        menuForm.ShowDialog();
                    }
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Mã Giảng Viên hoặc Mật Khẩu không chính xác!", "Đăng Nhập Thất Bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu:\n" + ex.Message, "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---------------------------------------------------------
        // Lớp nội bộ để vẽ ảnh nền và lớp phủ mờ cho Panel trái
        // ---------------------------------------------------------
        private class BrandPanel : Panel
        {
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            private Image? backgroundImageFile;
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public Image? BackgroundImageFile 
            { 
                get => backgroundImageFile; 
                set { backgroundImageFile = value; this.Invalidate(); } 
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                if (BackgroundImageFile != null)
                {
                    // Vẽ ảnh bao phủ toàn bộ panel (Aspect Fill)
                    float ratio = Math.Max((float)this.Width / BackgroundImageFile.Width, (float)this.Height / BackgroundImageFile.Height);
                    int newWidth = (int)(BackgroundImageFile.Width * ratio);
                    int newHeight = (int)(BackgroundImageFile.Height * ratio);
                    int x = (this.Width - newWidth) / 2;
                    int y = (this.Height - newHeight) / 2;

                    e.Graphics.DrawImage(BackgroundImageFile, new Rectangle(x, y, newWidth, newHeight));

                    // Lớp phủ màu đen mờ (Overlay) - Alpha = 160 (khoảng 63%)
                    using (SolidBrush overlayBrush = new SolidBrush(Color.FromArgb(160, Color.Black)))
                    {
                        e.Graphics.FillRectangle(overlayBrush, this.ClientRectangle);
                    }
                }
                else
                {
                    // Fallback sang Gradient nếu không tìm thấy ảnh
                    Color startColor = ColorTranslator.FromHtml("#0F2042");
                    Color endColor = ColorTranslator.FromHtml("#0A1128");

                    using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, startColor, endColor, LinearGradientMode.ForwardDiagonal))
                    {
                        e.Graphics.FillRectangle(brush, this.ClientRectangle);
                    }
                }
            }
        }

        // ---------------------------------------------------------
        // Lớp nội bộ để vẽ Card bo góc (CardContainer)
        // ---------------------------------------------------------
        private class CardPanel : Panel
        {
            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                int radius = 12;

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
                    path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
                    path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
                    path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
                    path.CloseFigure();

                    // Thiết lập vùng bo góc
                    this.Region = new Region(path);

                    // Vẽ viền xám nhạt bao quanh card
                    using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            }
        }

        // ---------------------------------------------------------
        // Lớp nội bộ để tạo Button bo góc
        // ---------------------------------------------------------
        private class RoundedButton : Button
        {
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public int BorderRadius { get; set; } = 8;

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);

                using (GraphicsPath path = GetRoundedPath(rect, BorderRadius))
                {
                    this.Region = new Region(path);

                    // Vẽ nền
                    using (SolidBrush brush = new SolidBrush(this.BackColor))
                    {
                        e.Graphics.FillPath(brush, path);
                    }

                    // Vẽ chữ
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

        // ---------------------------------------------------------
        // Lớp nội bộ để tạo TextBox bo góc (Dùng UserControl bao quanh)
        // ---------------------------------------------------------
        private class RoundedTextBox : UserControl
        {
            private TextBox textBox;
            private Panel lblEye;
            private Label lblPlaceholder;
            private bool isPasswordHidden = true;
            private string leftIconType = "";
            private string placeholderText = "";

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public int BorderRadius { get; set; } = 8;
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public Color BorderColor { get; set; } = ColorTranslator.FromHtml("#E2E8F0");
            
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public override string? Text 
            { 
                get => textBox.Text; 
                set 
                { 
                    textBox.Text = value ?? ""; 
                    UpdatePlaceholderVisibility(); 
                } 
            }
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public override Font? Font { get => base.Font; set { base.Font = value; textBox.Font = value; } }
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public override Color ForeColor { get => base.ForeColor; set { base.ForeColor = value; textBox.ForeColor = value; } }
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public bool UseSystemPasswordChar { get => textBox.UseSystemPasswordChar; set => textBox.UseSystemPasswordChar = value; }

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public string LeftIconType
            {
                get => leftIconType;
                set
                {
                    leftIconType = value;
                    UpdateLayout();
                    this.Invalidate();
                }
            }

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public string PlaceholderText
            {
                get => placeholderText;
                set
                {
                    placeholderText = value;
                    lblPlaceholder.Text = value;
                    UpdatePlaceholderVisibility();
                }
            }

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public bool ShowEyeIcon
            {
                get => lblEye.Visible;
                set
                {
                    lblEye.Visible = value;
                    UpdateLayout();
                }
            }

            public RoundedTextBox()
            {
                textBox = new TextBox();
                textBox.BorderStyle = BorderStyle.None;
                textBox.Location = new Point(12, 10);
                textBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                this.BackColor = Color.White;
                this.Padding = new Padding(12, 10, 12, 10);
                this.Size = new Size(320, 38);
                textBox.Width = this.Width - 24; // Đảm bảo TextBox đủ rộng ngay từ đầu
                this.Controls.Add(textBox);

                // Label hiển thị chữ gợi ý (Placeholder)
                lblPlaceholder = new Label();
                lblPlaceholder.ForeColor = Color.Gray;
                lblPlaceholder.Font = new Font("Segoe UI", 10f, FontStyle.Italic);
                lblPlaceholder.BackColor = Color.Transparent;
                lblPlaceholder.AutoSize = true;
                lblPlaceholder.Cursor = Cursors.IBeam;
                lblPlaceholder.Location = new Point(12, 10);
                lblPlaceholder.Click += (s, e) => textBox.Focus();
                this.Controls.Add(lblPlaceholder);
                lblPlaceholder.BringToFront();

                // Icon con mắt (Vẽ bằng GDI+ cho sắc nét và hiện đại)
                lblEye = new Panel();
                lblEye.Size = new Size(25, 25);
                lblEye.Cursor = Cursors.Hand;
                lblEye.Visible = false;
                lblEye.Location = new Point(this.Width - 38, 7);
                lblEye.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                lblEye.BackColor = Color.Transparent;
                
                lblEye.Paint += (s, e) => {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    Color iconColor = ColorTranslator.FromHtml("#A0AEC0");
                    using (Pen pen = new Pen(iconColor, 1.5f))
                    {
                        // Vẽ khung mắt (hình quả trám mượt)
                        Rectangle eyeRect = new Rectangle(3, 7, 19, 10);
                        e.Graphics.DrawArc(pen, eyeRect.X, eyeRect.Y - 2, eyeRect.Width, eyeRect.Height + 4, 0, -180);
                        e.Graphics.DrawArc(pen, eyeRect.X, eyeRect.Y - 2, eyeRect.Width, eyeRect.Height + 4, 0, 180);
                        
                        // Vẽ lòng đen
                        e.Graphics.DrawEllipse(pen, 9, 8, 6, 6);
                        using (SolidBrush brush = new SolidBrush(iconColor))
                        {
                            e.Graphics.FillEllipse(brush, 11, 10, 2, 2);
                        }
                    }
                };

                lblEye.Click += (s, e) => {
                    isPasswordHidden = !isPasswordHidden;
                    textBox.UseSystemPasswordChar = isPasswordHidden;
                };
                this.Controls.Add(lblEye);
                lblEye.BringToFront();

                // Đồng bộ sự kiện focus để vẽ lại viền (nếu muốn đổi màu khi focus)
                textBox.GotFocus += (s, e) => { 
                    BorderColor = ColorTranslator.FromHtml("#3182CE"); 
                    this.Invalidate(); 
                    UpdatePlaceholderVisibility();
                };
                textBox.LostFocus += (s, e) => { 
                    BorderColor = ColorTranslator.FromHtml("#E2E8F0"); 
                    this.Invalidate(); 
                    UpdatePlaceholderVisibility();
                };
                textBox.TextChanged += (s, e) => {
                    UpdatePlaceholderVisibility();
                };
                
                UpdateLayout();
            }

            private void UpdatePlaceholderVisibility()
            {
                if (lblPlaceholder == null || textBox == null) return;
                lblPlaceholder.Visible = string.IsNullOrEmpty(placeholderText) ? false : (string.IsNullOrEmpty(textBox.Text) && !textBox.Focused);
            }

            private void UpdateLayout()
            {
                if (textBox == null) return;
                
                int leftPadding = 12;
                if (!string.IsNullOrEmpty(leftIconType))
                {
                    leftPadding = 38; // Dành khoảng trống bên trái cho icon
                }

                int rightPadding = 12;
                if (lblEye != null && lblEye.Visible)
                {
                    rightPadding = 38; // Dành khoảng trống bên phải cho eye icon
                }

                textBox.Location = new Point(leftPadding, 10);
                textBox.Width = this.Width - leftPadding - rightPadding;

                if (lblPlaceholder != null)
                {
                    lblPlaceholder.Location = new Point(leftPadding, 10);
                }
            }

            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);
                UpdateLayout();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                using (GraphicsPath path = GetRoundedPath(rect, BorderRadius))
                {
                    // Vẽ nền trắng bên trong bo góc
                    using (SolidBrush brush = new SolidBrush(Color.White))
                    {
                        e.Graphics.FillPath(brush, path);
                    }

                    // Vẽ viền
                    using (Pen pen = new Pen(BorderColor, 1.5f))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }

                // Vẽ Left Icon nếu có
                if (!string.IsNullOrEmpty(leftIconType))
                {
                    bool isFocused = textBox.Focused;
                    Color iconColor = isFocused ? ColorTranslator.FromHtml("#3182CE") : ColorTranslator.FromHtml("#94A3B8");
                    Color fillTransColor = isFocused ? Color.FromArgb(40, ColorTranslator.FromHtml("#3182CE")) : Color.FromArgb(15, ColorTranslator.FromHtml("#94A3B8"));

                    using (Pen pen = new Pen(iconColor, 1.8f))
                    using (SolidBrush fillBrush = new SolidBrush(fillTransColor))
                    {
                        if (leftIconType.ToLower() == "user" || leftIconType.ToLower() == "person")
                        {
                            // Đầu người (tròn đầy đặn, có tô nhẹ bên trong)
                            e.Graphics.FillEllipse(fillBrush, 19, 11, 6, 6);
                            e.Graphics.DrawEllipse(pen, 19, 11, 6, 6);
                            
                            // Vai người (dáng bầu mềm mại, có tô nhẹ bên trong)
                            using (GraphicsPath shoulderPath = new GraphicsPath())
                            {
                                shoulderPath.AddArc(15, 18, 14, 10, 180, 180);
                                shoulderPath.AddLine(29, 23, 29, 26);
                                shoulderPath.AddLine(29, 26, 15, 26);
                                shoulderPath.CloseFigure();
                                
                                e.Graphics.FillPath(fillBrush, shoulderPath);
                                e.Graphics.DrawPath(pen, shoulderPath);
                            }
                        }
                        else if (leftIconType.ToLower() == "lock")
                        {
                            // Quai khóa
                            e.Graphics.DrawArc(pen, 17, 11, 10, 10, 180, 180);
                            e.Graphics.DrawLine(pen, 17, 16, 17, 19);
                            e.Graphics.DrawLine(pen, 27, 16, 27, 19);
                            
                            // Thân khóa bo tròn các góc
                            using (GraphicsPath lockBodyPath = new GraphicsPath())
                            {
                                Rectangle bodyRect = new Rectangle(14, 19, 16, 11);
                                int r = 3; // bo tròn góc
                                lockBodyPath.AddArc(bodyRect.X, bodyRect.Y, r * 2, r * 2, 180, 90);
                                lockBodyPath.AddArc(bodyRect.Right - r * 2, bodyRect.Y, r * 2, r * 2, 270, 90);
                                lockBodyPath.AddArc(bodyRect.Right - r * 2, bodyRect.Bottom - r * 2, r * 2, r * 2, 0, 90);
                                lockBodyPath.AddArc(bodyRect.X, bodyRect.Bottom - r * 2, r * 2, r * 2, 90, 90);
                                lockBodyPath.CloseFigure();
                                
                                e.Graphics.FillPath(fillBrush, lockBodyPath);
                                e.Graphics.DrawPath(pen, lockBodyPath);
                            }
                            
                            // Lỗ khóa thiết kế nghệ thuật
                            using (SolidBrush dotBrush = new SolidBrush(iconColor))
                            {
                                e.Graphics.FillEllipse(dotBrush, 21, 22, 2, 2);
                                e.Graphics.DrawLine(pen, 22, 23, 22, 26);
                            }
                        }
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

            // Chuyển tiếp các sự kiện quan trọng
            public new event EventHandler GotFocus { add => textBox.GotFocus += value; remove => textBox.GotFocus -= value; }
            public new event EventHandler LostFocus { add => textBox.LostFocus += value; remove => textBox.LostFocus -= value; }
        }
    }
}
