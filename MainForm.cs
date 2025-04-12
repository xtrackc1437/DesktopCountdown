using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DesktopCountdown
{
    public partial class MainForm : Form
    {
        private NotifyIcon trayIcon;
        private System.Windows.Forms.Timer countdownTimer;
        private string eventName;
        private DateTime targetTime;
        private string windowContent;
        private Size windowSize;
        private int fontSize;
        private Color textColor;
        private Label countdownLabel;

        // 配置文件中设置的窗口位置
        private Point windowPosition;

        public MainForm()
        {
            InitializeComponent();
            InitializeTrayIcon();
            LoadConfiguration();
            InitializeCountdownTimer();
            InitializeCountdownLabel();
            this.ShowInTaskbar = false; // 不在任务栏显示窗口
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Text = "倒计时应用"; // 设置窗体标题
            this.Size = windowSize; // 设置窗体大小
            this.BackColor = Color.Magenta; // 设置背景颜色
            this.TransparencyKey = Color.Magenta; // 设置透明色
            this.TopMost = true; // 置顶显示
            this.Location = windowPosition; // 设置窗口位置
            UpdateWindow(); // 更新倒计时内容
        }

        private void LoadConfiguration()
        {
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "countdown.conf");
            if (!File.Exists(configFilePath))
            {
                MessageBox.Show("配置文件未找到！");
                Application.Exit();
                return;
            }

            string[] configLines = File.ReadAllLines(configFilePath);
            foreach (string line in configLines)
            {
                string[] parts = line.Split('=');
                if (parts.Length != 2) continue;

                string key = parts[0].Trim();
                string value = parts[1].Trim();

                switch (key)
                {
                    case "EventName":
                        eventName = value;
                        break;
                    case "TargetTime":
                        targetTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(value)).DateTime;
                        break;
                    case "WindowSize":
                        windowSize = GetWindowSizeFromConfig(value);
                        break;
                    case "FontSize":
                        fontSize = int.Parse(value);
                        break;
                    case "TextColor":
                        textColor = ColorTranslator.FromHtml(value);
                        break;
                    case "WindowContent":
                        windowContent = value;
                        break;
                    case "WindowPosition":
                        string[] positionParts = value.Split(',');
                        if (positionParts.Length == 2)
                        {
                            windowPosition = new Point(int.Parse(positionParts[0]), int.Parse(positionParts[1]));
                        }
                        break;
                }
            }
        }

        private Size GetWindowSizeFromConfig(string size)
        {
            switch (size.ToLower())
            {
                case "small":
                    return new Size(200, 100);
                case "medium":
                    return new Size(300, 150);
                case "large":
                    return new Size(400, 200);
                default:
                    return new Size(300, 150);
            }
        }

        private void InitializeCountdownTimer()
        {
            countdownTimer = new System.Windows.Forms.Timer();
            countdownTimer.Interval = 1000; // 每秒更新一次
            countdownTimer.Tick += OnCountdownTick;
            countdownTimer.Start();
        }

        private void OnCountdownTick(object sender, EventArgs e)
        {
            UpdateWindow();
        }

        private void UpdateWindow()
        {
            TimeSpan remainingTime = targetTime - DateTime.Now;
            string content = windowContent
                .Replace("{EventName}", eventName)
                .Replace("{Days}", remainingTime.Days.ToString())
                .Replace("{Hours}", remainingTime.Hours.ToString())
                .Replace("{Mins}", remainingTime.Minutes.ToString())
                .Replace("{Seconds}", remainingTime.Seconds.ToString());

            countdownLabel.Text = content;
            countdownLabel.Font = new Font("Arial", fontSize);
            countdownLabel.ForeColor = textColor;
        }

        private void InitializeCountdownLabel()
        {
            countdownLabel = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                BackColor = Color.Transparent
            };
            this.Controls.Add(countdownLabel);
        }

        private void InitializeTrayIcon()
        {
            trayIcon = new NotifyIcon
            {
                Icon = new Icon("app.ico"),
                ContextMenuStrip = new ContextMenuStrip()
            };

            trayIcon.ContextMenuStrip.Items.Add("显示", null, ShowWindow);
            trayIcon.ContextMenuStrip.Items.Add("隐藏", null, HideWindow);
            trayIcon.ContextMenuStrip.Items.Add("窗口位置 - 左上角", null, MoveWindowToTopLeft);
            trayIcon.ContextMenuStrip.Items.Add("窗口位置 - 右上角", null, MoveWindowToTopRight);
            trayIcon.ContextMenuStrip.Items.Add("窗口位置 - 左下角", null, MoveWindowToBottomLeft);
            trayIcon.ContextMenuStrip.Items.Add("窗口位置 - 右下角", null, MoveWindowToBottomRight);
            trayIcon.ContextMenuStrip.Items.Add("打开配置文件", null, OpenConfigFile);
            trayIcon.ContextMenuStrip.Items.Add("重载配置", null, ReloadConfig);
            trayIcon.ContextMenuStrip.Items.Add("退出", null, ExitApp);

            trayIcon.Visible = true;
        }

        private void ShowWindow(object sender, EventArgs e)
        {
            this.Show();
        }

        private void HideWindow(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void MoveWindowToTopLeft(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);
        }

        private void MoveWindowToTopRight(object sender, EventArgs e)
        {
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width, 0);
        }

        private void MoveWindowToBottomLeft(object sender, EventArgs e)
        {
            this.Location = new Point(0, Screen.PrimaryScreen.WorkingArea.Height - this.Height);
        }

        private void MoveWindowToBottomRight(object sender, EventArgs e)
        {
            this.Location = new Point(
                Screen.PrimaryScreen.WorkingArea.Width - this.Width,
                Screen.PrimaryScreen.WorkingArea.Height - this.Height
            );
        }

        private void OpenConfigFile(object sender, EventArgs e)
        {
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "countdown.conf");
            System.Diagnostics.Process.Start("notepad.exe", configFilePath);
        }

        private void ReloadConfig(object sender, EventArgs e)
        {
            LoadConfiguration();
            UpdateWindow();
        }

        private void ExitApp(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
