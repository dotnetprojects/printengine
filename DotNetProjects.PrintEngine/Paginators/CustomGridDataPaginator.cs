using System.Windows;
using System.Windows.Media;
using SUT.PrintEngine.Paginators;
using SUT.PrintEngine.Utils;

namespace DotNetProjects.PrintEngine.Paginators
{
    public class CustomGridDataPaginator: ItemsPaginator
    {
        public CustomGridDataPaginator(DrawingVisual source, Size printSize, Thickness pageMargins, PrintTableDefination printTableDefination) : 
            base(source, printSize, pageMargins, printTableDefination)
        {
        }
    }
}
