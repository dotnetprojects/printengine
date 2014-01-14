using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using SUT.PrintEngine.Extensions;
using SUT.PrintEngine.Resources;
using SUT.PrintEngine.ViewModels;

namespace SUT.PrintEngine.Controls.WaitScreen
{
    public class WaitScreenViewModel : AViewModel, IWaitScreenViewModel
    {
        private Window _waitScreenWindow;

        protected Window WaitScreenWindow
        {
            get
            {
                if (_waitScreenWindow == null)
                {
                    if (((UserControl)View).Parent != null)
                    {
                        ((Window)((UserControl)View).Parent).Content = null;
                    }
                    _waitScreenWindow = new Window
                                            {
                                                AllowsTransparency = true,
                                                Content = View as UIElement,
                                                WindowStyle = WindowStyle.None,
                                                ShowInTaskbar = false,
                                                Background = new SolidColorBrush(Colors.Transparent),
                                                Padding = new Thickness(0),
                                                Margin = new Thickness(0),
                                                WindowState = WindowState.Normal,
                                                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                                                SizeToContent = SizeToContent.WidthAndHeight,
                                            };
                }
                return _waitScreenWindow;
            }
        }

        private readonly Dispatcher _dispatcher;
        public WaitScreenViewModel(IWaitScreenView view)
            : base(view)
        {
            if (Application.Current != null)
            {
                _dispatcher = Application.Current.Dispatcher;
            }
            _hideTimer = new Timer();
            _hideTimer.Elapsed += HideTimerElapsed;
            _hideTimer.Interval = 300;
            _hideTimer.Enabled = false;

            Enabled = true;

            ((UserControl)view).Loaded += WaitScreenPresenterLoaded;
        }


        void WaitScreenPresenterLoaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
        }
        public bool Enabled { get; set; }

        #region IWaitScreenViewModel Members

        public string Message
        {
            get
            {
                return (string)GetValue(MessageProperty);
            }
            set
            {
                SetValue(MessageProperty, value);
            }
        }

        public void HideNow()
        {
            HideWaitScreenHandler();
        }

        #endregion

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
                "Message",
                typeof(string),
                typeof(WaitScreenViewModel));

        void HideTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _hideTimer.Stop();
            if (_dispatcher != null)
                _dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(HideWaitScreenHandler));
        }

        private void HideWaitScreenHandler()
        {
            if (((UserControl)View).Parent != null)
            {
                ((Window)((UserControl)View).Parent).Content = null;
            }
            _isShown = false;
            WaitScreenWindow.Close();
            if (DisableParent && Application.Current != null)
            {
                if (Application.Current != null && Application.Current.MainWindow != null)
                {
                    Application.Current.EnableWindow();
                    ////Application.Current.MainWindow.Focus();
                }
            }
            _waitScreenWindow = null;
        }
        private bool _isShown;
        private bool _isLoaded;
        private readonly Timer _hideTimer;
        public bool Show()
        {
            return Show(StringTable.WaitScreenMessage, true);
        }
        public bool Show(string message)
        {
            return Show(message, true);
        }

        private void ShowWaitScreenHandler()
        {
            if (_isShown)
            {
                _hideTimer.Stop();
                return;
            }
            _isShown = true;
            WaitScreenWindow.Owner = ApplicationExtention.MainWindow;
            WaitScreenWindow.Show();
            if (DisableParent && Application.Current != null)
                Application.Current.DisableWindow(0.90);
        }

        public bool Hide()
        {
            if (_isShown == false)
                return false;
            if (Initiator != null)
                return false;
            _hideTimer.Start();            
            return true;
        }

        #region IWaitScreenViewModel Members

        public bool Show(string message, bool disableParent)
        {
            if (_isShown)
                return false;
            Message = message;
            DisableParent = disableParent;
            if (Enabled && _dispatcher != null)
            {
                _dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(ShowWaitScreenHandler));
                Block(true);
            }
            return true;
        }
        private void Block(bool loaded)
        {
            if (Application.Current == null)
                return;
            while (true)
            {
                Application.Current.DoEvents();
                System.Threading.Thread.Sleep(5);
                if (_isLoaded != loaded)
                    continue;
                break;
            }
        }

        #endregion

        public bool DisableParent { get; set; }


        #region IWaitScreenViewModel Members

        public object Initiator { get; set; }

        #endregion
    }
}
