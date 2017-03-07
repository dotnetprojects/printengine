using System.Windows;
using System.Windows.Media;
using DotNetProjects.PrintEngine.Paginators;
using DotNetProjects.PrintEngine.Utils;
using SUT.PrintEngine.Utils;
using SUT.PrintEngine.Views;

namespace SUT.PrintEngine.ViewModels
{
    public class CustomGridDataPrintContolViewModel: ItemsPrintControlViewModel, IDataTablePrintControlViewModel
    {
        private readonly CustomGridDataTable _table;
        private readonly string _header;
        public CustomGridDataPrintContolViewModel(PrintControlView view, CustomGridDataTable table, string header) : base(view)
        {
            _table = table;
            _header = header;
            PrintControlView.ScaleButtonVisibility(false);
        }

        protected override void CreatePaginator(DrawingVisual visual, Size printSize)
        {
            Paginator = new CustomGridDataPaginator(visual, printSize, PrintUtility.GetPageMargin(CurrentPrinterName), PrintTableDefination);
        }

        public override void ReloadPreview()
        {
            CustomGridDataHelper.SetupPrintControlPresenter(_table, this, _header, CurrentPaper, PageOrientation);
            FitToPage = false;
            base.ReloadPreview();
        }
    }
}
