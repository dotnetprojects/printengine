using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SUT.PrintEngine.ViewModels;

namespace SUT.PrintEngine.Views
{
    /// <summary>
    /// Interaction logic for AdvancedColorPicker.xaml
    /// </summary>
    public partial class PrintControlView : IPrintControlView
    {
        public PrintControlView()
        {
            InitializeComponent();
        }

        #region IView Members

        private APrintControlViewModel _viewModel;
        public IViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
            set
            {
                _viewModel = value as APrintControlViewModel;
                DataContext = _viewModel;
            }
        }
        public void SetPageNumberVisibility(Visibility visibility)
        {
        }

        #endregion

        #region IPrintControlView Members

        DocumentViewer IPrintControlView.DocumentViewer
        {
            get
            {
                return null;// DocViewer;
            }
        }

        public StackPanel GetPagePreviewContainer()
        {
            return PagePreviewContainer;
        }

        public ScrollViewer GetSv()
        {
            return null;//sv;
        }

        #endregion

        public void ScalePreviewPaneVisibility(bool isVisible)
        {
        }

        public void ResizeButtonVisibility(bool isVisible)
        {
        }

        public void PrintingOptionsWaitCurtainVisibility(bool isVisible)
        {
        }
        public void ScalePreviewNode(ScaleTransform scaleTransform)
        {
            var printControlViewModel = _viewModel as PrintControlViewModel;
            if (printControlViewModel != null)
                printControlViewModel.ScalePreviewLayout = scaleTransform;
        }

        internal void EnablePrintingOptionsSet(bool isEnabled)
        {
        }
    }
}