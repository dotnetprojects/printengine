using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SUT.PrintEngine.ViewModels;
using SUT.PrintEngine.Controls.WaitScreen;

namespace SUT.PrintEngine.Views
{
    /// <summary>
    /// Interaction logic for ScaleView.xaml
    /// </summary>
    public partial class ScaleView : UserControl, IWaitScreenView
    {
        private ScaleViewModel _viewModel;

        public ScaleView()
        {
            InitializeComponent();
        }

        public IViewModel ViewModel
        {
            set
            {
                _viewModel = value as ScaleViewModel;
                DataContext = _viewModel;
            }
        }

        private void cb_FitToPage_Check(object sender, RoutedEventArgs e)
        {
            if (cb_FitToPage.IsChecked == true)
                PageNumbersSlider.IsEnabled = false;
            else
                PageNumbersSlider.IsEnabled = true;
        }

        public void ScalePreviewNode(ScaleTransform scaleTransform)
        {
            PreviewNode.LayoutTransform = scaleTransform;
        }
    }
}
