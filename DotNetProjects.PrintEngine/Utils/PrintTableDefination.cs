using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace SUT.PrintEngine.Utils
{
    public class PrintTableDefination
    {
        public IList<double> ColumnWidths { get; set; }
        public IList<double> RowHeights { get; set; }
        public bool HasFooter { get; set; }
        public string FooterText { get; set; }
        public string[] ColumnNames { get; set; }
        public SolidColorBrush ColumnHeaderBrush { get; set; }
        public int ColumnHeaderFontSize { get; set; }
        public int ColumnHeaderHeight { get; set; }
        public string HeaderTemplate { get; set; }
    }
}