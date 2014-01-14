using System.Collections.Generic;
using SUT.PrintEngine.Utils;

namespace SUT.PrintEngine.ViewModels
{
    public interface IItemsPrintControlViewModel : IPrintControlViewModel
    {
        List<double> ColumnsWidths { get; set; }
        List<double> RowHeights { get; set; }
        PrintTableDefination PrintTableDefination { get; set; }
    }
}