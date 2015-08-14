using System.Windows;
using System.Windows.Media;
using SUT.PrintEngine.Paginators;
using SUT.PrintEngine.Views;

namespace SUT.PrintEngine.ViewModels
{
    public class DataTablePrintControlViewModel : ItemsPrintControlViewModel, IDataTablePrintControlViewModel
    {
        public DataTablePrintControlViewModel(PrintControlView view)
            : base(view)
        {
        }

        protected override void CreatePaginator(DrawingVisual visual, Size printSize)
        {
            Paginator = new DataTablePaginator(visual, printSize, PrintUtility.GetPageMargin(CurrentPrinterName), PrintTableDefination);
        }
    }
}