using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DesktopCountdown.Properties;
using System.IO;

namespace DesktopCountdown
{
    /// <summary>
    /// 托盘图标管理类，负责创建和管理系统托盘图标及菜单
    /// </summary>
    public class TrayIconManager : IDisposable
    {
        private readonly NotifyIcon trayIcon;
        private readonly Form mainForm;
        private readonly ConfigurationManager configManager;
        private readonly CountdownManager countdownManager;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="form">主窗体</param>
        /// <param name="configManager">配置管理器</param>
        /// <param name="countdownManager">倒计时管理器</param>
        public TrayIconManager(Form form, ConfigurationManager configManager, CountdownManager countdownManager)
        {
            this.mainForm = form;
            this.configManager = configManager;
            this.countdownManager = countdownManager;
            this.trayIcon = new NotifyIcon();
        }

        /// <summary>
        /// 初始化托盘图标
        /// </summary>
        public void InitializeTrayIcon()
        {
            try
            {
                // 使用Resources.AppIcon获取嵌入的图标
                trayIcon.Icon = Resources.AppIcon;
                trayIcon.ContextMenuStrip = CreateContextMenu();
                trayIcon.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"初始化托盘图标时出错: {ex.Message}");
                // 使用默认图标作为最终后备
                trayIcon.Icon = SystemIcons.Information;
                trayIcon.ContextMenuStrip = CreateContextMenu();
                trayIcon.Visible = true;
            }
        }

        /// <summary>
        /// 创建上下文菜单
        /// </summary>
        /// <returns>上下文菜单</returns>
        private ContextMenuStrip CreateContextMenu()
        {  
            ContextMenuStrip menuStrip = new ContextMenuStrip();

            menuStrip.Items.Add("显示", null, ShowWindow);
            menuStrip.Items.Add("隐藏", null, HideWindow);
            menuStrip.Items.Add(new ToolStripSeparator());
            menuStrip.Items.Add("窗口位置 - 左上角", null, MoveWindowToTopLeft);
            menuStrip.Items.Add("窗口位置 - 右上角", null, MoveWindowToTopRight);
            menuStrip.Items.Add("窗口位置 - 左下角", null, MoveWindowToBottomLeft);
            menuStrip.Items.Add("窗口位置 - 右下角", null, MoveWindowToBottomRight);
            menuStrip.Items.Add(new ToolStripSeparator());
            menuStrip.Items.Add("打开配置文件", null, OpenConfigFile);
            menuStrip.Items.Add("重载配置", null, ReloadConfig);
            menuStrip.Items.Add(new ToolStripSeparator());
            menuStrip.Items.Add("退出", null, ExitApp);

            return menuStrip;
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        private void ShowWindow(object sender, EventArgs e)
        {
            mainForm.Show();
        }

        /// <summary>
        /// 隐藏窗口
        /// </summary>
        private void HideWindow(object sender, EventArgs e)
        {
            mainForm.Hide();
        }

        /// <summary>
        /// 移动窗口到左上角
        /// </summary>
        private void MoveWindowToTopLeft(object sender, EventArgs e)
        {
            mainForm.Location = new Point(0, 0);
        }

        /// <summary>
        /// 移动窗口到右上角
        /// </summary>
        private void MoveWindowToTopRight(object sender, EventArgs e)
        {
            mainForm.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - mainForm.Width, 0);
        }

        /// <summary>
        /// 移动窗口到左下角
        /// </summary>
        private void MoveWindowToBottomLeft(object sender, EventArgs e)
        {
            mainForm.Location = new Point(0, Screen.PrimaryScreen.WorkingArea.Height - mainForm.Height);
        }

        /// <summary>
        /// 移动窗口到右下角
        /// </summary>
        private void MoveWindowToBottomRight(object sender, EventArgs e)
        {
            mainForm.Location = new Point(
                Screen.PrimaryScreen.WorkingArea.Width - mainForm.Width,
                Screen.PrimaryScreen.WorkingArea.Height - mainForm.Height
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

        #region IDisposable 实现
        
        private bool disposedValue = false; // 用于检测冗余调用

        /// <summary>
        /// 清理托盘图标资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源的核心方法
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // 释放托管资源
                    trayIcon.Visible = false;
                    trayIcon.Dispose();
                }

                disposedValue = true;
            }
        }
        
        #endregion
    }
}