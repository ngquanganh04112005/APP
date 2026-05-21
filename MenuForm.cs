using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

namespace TBUProject
{
    public class MenuForm : Form
    {
        private Panel pnlMainArea;
        private System.Collections.Generic.List<Panel> sidebarButtons = new System.Collections.Generic.List<Panel>();
        private string[] menuItems = { 
            "🏠 Trang Chủ", 
            "👨‍🎓 Sinh viên", 
            "👨‍🏫 Giảng viên", 
            "📁 Đồ án", 
            "📢 Thông báo"
        };

        private string loggedInMaGV;
        private string loggedInTenGV;
        private string loggedInChucVu;

        public MenuForm(string maGV = "GV001", string tenGV = "Nguyễn Văn A", string chucVu = "Giảng viên")
        {
            this.loggedInMaGV = maGV;
            this.loggedInTenGV = tenGV;
            this.loggedInChucVu = chucVu;

            this.Text = "Hệ thống Quản lý Đồ án - TBU";
            this.ClientSize = new Size(1280, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
            this.BackColor = ColorTranslator.FromHtml("#F7FAFC");
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            InitializeSidebar();
            InitializeMainArea();
            LoadPage(0);

            // Ngăn chặn tự động chọn (select) text ở thanh tìm kiếm khi mới mở
            this.Shown += (s, e) => this.ActiveControl = null;
        }

        private void InitializeSidebar()
        {
            Panel pnlSidebar = new Panel();
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Width = 260;
            pnlSidebar.BackColor = ColorTranslator.FromHtml("#0F2042");
            this.Controls.Add(pnlSidebar);

            // Logo
            Label lblLogo = new Label();
            lblLogo.Text = "🎓 TBU";
            lblLogo.ForeColor = Color.White;
            lblLogo.Font = new Font("Segoe UI", 13f, FontStyle.Bold);
            lblLogo.AutoSize = true;
            lblLogo.Location = new Point(20, 30);
            pnlSidebar.Controls.Add(lblLogo);

            int y = 90;
            for(int i = 0; i < menuItems.Length; i++)
            {
                int index = i; // Tạo bản sao cho closure
                bool isActive = (i == 0);
                Panel btn = CreateSidebarButton(menuItems[i], isActive);
                btn.Location = new Point(15, y);
                
                // Thêm sự kiện Click cho nút và label bên trong
                EventHandler clickHandler = (s, e) => LoadPage(index);
                btn.Click += clickHandler;
                foreach(Control c in btn.Controls) c.Click += clickHandler;

                sidebarButtons.Add(btn);
                pnlSidebar.Controls.Add(btn);
                y += 50;
            }

            // Profile Widget
            Panel pnlProfile = new Panel();
            pnlProfile.Size = new Size(230, 60);
            pnlProfile.Location = new Point(15, 710);
            pnlProfile.BackColor = ColorTranslator.FromHtml("#1A2B50");
            
            // Avatar (using a label with circle border)
            Label lblAvatar = new Label();
            lblAvatar.Text = "👨‍🏫";
            lblAvatar.Font = new Font("Segoe UI", 20f);
            lblAvatar.Size = new Size(40, 40);
            lblAvatar.Location = new Point(10, 10);
            
            Label lblName = new Label();
            lblName.Text = loggedInTenGV;
            lblName.ForeColor = Color.White;
            lblName.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            lblName.Location = new Point(60, 12);
            lblName.AutoSize = true;

            Label lblRole = new Label();
            lblRole.Text = loggedInChucVu;
            lblRole.ForeColor = ColorTranslator.FromHtml("#A0AEC0");
            lblRole.Font = new Font("Segoe UI", 8.5f, FontStyle.Regular);
            lblRole.Location = new Point(60, 32);
            lblRole.AutoSize = true;

            pnlProfile.Controls.Add(lblAvatar);
            pnlProfile.Controls.Add(lblName);
            pnlProfile.Controls.Add(lblRole);
            
            pnlSidebar.Controls.Add(pnlProfile);
        }

        private Panel CreateSidebarButton(string text, bool isActive)
        {
            Panel pnl = new Panel();
            pnl.Size = new Size(230, 40);
            pnl.Cursor = Cursors.Hand;
            
            // Bo góc 50% hai đầu (Pill shape) cho thanh highlight
            GraphicsPath path = new GraphicsPath();
            int r = pnl.Height / 2; // Bán kính = 50% chiều cao (20px)
            path.AddArc(0, 0, r * 2, r * 2, 180, 90);
            path.AddArc(pnl.Width - r * 2, 0, r * 2, r * 2, 270, 90);
            path.AddArc(pnl.Width - r * 2, pnl.Height - r * 2, r * 2, r * 2, 0, 90);
            path.AddArc(0, pnl.Height - r * 2, r * 2, r * 2, 90, 90);
            path.CloseFigure();
            pnl.Region = new Region(path);
            
            if (isActive) {
                pnl.BackColor = Color.FromArgb(40, 255, 255, 255); // Nền sáng mờ khi Active
            }

            Label lbl = new Label();
            lbl.Text = text;
            lbl.ForeColor = isActive ? Color.White : ColorTranslator.FromHtml("#A0AEC0");
            lbl.Font = new Font("Segoe UI", 10f, isActive ? FontStyle.Bold : FontStyle.Regular);
            lbl.AutoSize = true;
            lbl.Location = new Point(25, 10); // Dịch sang phải một chút để cân đối với phần bo cong
            lbl.BackColor = Color.Transparent;
            
            pnl.Controls.Add(lbl);
            return pnl;
        }

        private void InitializeMainArea()
        {
            pnlMainArea = new Panel();
            pnlMainArea.Dock = DockStyle.Fill;
            pnlMainArea.BackColor = ColorTranslator.FromHtml("#F7FAFC");
            pnlMainArea.AutoScroll = true; // Cho phép cuộn
            this.Controls.Add(pnlMainArea);
            pnlMainArea.BringToFront(); // Để không bị đè bởi sidebar
        }

        private void LoadPage(int index)
        {
            // Cập nhật trạng thái màu sắc của các nút trên sidebar
            for (int i = 0; i < sidebarButtons.Count; i++)
            {
                Panel btn = sidebarButtons[i];
                bool isActive = (i == index);
                btn.BackColor = isActive ? Color.FromArgb(40, 255, 255, 255) : Color.Transparent;
                
                foreach (Control c in btn.Controls)
                {
                    if (c is Label lbl)
                    {
                        lbl.ForeColor = isActive ? Color.White : ColorTranslator.FromHtml("#A0AEC0");
                        lbl.Font = new Font("Segoe UI", 10f, isActive ? FontStyle.Bold : FontStyle.Regular);
                    }
                }
            }

            // Xóa nội dung cũ
            pnlMainArea.Controls.Clear();

            // Tải trang tương ứng
            if (index == 0)
            {
                DashboardOverviewControl overviewControl = new DashboardOverviewControl();
                overviewControl.OnNavigateRequested = (targetIndex) => LoadPage(targetIndex);
                pnlMainArea.Controls.Add(overviewControl);
            }
            else if (index == 1)
            {
                DuyetDeTaiControl thesisControl = new DuyetDeTaiControl();
                pnlMainArea.Controls.Add(thesisControl);
            }
            else if (index == 2)
            {
                GiangVienControl advisorControl = new GiangVienControl();
                pnlMainArea.Controls.Add(advisorControl);
            }
            else if (index == 3)
            {
                QuanLyDoAnControl projectControl = new QuanLyDoAnControl();
                pnlMainArea.Controls.Add(projectControl);
            }
            else
            {
                LoadPlaceholderPage(menuItems[index]);
            }
        }

        private void LoadPlaceholderPage(string title)
        {
            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 24f, FontStyle.Bold);
            lblTitle.ForeColor = ColorTranslator.FromHtml("#A0AEC0");
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(50, 50);
            
            Label lblDesc = new Label();
            lblDesc.Text = "Tính năng này đang được phát triển...\nVui lòng quay lại sau!";
            lblDesc.Font = new Font("Segoe UI", 14f);
            lblDesc.ForeColor = ColorTranslator.FromHtml("#CBD5E0");
            lblDesc.AutoSize = true;
            lblDesc.Location = new Point(50, 100);

            pnlMainArea.Controls.Add(lblTitle);
            pnlMainArea.Controls.Add(lblDesc);
        }
    }
}
