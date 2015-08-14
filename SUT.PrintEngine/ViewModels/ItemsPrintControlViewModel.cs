using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using SUT.PrintEngine.Paginators;
using SUT.PrintEngine.Utils;
using SUT.PrintEngine.Views;

namespace SUT.PrintEngine.ViewModels
{
    public class ItemsPrintControlViewModel : PrintControlViewModel, IItemsPrintControlViewModel
    {
        public ItemsPrintControlViewModel(PrintControlView view)
            : base(view)
        {
        }

        public List<double> ColumnsWidths { get; set; }
        public List<double> RowHeights { get; set; }
        public PrintTableDefination PrintTableDefination { get; set; }

        protected override void CreatePaginator(DrawingVisual visual, Size printSize)
        {
            Paginator = new ItemsPaginator(visual, printSize, PrintUtility.GetPageMargin(CurrentPrinterName), PrintTableDefination);
        }
    }
}