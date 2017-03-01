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
    /// Interaction logic for PrintParametersView.xaml
    /// </summary>
    public partial class PrintParametersView : UserControl, IWaitScreenView
    {
        private PrintParametersViewModel _viewModel;
        public PrintParametersView()
        {
            InitializeComponent();
        }

        public IViewModel ViewModel
        {
            set
            {
                _viewModel = value as PrintParametersViewModel;
                DataContext = _viewModel;
            }
        }

        public void SetPageNumberVisibility(Visibility visibility)
        {
            PageNumberMarker.Visibility = visibility;
        }

        public void PrintingOptionsWaitCurtainVisibility(bool isVisible)
        {
            if (isVisible)
                PrintingOptionsWaitCurtain.Visibility = Visibility.Visible;
            else
                PrintingOptionsWaitCurtain.Visibility = Visibility.Collapsed;
        }

        public void SetPrintingOptionsWaitCurtainVisibility(Visibility visibility)
        {
            PrintingOptionsWaitCurtain.Visibility = visibility;
        }
    }
}
