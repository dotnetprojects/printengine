using System.Windows;
using System.Windows.Media;
using SUT.PrintEngine.Paginators;
using SUT.PrintEngine.Views;

namespace SUT.PrintEngine.ViewModels
{
    public class GridPrintControlViewModel : ItemsPrintControlViewModel, IGridPrintControlViewModel
    {
        public GridPrintControlViewModel(PrintControlView view)
            : base(view)
        {
        }
        protected override void CreatePaginator(DrawingVisual visual, Size printSize)
        {
            Paginator = new DataGridPaginator(visual, printSize, PrintUtility.GetPageMargin(CurrentPrinterName), PrintTableDefination);
        }
    }
}