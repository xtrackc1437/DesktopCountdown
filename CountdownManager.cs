using System;
using System.Drawing;
using System.Windows.Forms;

namespace DesktopCountdown
{
    /// <summary>
    /// 倒计时管理器，负责处理倒计时逻辑和窗口更新
    /// </summary>
    public class CountdownManager
    {
        private readonly System.Windows.Forms.Timer countdownTimer;
        private readonly Label countdownLabel;
        private readonly Form mainForm;

        // 倒计时相关属性
        private string eventName;
        private DateTime targetTime;
        private string windowContent;
        private int fontSize;
        private Color textColor;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="form">主窗体</param>
        public CountdownManager(Form form)
        {
            this.mainForm = form;
            this.countdownLabel = CreateCountdownLabel();
            this.countdownTimer = new System.Windows.Forms.Timer();
        }

        /// <summary>
        /// 初始化倒计时标签
        /// </summary>
        /// <returns>倒计时标签控件</returns>
        private Label CreateCountdownLabel()
        {
            Label label = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                BackColor = Color.Transparent
            };
            mainForm.Controls.Add(label);
            return label;
        }

        /// <summary>
        /// 初始化倒计时计时器
        /// </summary>
        public void InitializeCountdownTimer()
        {
            countdownTimer.Interval = 1000; // 每秒更新一次
            countdownTimer.Tick += OnCountdownTick;
            countdownTimer.Start();
        }

        /// <summary>
        /// 更新配置信息
        /// </summary>
        /// <param name="configManager">配置管理器</param>
        public void UpdateConfiguration(ConfigurationManager configManager)
        {
            this.eventName = configManager.EventName;
            this.targetTime = configManager.TargetTime;
            this.windowContent = configManager.WindowContent;
            this.fontSize = configManager.FontSize;
            this.textColor = configManager.TextColor;
            
            // 更新主窗体属性
            mainForm.Size = configManager.WindowSize;
            mainForm.Location = configManager.WindowPosition;
            
            // 更新倒计时显示
            UpdateWindow();
        }

        /// <summary>
        /// 计时器触发事件处理函数
        /// </summary>
        private void OnCountdownTick(object sender, EventArgs e)
        {
            UpdateWindow();
        }

        /// <summary>
        /// 更新窗口显示
        /// </summary>
        public void UpdateWindow()
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

        /// <summary>
        /// 清理计时器资源
        /// </summary>
        public void Dispose()
        {
            countdownTimer.Stop();
            countdownTimer.Dispose();
        }
    }
}