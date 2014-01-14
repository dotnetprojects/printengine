using System.Windows;
using System.Windows.Media;
using SUT.PrintEngine.Utils;

namespace SUT.PrintEngine.Paginators
{
    public class DataGridPaginator : ItemsPaginator
    {
        public DataGridPaginator(DrawingVisual source, Size printSize, Thickness pageMargins, PrintTableDefination printTableDefination)
            : base(source, printSize, pageMargins, printTableDefination)
        {
        }

        protected override int GetHorizontalPageCount()
        {            
            var pageCountX = 0;
            double totalWidth = 0;
            double lastTotalWidth = 0;
            int columnCount = 0;
            for (var i = 0; i < PrintTableDefination.ColumnWidths.Count; i++)
            {
                lastTotalWidth = totalWidth + PrintTableDefination.ColumnWidths[i];
                if (totalWidth + PrintTableDefination.ColumnWidths[i] <= PrintablePageWidth)
                {
                    totalWidth += PrintTableDefination.ColumnWidths[i];
                    columnCount++;
                }
                else
                {
                    pageCountX++;
                    ColumnCount.Add(columnCount);
                    AdjustedPageWidths.Add(totalWidth);
                    columnCount = 0;
                    totalWidth = 0;
                    i--;
                }

            }
            ColumnCount.Add(columnCount);
            AdjustedPageWidths.Add(lastTotalWidth);
            return pageCountX + 1;
        }
        protected override double GetPageWidth(int pageNumber)
        {
            return AdjustedPageWidths[pageNumber];
        }
    }
}