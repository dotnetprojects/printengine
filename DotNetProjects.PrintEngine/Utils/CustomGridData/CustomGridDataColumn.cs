using System;
using System.Windows;

namespace SUT.PrintEngine.Utils
{
    public class CustomGridDataColumn
    {
        public object Header { get; set; }

        public double Width { get; set; }

        public Func<object, double, UIElement> CellFunc { get; set; } 

        public object GridColumn { get; set; }
    }
}
