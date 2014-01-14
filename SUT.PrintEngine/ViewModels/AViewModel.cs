using System.Windows;
using SUT.PrintEngine.Views;

namespace SUT.PrintEngine.ViewModels
{
    public abstract class AViewModel:DependencyObject, IViewModel
    {
        protected AViewModel(IView view)
        {
            View = view;
            view.ViewModel = this;
        }
        #region IViewModel Members

        public IView View
        {
            get
            {
                return (IView) GetValue(ViewProperty);
            }
            set
            {
                SetValue(ViewProperty, value);
            }
        }
        public static readonly DependencyProperty ViewProperty =
           DependencyProperty.Register("View", typeof(IView), typeof(AViewModel));
        #endregion
    }
}
