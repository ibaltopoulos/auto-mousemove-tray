using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace AutoMouseMoveTray
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly NotifyIcon _notifyIcon = new NotifyIcon();
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e); 

            _notifyIcon.Icon = AutoMouseMoveTray.Properties.Resources.TrayIcon;// new Icon(@"../../TrayIcon.ico");
            _notifyIcon.ShowBalloonTip(5000, "Hi", "This is a BallonTip from Windows Notification", ToolTipIcon.Info);
            _notifyIcon.Visible = true;

            _notifyIcon.MouseClick += _notifyIcon_MouseClick;
        }

        void _notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) {
                Current.Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
        } 
    }
}
