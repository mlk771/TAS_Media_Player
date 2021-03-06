﻿using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace Media_Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        static bool _isMaximized;
        static bool _isPaused;

        public MainWindow()
        {
            InitializeComponent();
            TsSlider.Width = WWindow.Width;
            LblInfo.Visibility = Visibility.Hidden;
            // SystemParameters.PrimaryScreenWidth;
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (_isPaused)
            {
                _isPaused = false;
                MediaElement.Play();
                BkgStartWindow.Visibility = Visibility.Hidden;
                LogoStartWindow.Visibility = Visibility.Hidden;
                LblMediaPlayer.Visibility = Visibility.Hidden;
                BkgTopBar.Opacity = 0;

                ChangeImage("Pause");

            }
            else
            {
                MediaElement.Pause();
                _isPaused = true;

                ChangeImage("Play");
            }
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            var forward = TimeSpan.FromSeconds(MediaElement.Position.TotalSeconds + 10);
            MediaElement.Position = forward;

            LblInfo.Visibility = Visibility.Visible;
            LblInfo.Content = "[ Forward: 10 Seconds ]";
            ShowHideTimer();
        }

        private void btnBackward_Click(object sender, RoutedEventArgs e)
        {
            var reverse = TimeSpan.FromSeconds(MediaElement.Position.TotalSeconds - 10);
            MediaElement.Position = reverse;

            LblInfo.Visibility = Visibility.Visible;
            LblInfo.Content = "[ Background: 10 Seconds ]";
            ShowHideTimer();
        }

        private void ChangeImage(string status)
        {
            var img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(Environment.
                                    CurrentDirectory.
                                    Replace("bin\\Debug", "Media\\") +
                                    status + ".png");
            img.EndInit();
            PlayPauseImg.Source = img;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            MediaElement.Stop();
            MediaElement.Close();
            BkgStartWindow.Visibility = Visibility.Visible;
            LogoStartWindow.Visibility = Visibility.Visible;
            LblMediaPlayer.Visibility = Visibility.Visible;
            LblFileName.Content = "";
            LblTimeElapsed.Content = "0:00:00";
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                AddExtension = true,
                DefaultExt = "*.*",
                Filter = Properties.Resources.Media_Files_Format
            };
            ofd.ShowDialog();

            try
            {
                MediaElement.Source = new Uri(ofd.FileName);
            }
            catch (Exception)
            {
                // ReSharper disable ObjectCreationAsStatement
                new NullReferenceException("Error");
                // ReSharper restore ObjectCreationAsStatement
            }
            LblFileName.Content = ofd.SafeFileName;
            MediaElement.MediaOpened += mediaElement_MediaOpened;
            MediaElement.Play();
            BkgStartWindow.Visibility = Visibility.Hidden;
            LogoStartWindow.Visibility = Visibility.Hidden;
            LblMediaPlayer.Visibility = Visibility.Hidden;
            ChangeImage("Pause");

            WWindow.Width = MediaElement.NaturalVideoWidth;
            WWindow.Height = MediaElement.NaturalVideoHeight;
        }

        void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += timer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            MediaElement.Volume = VolSlider.Value;
            var d = MediaElement.NaturalDuration;
            TsSlider.Maximum = d.TimeSpan.TotalSeconds;
            //d.TimeSpan.Hours + ":" + d.TimeSpan.Minutes + ":" + d.TimeSpan.Seconds;
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            TsSlider.Value = MediaElement.Position.TotalSeconds;
            LblTimeElapsed.Content = MediaElement.Position.Hours + ":" +
                                     MediaElement.Position.Minutes + ":" +
                                     MediaElement.Position.Seconds;
        }

        private void tsSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var ts = TimeSpan.FromSeconds(e.NewValue);
            MediaElement.Position = ts;

        }

        private void volSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MediaElement.Volume = VolSlider.Value;
            try
            {
                LblInfo.Visibility = Visibility.Visible;
                LblInfo.Content = "[ Volume: %" + Convert.ToString(Math.Round(VolSlider.Value * 100, 0)) + " ]";
                ShowHideTimer();
            }
            catch (Exception)
            {

            }

        }

        private void btnMaximizeWindow_Click(object sender, RoutedEventArgs e)
        {
            if (_isMaximized)
            {
                WWindow.WindowState = WindowState.Normal;
                TsSlider.Width = WWindow.Width - 305;
                _isMaximized = false;
            }
            else
            {
                WWindow.WindowState = WindowState.Maximized;
                TsSlider.Width = SystemParameters.PrimaryScreenWidth - 305;
                _isMaximized = true;
            }
        }

        private void DragWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void KeyPressed_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

            switch (e.Key)
            {
                case Key.Enter:
                    if (_isMaximized)
                    {
                        WWindow.WindowState = WindowState.Normal;
                        _isMaximized = false;
                    }
                    else
                    {
                        WWindow.WindowState = WindowState.Maximized;
                        _isMaximized = true;
                    }
                    break;
                case Key.Up:
                    VolSlider.Value += 0.05;
                    break;
                case Key.Down:
                    VolSlider.Value -= 0.05;
                    break;
                case Key.Left:
                    {
                        var reverse = TimeSpan.FromSeconds(MediaElement.Position.TotalSeconds - 10);
                        MediaElement.Position = reverse;
                    }
                    break;
                case Key.Right:
                    try
                    {
                        var forward = TimeSpan.FromSeconds(MediaElement.Position.TotalSeconds + 10);
                        MediaElement.Position = forward;
                    }
                    // ReSharper disable EmptyGeneralCatchClause
                    catch (Exception)
                    // ReSharper restore EmptyGeneralCatchClause
                    {

                    }
                    break;
            }
            if (e.Key == Key.Space)
            {
                if (_isPaused)
                {
                    MediaElement.Play();
                    _isPaused = false;
                }
                else
                {
                    MediaElement.Pause();
                    _isPaused = true;
                }
            }
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.O)
            {
                btnOpen_Click(sender, e);
            }
            Keyboard.Focus(MediaElement);
        }

        private void ShowHideTimer()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3d) };
            timer.Tick += TimerTick;
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            var timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= TimerTick;
            LblInfo.Visibility = Visibility.Hidden;
        }
        private void Media_Drop(object sender, System.Windows.DragEventArgs e)
        {
            try
            {
                //mediaElement.Source = new Uri(e.);
            }
            catch (Exception)
            {
                // ReSharper disable ObjectCreationAsStatement
                new NullReferenceException("Error");
                // ReSharper restore ObjectCreationAsStatement
            }

            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += timer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            MediaElement.Volume = VolSlider.Value;
        }

        private void TopBarTools_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeTopBarVisibility(Visibility.Visible, 1);
        }

        private void TopBarTools_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeTopBarVisibility(Visibility.Hidden, 0);
        }

        private void ChangeTopBarVisibility(Visibility status, int destination)
        {
            BtnMaximizeWindow.Visibility = status;
            BtnMinimizeWindow.Visibility = status;
            BtnClose.Visibility = status;
            LblFileName.Visibility = status;
            var animation = new DoubleAnimation(destination, TimeSpan.FromMilliseconds(250));
            BkgTopBar.BeginAnimation(OpacityProperty, animation);
        }

        private void BottomBar_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeBotBarVisibility(Visibility.Visible, 1);
        }

        private void BottomBar_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeBotBarVisibility(Visibility.Hidden, 0);
        }

        private void ChangeBotBarVisibility(Visibility status, int destination)
        {
            TsSlider.Visibility = status;
            BtnOpen.Visibility = status;
            BtnPlay.Visibility = status;
            BtnStop.Visibility = status;
            BtnBackward.Visibility = status;
            BtnForward.Visibility = status;
            VolSlider.Visibility = status;
            LblVol.Visibility = status;
            LblTimeElapsed.Visibility = status;
            var animation = new DoubleAnimation(destination, TimeSpan.FromMilliseconds(250));
            BkgBotBar.BeginAnimation(OpacityProperty, animation);
        }

        private void btnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnMinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            WWindow.WindowState = WindowState.Minimized;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new About();
            aboutWindow.Show();
        }

        private void resize_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TsSlider.Width = WWindow.Width - 310;
            BkgStartWindow.Height = WWindow.Height - 100;
        }
    }
}
