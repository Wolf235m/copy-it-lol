using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace MythicBootShell
{
    public partial class WeAreDumb : Window
    {
        DispatcherTimer fallbackTimer;

        public WeAreDumb()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string videoPath = "Video.mp4";
                if (!File.Exists(videoPath))
                    throw new FileNotFoundException("Video not found.");

                BootVideo.Source = new Uri(videoPath, UriKind.Relative);
                BootVideo.Play();

                fallbackTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(15)
                };
                fallbackTimer.Tick += (s, _) =>
                {
                    fallbackTimer.Stop();
                    LaunchShell();
                };
                fallbackTimer.Start();
            }
            catch
            {
                LaunchShell();
            }
        }

        private void BootVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            FadeOutAndLaunch();
        }

        private void BootVideo_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            LaunchShell();
        }

        private void FadeOutAndLaunch()
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(2));
            fadeOut.Completed += (s, _) => LaunchShell();
            this.BeginAnimation(Window.OpacityProperty, fadeOut);
        }

        private void LaunchShell()
        {
            try
            {
                Process.Start("C:\\Windows\\System32\\userinit.exe");
            }
            catch
            {
                Process.Start("explorer.exe");
            }
            Application.Current.Shutdown();
        }
    }
}
