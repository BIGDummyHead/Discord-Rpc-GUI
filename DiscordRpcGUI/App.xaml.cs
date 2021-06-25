using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
            not.Click += OpenBack;
        }

        public static void Min()
        {
            not.Visible = true;
            not.ShowBalloonTip(3000, "Discord RPC", "Your RPC has been minimized to the tray! Don't worry we'll keep the hamsters rollin'", ToolTipIcon.Info);
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
