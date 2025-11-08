using System;
using System.Drawing;
using System.Windows.Forms;

namespace DesktopCountdown
{
    public partial class MainForm : Form
    {
        // 使用独立的管理器类
        private ConfigurationManager configManager;
        private CountdownManager countdownManager;
        private TrayIconManager trayIconManager;

        public MainForm()
        {
            InitializeComponent();
            InitializeManagers();
            this.ShowInTaskbar = false; // 不在任务栏显示窗口
        }

        private void InitializeManagers()
        {
            // 初始化配置管理器
            configManager = new ConfigurationManager();
            if (!configManager.LoadConfiguration())
            {
                Application.Exit();
                return;
            }

            // 初始化倒计时管理器
            countdownManager = new CountdownManager(this);
            countdownManager.UpdateConfiguration(configManager);
            countdownManager.InitializeCountdownTimer();

            // 初始化托盘图标管理器
            trayIconManager = new TrayIconManager(this, configManager, countdownManager);
            trayIconManager.InitializeTrayIcon();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Text = "倒计时应用"; // 设置窗体标题
            this.Size = configManager.WindowSize; // 设置窗体大小
            this.BackColor = Color.Magenta; // 设置背景颜色
            this.TransparencyKey = Color.Magenta; // 设置透明色
            this.TopMost = true; // 置顶显示
            this.Location = configManager.WindowPosition; // 设置窗口位置
            countdownManager.UpdateWindow(); // 更新倒计时内容
        }

        // Dispose方法由设计器自动生成在Designer.cs文件中
        // 如需自定义资源清理，请在设计器生成的Dispose方法中添加
    }
}
