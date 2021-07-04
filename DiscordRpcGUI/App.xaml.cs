using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace DiscordRpcGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        readonly static NotifyIcon not = new();
        public App()
        {
            Stream iconStream = GetResourceStream(new Uri("pack://application:,,,/Resources/icon.ico")).Stream;
            not.Icon = new Icon(iconStream);
            iconStream.Dispose();
            not.BalloonTipClicked += OpenBack;
            not.Click += OpenBack;
        }

        public static void Min(bool includeMessage = true)
        {
            DiscordRpcGUI.MainWindow.Instance.Visibility = Visibility.Collapsed;
          DiscordRpcGUI.MainWindow.Instance.ShowInTaskbar = false;
            not.Visible = true;

            if(includeMessage)
            not.ShowBalloonTip(750, "Discord RPC", "Your RPC has been minimized to the tray! Don't worry we'll keep the hamsters rollin'", ToolTipIcon.Info);
        }

        public static async Task<bool> OneInstance()
        {
            Process[] procs = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);

            if (procs.Length > 1)
            {
                DiscordRpcGUI.MainWindow.Instance.Visibility = Visibility.Collapsed;
                DiscordRpcGUI.MainWindow.Instance.ShowInTaskbar = false;
                not.Visible = true;
                not.ShowBalloonTip(500, "Discord RPC", "Woah, you can't run two Discord RPC's that is like illegal or something!", ToolTipIcon.Warning);
                await Task.Delay(TimeSpan.FromSeconds(.1));
                Process.GetCurrentProcess().Kill();

                return true;
            }
            else
                return false;
        }

        void OpenBack(object sender, EventArgs e)
        {
            MainWindow.Visibility = Visibility.Visible;
            MainWindow.WindowState = WindowState.Normal;
            MainWindow.ShowInTaskbar = true;
            not.Visible = false;
        }
    }
}
