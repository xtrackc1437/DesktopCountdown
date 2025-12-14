using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace DesktopCountdown
{
    /// <summary>
    /// 主题管理器，负责检测和管理系统主题
    /// </summary>
    public static class ThemeManager
    {
        /// <summary>
        /// 获取当前系统是否使用深色主题
        /// </summary>
        /// <returns>如果是深色主题返回true，否则返回false</returns>
        public static bool IsDarkThemeEnabled()
        {
            try
            {
                // 检查Windows 10及以上版本的系统主题设置
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null)
                    {
                        object value = key.GetValue("AppsUseLightTheme");
                        if (value != null)
                        {
                            // 0表示深色主题，1表示浅色主题
                            return (int)value == 0;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // 如果无法访问注册表，默认返回浅色主题
            }

            return false;
        }

        /// <summary>
        /// 为ContextMenuStrip应用主题样式
        /// </summary>
        /// <param name="menuStrip">要应用主题的ContextMenuStrip</param>
        public static void ApplyTheme(ContextMenuStrip menuStrip)
        {
            if (menuStrip == null)
                return;

            bool isDarkTheme = IsDarkThemeEnabled();
            
            if (isDarkTheme)
            {
                // 应用深色主题
                menuStrip.BackColor = System.Drawing.Color.FromArgb(32, 32, 32);
                menuStrip.ForeColor = System.Drawing.Color.White;
                
                // 为所有菜单项应用主题
                ApplyDarkThemeToItems(menuStrip.Items);
            }
            else
            {
                // 应用浅色主题（默认主题）
                menuStrip.BackColor = System.Drawing.SystemColors.Menu;
                menuStrip.ForeColor = System.Drawing.SystemColors.MenuText;
                
                // 为所有菜单项应用主题
                ApplyLightThemeToItems(menuStrip.Items);
            }
        }

        /// <summary>
        /// 为ToolStripItemCollection应用深色主题
        /// </summary>
        /// <param name="items">要应用主题的ToolStripItemCollection</param>
        private static void ApplyDarkThemeToItems(System.Windows.Forms.ToolStripItemCollection items)
        {
            foreach (System.Windows.Forms.ToolStripItem item in items)
            {
                if (item is System.Windows.Forms.ToolStripMenuItem menuItem)
                {
                    menuItem.BackColor = System.Drawing.Color.FromArgb(32, 32, 32);
                    menuItem.ForeColor = System.Drawing.Color.White;
                    
                    // 递归处理子菜单
                    if (menuItem.DropDownItems.Count > 0)
                    {
                        ApplyDarkThemeToItems(menuItem.DropDownItems);
                    }
                }
                else if (item is System.Windows.Forms.ToolStripSeparator)
                {
                    // 分隔线样式
                    item.ForeColor = System.Drawing.Color.Gray;
                }
            }
        }

        /// <summary>
        /// 为ToolStripItemCollection应用浅色主题
        /// </summary>
        /// <param name="items">要应用主题的ToolStripItemCollection</param>
        private static void ApplyLightThemeToItems(System.Windows.Forms.ToolStripItemCollection items)
        {
            foreach (System.Windows.Forms.ToolStripItem item in items)
            {
                if (item is System.Windows.Forms.ToolStripMenuItem menuItem)
                {
                    menuItem.BackColor = System.Drawing.SystemColors.Menu;
                    menuItem.ForeColor = System.Drawing.SystemColors.MenuText;
                    
                    // 递归处理子菜单
                    if (menuItem.DropDownItems.Count > 0)
                    {
                        ApplyLightThemeToItems(menuItem.DropDownItems);
                    }
                }
                else if (item is System.Windows.Forms.ToolStripSeparator)
                {
                    // 分隔线样式
                    item.ForeColor = System.Drawing.SystemColors.ControlDark;
                }
            }
        }
    }
}