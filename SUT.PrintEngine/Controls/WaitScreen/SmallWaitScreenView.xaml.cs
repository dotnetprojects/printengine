using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;
using SUT.PrintEngine.ViewModels;

namespace SUT.PrintEngine.Controls.WaitScreen
{
    /// <summary>
    /// Interaction logic for WaitScreenView.xaml
    /// </summary>
    public partial class SmallWaitScreenView : UserControl,IWaitScreenView
    {
        public SmallWaitScreenView()
        {
            InitializeComponent();
            _animationTimer = new DispatcherTimer(
                DispatcherPriority.Input, Dispatcher) {Interval = new TimeSpan(0, 0, 0, 0, 75)};
        }

        #region IView Members

        public IWaitScreenViewModel WaitScreenViewModel;
        public IViewModel ViewModel
        {
            set 
            {
                WaitScreenViewModel = value as IWaitScreenViewModel;
                DataContext = value;
            }
        }

        #endregion

         #region Data
        private readonly DispatcherTimer _animationTimer;
        #endregion

        #region Private Methods
        private void Start()
        {
            _animationTimer.Tick += HandleAnimationTick;
            _animationTimer.Start();
        }

        private void Stop()
        {
            _animationTimer.Stop();
            _animationTimer.Tick -= HandleAnimationTick;
        }

        private void HandleAnimationTick(object sender, EventArgs e)
        {
            SpinnerRotate.Angle = (SpinnerRotate.Angle + 36) % 360;
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            const double offset = Math.PI;
            const double step = Math.PI * 2 / 10.0;

            SetPosition(C0, offset, 0.0, step);
            SetPosition(C1, offset, 1.0, step);
            SetPosition(C2, offset, 2.0, step);
            SetPosition(C3, offset, 3.0, step);
            SetPosition(C4, offset, 4.0, step);
            SetPosition(C5, offset, 5.0, step);
            SetPosition(C6, offset, 6.0, step);
            SetPosition(C7, offset, 7.0, step);
            SetPosition(C8, offset, 8.0, step);
        }

        private static void SetPosition(Ellipse ellipse, double offset,
            double posOffSet, double step)
        {
            const double diff = 10.0;
            ellipse.SetValue(Canvas.LeftProperty, diff
                + Math.Sin(offset + posOffSet * step) * diff);

            ellipse.SetValue(Canvas.TopProperty, diff
                + Math.Cos(offset + posOffSet * step) * diff);
        }

        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void HandleVisibleChanged(object sender,
            DependencyPropertyChangedEventArgs e)
        {
            var isVisible = (bool)e.NewValue;

            if (isVisible)
                Start();
            else
                Stop();
        }
        #endregion

    }
}
