using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Unity;

namespace SUT.PrintEngine.Extensions
{
    public static class ApplicationExtention
    {
        public static IUnityContainer UnityContainer { get; set; }

        private static IEventAggregator EventAggregator
        {
            get
            {
                return null;// UnityContainer.Resolve<IEventAggregator>();
            }
        }

        public static event EventHandler WindowEnabled;
        public static event EventHandler WindowDisabled;
        private static Window _mainWindow;
        public static Window MainWindow
        {
            set
            {
                if (!DifferentWindow(value)) return;

                ChangeMainWindow(value);
            }
            get
            {
                return GetMainWindow();
            }
        }

        private static Window GetMainWindow()
        {
            return _mainWindow ?? Application.Current.MainWindow;
        }

        private static void ChangeMainWindow(Window newMainWindow)
        {
            var oldMainWindow = _mainWindow ?? Application.Current.MainWindow;
            _mainWindow = newMainWindow ?? Application.Current.MainWindow;
            UnsetWindowActivationEventHandlers(oldMainWindow);
            SetWindowActivationEventHandlers(newMainWindow);
            PublishMainWindowChangedEvent(oldMainWindow);
        }

        private static void SetWindowActivationEventHandlers(Window window)
        {
            if (window != null && window != Application.Current.MainWindow)
            {
                window.Activated += WindowActivated;
                window.Deactivated += WindowDeactivated;
            }
        }
        private static void UnsetWindowActivationEventHandlers(Window window)
        {
            if (window != null && window != Application.Current.MainWindow)
            {
                window.Activated += WindowActivated;
                window.Deactivated += WindowDeactivated;
            }
        }

        private static void WindowDeactivated(object sender, EventArgs e)
        {
            //if (EventAggregator != null)
            //    EventAggregator.GetEvent<BaseEvent<MainWindowActivationChangedEventArgs>>().Publish(new MainWindowActivationChangedEventArgs { Sender = sender, IsActivated = false });
        }

        private static void WindowActivated(object sender, EventArgs e)
        {
            //if (EventAggregator != null)
            //    EventAggregator.GetEvent<BaseEvent<MainWindowActivationChangedEventArgs>>().Publish(new MainWindowActivationChangedEventArgs { Sender = sender, IsActivated = true });
        }

        private static void PublishMainWindowChangedEvent(Window oldMainWindow)
        {
            if (EventAggregator != null)
            {
                //EventAggregator.GetEvent<BaseEvent<MainWindowChangedEventArgs>>().Publish(new MainWindowChangedEventArgs { Sender = null, CurrentMainWindow = _mainWindow, OldMainWindow = oldMainWindow });
            }
        }

        private static bool DifferentWindow(Window newMainWindow)
        {
            return _mainWindow != newMainWindow;
        }

        public static void DisableWindow(this Application source)
        {
            source.DisableWindow(0.5);
        }

        public static void DisableWindow(this Application source, double opacity)
        {
            if (!ContainsValidWindow(source)) return;

            var window = MainWindow ?? source.Windows[0];
            DisableWindow(window, opacity);
        }

        private static void DisableWindow(ContentControl window, double opacity)
        {
            window.Focusable = true;
            window.MouseEnter += WindowMouseEnter;
            if (((FrameworkElement)window.Content).IsEnabled == false)
                return;
            ((FrameworkElement)window.Content).IsEnabled = false;
            MakeDisabledOpacity(window, opacity);
            PublishMainWindowEnableChangedEven(window, false);
        }

        private static void MakeDisabledOpacity(UIElement window, double opacity)
        {
            window.Opacity = opacity;
        }

        private static void PublishMainWindowEnableChangedEven(ContentControl window, bool isEnabled)
        {
            if (WindowDisabled != null)
                WindowDisabled(window, new EventArgs());
            if (EventAggregator != null)
            {
                //EventAggregator.GetEvent<BaseEvent<MainWindowEnableChangedEventArgs>>().Publish(new MainWindowEnableChangedEventArgs { Sender = window, IsEnabled = isEnabled });
            }
        }

        private static bool ContainsValidWindow(Application source)
        {
            return source != null && source.Windows.Count > 0;
        }

        static void WindowMouseEnter(object sender, MouseEventArgs e)
        {
            Keyboard.Focus(sender as UIElement);
        }

        public static void EnableWindow(this Application source)
        {
            if (source != null && source.Windows.Count > 0)
            {
                var window = MainWindow ?? source.Windows[0];
                if (window != null)
                {
                    window.MouseEnter -= WindowMouseEnter;
                    if (((FrameworkElement)window.Content).IsEnabled)
                        return;
                    ((FrameworkElement)window.Content).IsEnabled = true;
                    window.Opacity = 1;
                }
                if (WindowEnabled != null)
                    WindowEnabled(source, new EventArgs());
                if (EventAggregator != null)
                {
                    //EventAggregator.GetEvent<BaseEvent<MainWindowEnableChangedEventArgs>>().Publish(new MainWindowEnableChangedEventArgs { Sender = source, IsEnabled = true });
                }
            }
        }
        public static bool IsWindowEnabled(this Application source)
        {
            if (source == null)
                return false;
            var isEnabled = true;
            if (source.Windows.Count > 0)
            {
                var window = MainWindow ?? source.Windows[0];
                if (window != null) isEnabled = ((FrameworkElement)window.Content).IsEnabled;
            }
            return isEnabled;
        }

        public static void DoEvents(this Application source)
        {
            if (source == null)
                return;
            source.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
        }
        public static void Wait(this Application source, double miliseconds)
        {
            var start = DateTime.Now;
            while ((DateTime.Now - start).TotalMilliseconds < miliseconds)
            {
                source.DoEvents();
            }
        }

        private static int _priorityCount;
        public static void ResetDoEventsPriority(this Application source)
        {
            _priorityCount = 0;
        }

        public static void DoEvents(this Application source, int priority)
        {
            if (++_priorityCount != priority) return;
            _priorityCount = 0;
            DoEvents(source);
        }
    }
}
