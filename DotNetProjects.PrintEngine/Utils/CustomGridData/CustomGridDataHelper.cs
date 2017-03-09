using System.Drawing.Printing;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SUT.PrintEngine.Extensions;
using SUT.PrintEngine.Utils;
using SUT.PrintEngine.ViewModels;

namespace DotNetProjects.PrintEngine.Utils
{
    public static class CustomGridDataHelper
    {
        /// <summary>
        /// Доп. смещение, необходимо, чтобы была граница справа
        /// </summary>
        private const int AuxillarySpace = 120;
        public static void SetupPrintControlPresenter(CustomGridDataTable dataTable, CustomGridDataPrintContolViewModel printControlPresenter, string headerTemplate, PaperSize paperSize, PageOrientation pageOrientation)
        {
            var pageAccrossWidth = dataTable.ColumnWidthsWithoutScale.Sum();
            
            if(pageOrientation == PageOrientation.Landscape)
                dataTable.ScaleWidths = (paperSize.Height - dataTable.Margin.Left - dataTable.Margin.Right - AuxillarySpace) / pageAccrossWidth;
            else
                dataTable.ScaleWidths = (paperSize.Width - dataTable.Margin.Left - dataTable.Margin.Right - AuxillarySpace) / pageAccrossWidth; 
            
            var customVisualGrid = CreateDocument(dataTable, pageAccrossWidth * dataTable.ScaleWidths);
            
            var rowHeights = PrintControlFactory.CalculateRowHeights(customVisualGrid);

            var drawingVisual = PrintControlFactory.CreateDrawingVisual(customVisualGrid, customVisualGrid.ActualWidth, customVisualGrid.ActualHeight, dataTable.Margin);

            var printTableDefination = new PrintTableDefination
            {
                ColumnWidths = dataTable.ColumnWidths,
                RowHeights = rowHeights,
                HasFooter = false,
                FooterText = null,
                ColumnHeaderFontSize = 12,
                ColumnHeaderBrush = Brushes.Black,
                ColumnHeaderHeight = 22,
                HeaderTemplate = headerTemplate
            };

            printControlPresenter.PrintTableDefination = printTableDefination;
            printControlPresenter.DrawingVisual = drawingVisual;
        }

        private static Border CreateDocument(CustomGridDataTable dataTable, double pageAcrossWidth)
        {
            Application.Current.DoEvents();
            var spBorder = new Border { BorderBrush = Brushes.Gray, BorderThickness = new Thickness(0.25), Background = Brushes.White };
            var stackPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Orientation = Orientation.Vertical
            };

            var borderHeader = new Border { BorderBrush = Brushes.Gray, BorderThickness = new Thickness(0.25), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
            var borderSp = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            foreach (var column in dataTable.Columns)
            {
                var tbBorder = new Border { BorderBrush = Brushes.Gray, BorderThickness = new Thickness(0.25, 0, 0.25, 0) };

                var grid = new Grid()
                {
                    Width = column.Width*dataTable.ScaleWidths
                };

                var textBlock = new TextBlock()
                {
                    FontWeight = FontWeights.Bold,
                    Text = column.Header.ToString(),
                    Margin = new Thickness(5, 2, 0, 0)
                };

                grid.Children.Add(textBlock);

                tbBorder.Child = grid;

                borderSp.Children.Add(tbBorder);
            }

            borderHeader.Child = borderSp;
            stackPanel.Children.Add(borderHeader);
            
            foreach (var dataRow in dataTable.Rows) 
            {
                Application.Current.DoEvents();
                var border = new Border { Margin = new Thickness(0, 0, 0, 0), BorderBrush = Brushes.Gray, BorderThickness = new Thickness(0.25), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
                var sp = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                foreach (var dataColumn in dataTable.Columns) 
                {
                    var tbBorder = new Border { HorizontalAlignment = HorizontalAlignment.Center, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(0.25, 0, 0.25, 0) };
                    var textBlock = CreateCellBlock(dataRow, dataColumn, tbBorder, dataTable.ScaleWidths);
                    tbBorder.Child = textBlock;
                    sp.Children.Add(tbBorder);
                }
                border.Child = sp;
                stackPanel.Children.Add(border);
            }
            spBorder.Child = stackPanel;

            UiUtil.UpdateSize(spBorder, pageAcrossWidth);

            return spBorder;
        }

        public static UIElement CreateCellBlock(CustomGridDataRow printDataRow, CustomGridDataColumn printDataColumn, Border tbBorder, double scale)
        {
            return printDataColumn.CellFunc(printDataRow.DataContext, scale);
        }
    }
}
