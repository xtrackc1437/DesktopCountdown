using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DesktopCountdown
{
    /// <summary>
    /// 配置管理类，负责加载和解析配置文件
    /// </summary>
    public class ConfigurationManager
    {
        // 配置文件路径
        private readonly string configFilePath;

        // 配置属性
        public string EventName { get; private set; }
        public DateTime TargetTime { get; private set; }
        public string WindowContent { get; private set; }
        public Size WindowSize { get; private set; }
        public int FontSize { get; private set; }
        public Color TextColor { get; private set; }
        public Point WindowPosition { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConfigurationManager()
        {
            configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "countdown.conf");
            // 设置默认值
            WindowPosition = new Point(100, 100);
            WindowSize = new Size(300, 150);
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <returns>是否成功加载配置</returns>
        public bool LoadConfiguration()
        {
            if (!File.Exists(configFilePath))
            {
                MessageBox.Show("配置文件未找到！");
                return false;
            }

            try
            {
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
                            EventName = value;
                            break;
                        case "TargetTime":
                            TargetTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(value)).DateTime;
                            break;
                        case "WindowSize":
                            WindowSize = GetWindowSizeFromConfig(value);
                            break;
                        case "FontSize":
                            FontSize = int.Parse(value);
                            break;
                        case "TextColor":
                            TextColor = ColorTranslator.FromHtml(value);
                            break;
                        case "WindowContent":
                            WindowContent = value;
                            break;
                        case "WindowPosition":
                            string[] positionParts = value.Split(',');
                            if (positionParts.Length == 2)
                            {
                                WindowPosition = new Point(int.Parse(positionParts[0]), int.Parse(positionParts[1]));
                            }
                            break;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载配置文件时出错: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 从配置值解析窗口大小
        /// </summary>
        /// <param name="size">配置中的大小字符串</param>
        /// <returns>窗口大小</returns>
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

        /// <summary>
        /// 打开配置文件
        /// </summary>
        public void OpenConfigFile()
        {
            try
            {
                System.Diagnostics.Process.Start("notepad.exe", configFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法打开配置文件: {ex.Message}");
            }
        }
    }
}