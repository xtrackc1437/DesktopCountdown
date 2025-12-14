using System;
using System.Drawing;
using System.Windows.Forms;

namespace DesktopCountdown
{
    public partial class MainForm : Form
    {
        // 使用独立的管理器类，改为protected以便在Dispose方法中访问
        protected ConfigurationManager configManager;
        protected CountdownManager countdownManager;
        protected TrayIconManager trayIconManager;
        protected ContextMenuStrip floatingWindowContextMenu;

        public MainForm()
        {
            InitializeComponent();
            InitializeManagers();
            InitializeFloatingWindowContextMenu();
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
            // 添加空值检查，避免配置加载失败时出现NullReferenceException
            if (configManager == null || countdownManager == null)
            {
                Application.Exit();
                return;
            }
            
            this.Text = "倒计时应用"; // 设置窗体标题
            this.Size = configManager.WindowSize; // 设置窗体大小
            this.BackColor = Color.Magenta; // 设置背景颜色
            this.TransparencyKey = Color.Magenta; // 设置透明色
            this.TopMost = true; // 置顶显示
            this.Location = configManager.WindowPosition; // 设置窗口位置
            countdownManager.UpdateWindow(); // 更新倒计时内容
        }

        /// <summary>
        /// 初始化悬浮窗右键菜单
        /// </summary>
        private void InitializeFloatingWindowContextMenu()
        {
            floatingWindowContextMenu = new ContextMenuStrip();

            // 第一部分：隐藏按钮
            floatingWindowContextMenu.Items.Add("隐藏", null, HideWindow);
            floatingWindowContextMenu.Items.Add(new ToolStripSeparator());

            // 第二部分：窗口位置主选项（带子菜单）
            ToolStripMenuItem windowPositionMenuItem = new ToolStripMenuItem("窗口位置");
            
            // 创建子菜单项
            windowPositionMenuItem.DropDownItems.Add("左上角", null, MoveWindowToTopLeft);
            windowPositionMenuItem.DropDownItems.Add("右上角", null, MoveWindowToTopRight);
            windowPositionMenuItem.DropDownItems.Add("左下角", null, MoveWindowToBottomLeft);
            windowPositionMenuItem.DropDownItems.Add("右下角", null, MoveWindowToBottomRight);
            
            floatingWindowContextMenu.Items.Add(windowPositionMenuItem);
            floatingWindowContextMenu.Items.Add(new ToolStripSeparator());

            // 第三部分：配置相关选项
            floatingWindowContextMenu.Items.Add("重载配置文件", null, ReloadConfig);
            floatingWindowContextMenu.Items.Add("打开配置文件", null, OpenConfigFile);
            floatingWindowContextMenu.Items.Add(new ToolStripSeparator());

            // 第四部分：退出选项
            floatingWindowContextMenu.Items.Add("退出", null, ExitApp);

            // 关联右键菜单到窗体
            this.ContextMenuStrip = floatingWindowContextMenu;
            
            // 应用系统主题
            ThemeManager.ApplyTheme(floatingWindowContextMenu);
        }

        /// <summary>
        /// 隐藏窗口
        /// </summary>
        private void HideWindow(object sender, EventArgs e)
        {
            this.Hide();
        }

        /// <summary>
        /// 移动窗口到左上角
        /// </summary>
        private void MoveWindowToTopLeft(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);
        }

        /// <summary>
        /// 移动窗口到右上角
        /// </summary>
        private void MoveWindowToTopRight(object sender, EventArgs e)
        {
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width, 0);
        }

        /// <summary>
        /// 移动窗口到左下角
        /// </summary>
        private void MoveWindowToBottomLeft(object sender, EventArgs e)
        {
            this.Location = new Point(0, Screen.PrimaryScreen.WorkingArea.Height - this.Height);
        }

        /// <summary>
        /// 移动窗口到右下角
        /// </summary>
        private void MoveWindowToBottomRight(object sender, EventArgs e)
        {
            this.Location = new Point(
                Screen.PrimaryScreen.WorkingArea.Width - this.Width,
                Screen.PrimaryScreen.WorkingArea.Height - this.Height
            );
        }

        /// <summary>
        /// 打开配置文件
        /// </summary>
        private void OpenConfigFile(object sender, EventArgs e)
        {
            configManager.OpenConfigFile();
        }

        /// <summary>
        /// 重载配置
        /// </summary>
        private void ReloadConfig(object sender, EventArgs e)
        {
            if (configManager.LoadConfiguration())
            {
                countdownManager.UpdateConfiguration(configManager);
            }
        }

        /// <summary>
        /// 退出应用
        /// </summary>
        private void ExitApp(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Dispose方法由设计器自动生成在Designer.cs文件中
        // 如需自定义资源清理，请在设计器生成的Dispose方法中添加
    }
}
