using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Windows.Xps.Serialization;

namespace SUT.PrintEngine.Extensions
{
    public static class ApplicationExtention
    {
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
        }
         
        private static bool DifferentWindow(Window newMainWindow)
        {
            return _mainWindow != newMainWindow;
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
            }
        }
           
        public static void DoEvents(this Application source)
        {
            if (source == null)
                return;
            source.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
        }
    }
}
