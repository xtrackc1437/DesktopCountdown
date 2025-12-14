using System;
using System.Drawing;
using System.IO;
using System.Linq;
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
                MessageBox.Show("配置文件未找到，正在创建默认配置...");
                WriteDefaultConfiguration();
            }

            try
            {
                string[] configLines = File.ReadAllLines(configFilePath);
                
                // 使用Select将配置行转换为键值对，忽略注释行（以#开头的行）
                var configPairs = configLines
                    .Where(line => !string.IsNullOrWhiteSpace(line) && !line.Trim().StartsWith("#"))
                    .Select(line => line.Split('='))
                    .Where(parts => parts.Length == 2)
                    .ToDictionary(
                        parts => parts[0].Trim(),
                        parts => parts[1].Trim()
                    );

                // 解析配置值，添加详细的错误处理
                if (configPairs.TryGetValue("EventName", out string eventName))
                {
                    EventName = eventName;
                }
                else
                {
                    MessageBox.Show("配置文件缺少必填字段: EventName");
                    return false;
                }

                if (configPairs.TryGetValue("TargetTime", out string targetTimeStr))
                {
                    try
                    {
                        TargetTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(targetTimeStr)).DateTime;
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("配置文件中TargetTime字段格式无效: 应为有效的Unix时间戳");
                        return false;
                    }
                    catch (ArgumentException)
                    {
                        MessageBox.Show("配置文件中TargetTime字段值超出范围");
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("配置文件缺少必填字段: TargetTime");
                    return false;
                }

                if (configPairs.TryGetValue("WindowSize", out string windowSizeStr))
                {
                    WindowSize = GetWindowSizeFromConfig(windowSizeStr);
                }
                else
                {
                    WindowSize = new Size(300, 150); // 默认值
                }

                if (configPairs.TryGetValue("FontSize", out string fontSizeStr))
                {
                    try
                    {
                        FontSize = int.Parse(fontSizeStr);
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("配置文件中FontSize字段格式无效: 应为整数");
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("配置文件缺少必填字段: FontSize");
                    return false;
                }

                if (configPairs.TryGetValue("TextColor", out string textColorStr))
                {
                    try
                    {
                        TextColor = ColorTranslator.FromHtml(textColorStr);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("配置文件中TextColor字段格式无效: 应为有效的HTML颜色代码");
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("配置文件缺少必填字段: TextColor");
                    return false;
                }

                if (configPairs.TryGetValue("WindowContent", out string windowContent))
                {
                    WindowContent = windowContent;
                }
                else
                {
                    MessageBox.Show("配置文件缺少必填字段: WindowContent");
                    return false;
                }

                if (configPairs.TryGetValue("WindowPosition", out string windowPositionStr))
                {
                    string[] positionParts = windowPositionStr.Split(',');
                    if (positionParts.Length == 2)
                    {
                        try
                        {
                            WindowPosition = new Point(int.Parse(positionParts[0]), int.Parse(positionParts[1]));
                        }
                        catch (FormatException)
                        {
                            MessageBox.Show("配置文件中WindowPosition字段格式无效: 应为X,Y坐标格式");
                            return false;
                        }
                    }
                }

                // 验证所有必填字段都已设置
                return ValidateRequiredFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载配置文件时出错: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// 验证所有必填字段是否都已设置
        /// </summary>
        /// <returns>是否通过验证</returns>
        private bool ValidateRequiredFields()
        {
            if (string.IsNullOrEmpty(EventName))
            {
                MessageBox.Show("配置验证失败: EventName不能为空");
                return false;
            }
            
            if (string.IsNullOrEmpty(WindowContent))
            {
                MessageBox.Show("配置验证失败: WindowContent不能为空");
                return false;
            }
            
            // 其他必填字段已经在解析时验证过
            return true;
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

        /// <summary>
        /// 创建默认配置文件
        /// </summary>
        private void WriteDefaultConfiguration()
        {
            try
            {
                // 获取屏幕信息以确定右上角位置
                Screen screen = Screen.PrimaryScreen;
                int rightMargin = 50;
                int topMargin = 50;
                Point topRightPosition = new Point(screen.WorkingArea.Right - 300 - rightMargin, screen.WorkingArea.Top + topMargin);

                // 设置默认配置值
                EventName = "默认事件";
                TargetTime = DateTime.Now.AddDays(1); // 默认1天后
                WindowContent = "距离{EventName}还有{Days}天{Hours}小时{Mins}分{Seconds}秒";
                WindowSize = new Size(300, 150);
                FontSize = 24;
                TextColor = Color.Orange;
                WindowPosition = topRightPosition;

                // 构建配置内容，包含注释和创建时间
                string[] configLines = new string[]
                {
                    "# 配置文件创建时间 (UTC): " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    "# Created Time (UTC): " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    "",
                    "# 配置教程 / Configuration Tutorial",
                    "# ----------------------------------",
                    "# EventName: 事件名称",
                    "# EventName: Event name",
                    "",
                    "# TargetTime: 目标时间 (Unix时间戳格式)",
                    "# TargetTime: Target time (Unix timestamp format)",
                    "",
                    "# WindowContent: 窗口显示内容模板，可用变量: {EventName}, {Days}, {Hours}, {Mins}, {Seconds}",
                    "# WindowContent: Window display template, available variables: {EventName}, {Days}, {Hours}, {Mins}, {Seconds}",
                    "",
                    "# WindowSize: 窗口大小 (small/medium/large)",
                    "# WindowSize: Window size (small/medium/large)",
                    "",
                    "# FontSize: 字体大小",
                    "# FontSize: Font size",
                    "",
                    "# TextColor: 文本颜色 (HTML颜色代码或颜色名称)",
                    "# TextColor: Text color (HTML color code or color name)",
                    "",
                    "# WindowPosition: 窗口位置 (X,Y坐标)",
                    "# WindowPosition: Window position (X,Y coordinates)",
                    "",
                    $"EventName={EventName}",
                    $"TargetTime={DateTimeOffset.Now.AddDays(1).ToUnixTimeSeconds()}",
                    $"WindowContent={WindowContent}",
                    $"WindowSize=medium",
                    $"FontSize={FontSize}",
                    $"TextColor={ColorTranslator.ToHtml(TextColor)}",
                    $"WindowPosition={WindowPosition.X},{WindowPosition.Y}"
                };

                // 写入配置文件
                File.WriteAllLines(configFilePath, configLines);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"创建默认配置文件时出错: {ex.Message}");
            }
        }
    }
}