using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SUT.PrintEngine.Utils
{
    public class CustomGridDataTable
    {
        public double ScaleWidths { get; set; } = 1;
        public IList<CustomGridDataColumn> Columns { get; } = new List<CustomGridDataColumn>();
        public IList<CustomGridDataRow> Rows { get; } = new List<CustomGridDataRow>(); 
        public Thickness Margin { get; set; }
        public IList<double> ColumnWidths
        {
            get { return Columns.Select(c => c.Width * ScaleWidths).ToList(); }
        }

        public IList<double> ColumnWidthsWithoutScale
        {
            get { return Columns.Select(c => c.Width).ToList(); }
        }

        public string[] ColumnNames
        {
            get { return Columns.Select(c => c.Header.ToString()).ToArray(); }
        } 

        public void AddColumn(CustomGridDataColumn column)
        {
            Columns.Add(column);
        }

        public void AddRow(CustomGridDataRow row)
        {
            Rows.Add(row);
        }
    }
}
