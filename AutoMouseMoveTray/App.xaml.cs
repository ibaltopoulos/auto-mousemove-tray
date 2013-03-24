using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Application = System.Windows.Application;

namespace AutoMouseMoveTray
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly NotifyIcon _notifyIcon = new NotifyIcon();
        private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer();
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SetupNotifyIcon();
            SetupIdleTimeChecker();
        }

        private void SetupIdleTimeChecker() {
            _dispatcherTimer.Tick += moveMouseifIdle;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            _dispatcherTimer.Start();
        }

        private void SetupNotifyIcon() {
            _notifyIcon.Icon = AutoMouseMoveTray.Properties.Resources.TrayIcon;// new Icon(@"../../TrayIcon.ico");
            _notifyIcon.ShowBalloonTip(5000, "Hi", "This is a BallonTip from Windows Notification", ToolTipIcon.Info);
            _notifyIcon.Visible = true;

            _notifyIcon.MouseClick += notifyIcon_MouseClick;
        }

        void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) {
                Current.Shutdown();
            }
        }

        private void moveMouseifIdle(object sender, EventArgs e) {
            var idleTime = IdleTimeDetector.GetIdleTimeInfo();
            const int idleTimeThreshold = 4;
            if (idleTime.IdleTime.TotalSeconds < idleTimeThreshold) return;
            
            // They are idle!
            var newX = Math.Min(Cursor.Position.X + 1, (int) SystemParameters.PrimaryScreenWidth);
            var newY = Math.Max(Cursor.Position.Y - 1, 0);

            SetCursorPos(newX, newY);
        }

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);



        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _dispatcherTimer.Stop();
        } 
    }

    public static class IdleTimeDetector
    {
        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        public static IdleTimeInfo GetIdleTimeInfo()
        {
            int systemUptime = Environment.TickCount,
                lastInputTicks = 0,
                idleTicks = 0;

            var lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                lastInputTicks = (int)lastInputInfo.dwTime;

                idleTicks = systemUptime - lastInputTicks;
            }

            return new IdleTimeInfo
            {
                LastInputTime = DateTime.Now.AddMilliseconds(-1 * idleTicks),
                IdleTime = new TimeSpan(0, 0, 0, 0, idleTicks),
                SystemUptimeMilliseconds = systemUptime,
            };
        }
    }

    public class IdleTimeInfo
    {
        public DateTime LastInputTime { get; internal set; }

        public TimeSpan IdleTime { get; internal set; }

        public int SystemUptimeMilliseconds { get; internal set; }
    }

    internal struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }
}
