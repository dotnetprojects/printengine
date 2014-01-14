using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using SUT.PrintEngine.Utils;

namespace SUT.PrintEngine.Paginators
{
    public class VisualPaginator : DocumentPaginator
    {
        protected const string DrawingVisualNullMessage = "Drawing Visual Source is null";
        public int HorizontalPageCount;

        private readonly Size _printSize;
        protected Thickness PageMargins;
        private readonly Thickness _originalMargin;
        protected Size ContentSize;
        protected Pen FramePen;
        protected readonly List<double> AdjustedPageWidths = new List<double>();
        protected readonly List<double> AdjustedPageHeights = new List<double>();
        private int _verticalPageCount;
        protected Rect FrameRect;
        public List<DrawingVisual> DrawingVisuals;
        private IDocumentPaginatorSource _document;
        protected double PrintablePageWidth;
        public DrawingVisual DrawingVisual;
        protected double PrintablePageHeight;
        public bool ShowPageMarkers;
        protected double HeaderHeight;
        public event EventHandler<PageEventArgs> PageCreated;

        public void OnPageCreated(PageEventArgs e)
        {
            EventHandler<PageEventArgs> handler = PageCreated;
            if (handler != null) handler(this, e);
        }

        public VisualPaginator(DrawingVisual source, Size printSize, Thickness pageMargins, Thickness originalMargin)
        {
            DrawingVisual = source;
            _printSize = printSize;
            PageMargins = pageMargins;
            _originalMargin = originalMargin;

        }

        public void Initialize(bool isMarkPageNumbers)
        {
            ShowPageMarkers = isMarkPageNumbers;
            var totalHorizontalMargin = PageMargins.Left + PageMargins.Right;
            var toltalVerticalMargin = PageMargins.Top + PageMargins.Bottom;

            PrintablePageWidth = PageSize.Width - totalHorizontalMargin;
            PrintablePageHeight = PageSize.Height - toltalVerticalMargin;

            ContentSize = new Size(_printSize.Width - totalHorizontalMargin, _printSize.Height - toltalVerticalMargin);
            FrameRect = new Rect(new Point(PageMargins.Left, PageMargins.Top), new Size(_printSize.Width - totalHorizontalMargin, _printSize.Height - toltalVerticalMargin));
            FramePen = new Pen(Brushes.Black, 0);

            HorizontalPageCount = GetHorizontalPageCount();

            _verticalPageCount = GetVerticalPageCount();

            CreateAllPageVisuals();
        }

        private void CreateAllPageVisuals()
        {
            DrawingVisuals = new List<DrawingVisual>();

            for (var verticalPageNumber = 0; verticalPageNumber < _verticalPageCount; verticalPageNumber++)
            {
                for (var horizontalPageNumber = 0; horizontalPageNumber < HorizontalPageCount; horizontalPageNumber++)
                {
                    const float horizontalOffset = 0;
                    var verticalOffset = (float)(verticalPageNumber * PrintablePageHeight);
                    var pageBounds = GetPageBounds(horizontalPageNumber, verticalPageNumber, horizontalOffset, verticalOffset);
                    var visual = new DrawingVisual();
                    using (var dc = visual.RenderOpen())
                    {
                        CreatePageVisual(pageBounds, DrawingVisual,
                                         IsFooterPage(horizontalPageNumber), dc);
                    }
                    DrawingVisuals.Add(visual);
                }
            }
        }

        protected virtual Rect GetPageBounds(int horizontalPageNumber, int verticalPageNumber, float horizontalOffset, float verticalOffset)
        {
            var x = (float)((horizontalPageNumber * PrintablePageWidth));
            return new Rect { X = x, Y = verticalOffset, Size = ContentSize };
        }

        private static bool IsFooterPage(int horizontalPageNumber)
        {
            return horizontalPageNumber == 0;
        }

        protected virtual int GetVerticalPageCount()
        {
            int count;
            if (IsDrawingNotNull())
                count = (int)Math.Ceiling(GetDrawingBounds().Height / (PrintablePageHeight));
            else
            {
                throw new NullReferenceException(DrawingVisualNullMessage);
            }
            return count;
        }

        protected virtual Rect GetDrawingBounds()
        {
            return DrawingVisual.Drawing.Bounds;
        }

        protected virtual bool IsDrawingNotNull()
        {
            return DrawingVisual.Drawing != null;
        }

        public override DocumentPage GetPage(int pageNumber)
        {
            DrawingVisual pageVisual = GetPageVisual(pageNumber);
            var documentPage = new DocumentPage(pageVisual, PageSize, FrameRect, FrameRect);
            if (ShowPageMarkers)
                InsertPageMarkers(pageNumber + 1, documentPage);
            OnPageCreated(new PageEventArgs(pageNumber));
            return documentPage;
        }

        private DrawingVisual GetPageVisual(int pageNumber)
        {
            var totalHorizontalMargin = _originalMargin.Left + _originalMargin.Right;
            var toltalVerticalMargin = _originalMargin.Top + _originalMargin.Bottom;
            var printablePageWidth = PageSize.Width - totalHorizontalMargin;
            var printablePageHeight = PageSize.Height - toltalVerticalMargin - 10;

            var xFactor = printablePageWidth / PageSize.Width;
            var yFactor = printablePageHeight / PageSize.Height;
            var scaleFactor = Math.Max(xFactor, yFactor);
            var pageVisual = DrawingVisuals[pageNumber];
            var transformGroup = new TransformGroup();
            var scaleTransform = new ScaleTransform(scaleFactor, scaleFactor);
            var translateTransform = new TranslateTransform(_originalMargin.Left, _originalMargin.Top);
            transformGroup.Children.Add(translateTransform);
            transformGroup.Children.Add(scaleTransform);
            pageVisual.Transform = transformGroup;
            return pageVisual;
        }

        protected virtual void InsertPageMarkers(int pageNumber, DocumentPage documentPage)
        {
            var labelDrawingVisual = new DrawingVisual();
            using (var drawingContext = labelDrawingVisual.RenderOpen())
            {
                var pageNumberContent = pageNumber + "/" + PageCount;
                var ft = new FormattedText(pageNumberContent,
                                           Thread.CurrentThread.CurrentCulture,
                                           FlowDirection.LeftToRight,
                                           new Typeface("Verdana"),
                                           15, Brushes.Black);

                drawingContext.DrawText(ft, new Point(PageMargins.Left, PageMargins.Top));
            }

            var drawingGroup = new DrawingGroup();
            drawingGroup.Children.Add(((DrawingVisual)documentPage.Visual).Drawing);
            drawingGroup.Children.Add(labelDrawingVisual.Drawing);

            var currentDrawingVisual = (DrawingVisual)documentPage.Visual;
            using (var currentDrawingContext = currentDrawingVisual.RenderOpen())
            {
                currentDrawingContext.DrawDrawing(drawingGroup);
                currentDrawingContext.PushOpacityMask(Brushes.White);
            }
        }

        public override bool IsPageCountValid
        {
            get
            {
                return true;
            }
        }

        public override int PageCount
        {
            get
            {
                return _verticalPageCount * HorizontalPageCount;
            }
        }

        public override sealed Size PageSize
        {
            get { return _printSize; }
            set { }
        }

        public override IDocumentPaginatorSource Source
        {
            get
            {
                return _document;
            }
        }

        public void UpdatePageMarkers(bool showPageMarkers)
        {
            ShowPageMarkers = showPageMarkers;
        }

        public IDocumentPaginatorSource CreateDocumentPaginatorSource()
        {
            var document = new FixedDocument();
            for (var i = 0; i < PageCount; i++)
            {
                var page = GetPage(i);
                var fp = new FixedPage { ContentBox = FrameRect, BleedBox = FrameRect, Width = page.Size.Width, Height = page.Size.Height };

                var vb = new DrawingBrush(DrawingVisuals[i].Drawing)
                             {
                                 Stretch = Stretch.Uniform,
                                 ViewboxUnits = BrushMappingMode.Absolute,
                                 Viewbox = new Rect(page.Size)
                             };
                var totalHorizontalMargin = _originalMargin.Left + _originalMargin.Right;
                var toltalVerticalMargin = _originalMargin.Top + _originalMargin.Bottom;
                var printablePageWidth = PageSize.Width - totalHorizontalMargin;
                var printablePageHeight = PageSize.Height - toltalVerticalMargin - 10;

                var rect = new Rect(_originalMargin.Left, _originalMargin.Top, printablePageWidth, printablePageHeight);
                fp.Children.Add(CreateContentRectangle(vb, rect));
                var pageContent = new PageContent();
                ((IAddChild)pageContent).AddChild(fp);
                document.Pages.Add(pageContent);
            }
            _document = document;
            _document.DocumentPaginator.PageSize = new Size(PageSize.Width, PageSize.Height);
            return _document;
        }

        public List<FixedDocument> CreateFixedDocumentsForEachPage()
        {
            var documents = new List<FixedDocument>();
            for (var i = 0; i < PageCount; i++)
            {
                var document = new FixedDocument();
                var page = GetPage(i);
                var fp = new FixedPage { ContentBox = FrameRect, BleedBox = FrameRect, Width = page.Size.Width, Height = page.Size.Height };
                var vb = new DrawingBrush(DrawingVisuals[i].Drawing) { Stretch = Stretch.Uniform, ViewboxUnits = BrushMappingMode.Absolute, Viewbox = new Rect(page.Size) };

                var totalHorizontalMargin = _originalMargin.Left + _originalMargin.Right;
                var toltalVerticalMargin = _originalMargin.Top + _originalMargin.Bottom;
                var printablePageWidth = PageSize.Width - totalHorizontalMargin;
                var printablePageHeight = PageSize.Height - toltalVerticalMargin - 10;

                var rect = new Rect(_originalMargin.Left, _originalMargin.Top, printablePageWidth, printablePageHeight);
                fp.Children.Add(CreateContentRectangle(vb, rect));
                var pageContent = new PageContent();
                ((IAddChild)pageContent).AddChild(fp);
                document.Pages.Add(pageContent);
                documents.Add(document);
            }
            return documents;
        }

        public List<FixedDocument> CreateFixedDocumentsForEachPageWithPageNumber(int startPageNumber, double height, string slideName)
        {
            slideName = GetSlideNameForEntityChartHeader(slideName);
            var documents = new List<FixedDocument>();
            for (var i = 0; i < PageCount; i++)
            {
                var document = new FixedDocument();
                var page = GetPage(i);
                var fp = new FixedPage { ContentBox = FrameRect, BleedBox = FrameRect, Width = page.Size.Width, Height = page.Size.Height };
                var vb = new DrawingBrush(DrawingVisuals[i].Drawing) { Stretch = Stretch.Uniform, ViewboxUnits = BrushMappingMode.Absolute, Viewbox = new Rect(page.Size) };

                var totalHorizontalMargin = _originalMargin.Left + _originalMargin.Right;
                var toltalVerticalMargin = _originalMargin.Top + _originalMargin.Bottom;
                var printablePageWidth = PageSize.Width - totalHorizontalMargin;
                var printablePageHeight = height - toltalVerticalMargin - 10;

                var rect = new Rect(_originalMargin.Left, _originalMargin.Top + Constants.CsBook.EntityChartPageHeaderSize, printablePageWidth, printablePageHeight);
                fp.Children.Add(CreateContentRectangle(vb, rect));

                InsertEntityChartPageHeader(fp, startPageNumber++, slideName, i + 1);

                var pageContent = new PageContent();
                ((IAddChild)pageContent).AddChild(fp);
                document.Pages.Add(pageContent);
                documents.Add(document);
            }
            return documents;
        }

        private string GetSlideNameForEntityChartHeader(string slideName)
        {
            var entityChartSlideNameMaxSize = PageSize.Width - Constants.CsBook.PageNumberTextLength - 10;
            if (slideName.Length > entityChartSlideNameMaxSize)
            {
                return string.Format("{0}...", slideName.Substring(0, (int)entityChartSlideNameMaxSize));
            }
            return slideName;
        }

        private void InsertEntityChartPageHeader(FixedPage fp, int pageNumber, string slideName, int pageIndex)
        {
            var slideNameAvailableWidth = PageSize.Width - 2 * Constants.CsBook.PageNumberTextLength;

            var pageNumberLabel = new TextBlock { Text = string.Format("Page {0}", pageNumber) };
            FixedPage.SetLeft(pageNumberLabel, PageSize.Width - Constants.CsBook.PageNumberTextLength);
            FixedPage.SetTop(pageNumberLabel, 10);
            fp.Children.Add(pageNumberLabel);

            var noOfTotal = string.Format("({0} of {1})", pageIndex, PageCount);
            const int noOfTotalFontSize = 10;
            var ft1 = new FormattedText(noOfTotal, Thread.CurrentThread.CurrentCulture,
                                       FlowDirection.LeftToRight, new Typeface("Verdana"), noOfTotalFontSize, Brushes.Black);
            var pageNumberTextWidth = ft1.Width;
            var availableSlideNameWidth = slideNameAvailableWidth - pageNumberTextWidth;
            ft1 = new FormattedText(slideName, Thread.CurrentThread.CurrentCulture,
                                       FlowDirection.LeftToRight, new Typeface("Verdana"), 20, Brushes.Black);
            var slideNameTextWidth = ft1.Width;
            var txtBlockSlideNameWidth = (slideNameTextWidth > availableSlideNameWidth)
                                             ? availableSlideNameWidth
                                             : slideNameTextWidth;
            var stackPanel = new StackPanel { Orientation = Orientation.Horizontal};
            var txtBlockSlideName = new TextBlock
                                        {
                                            Text = slideName,
                                            Width = Math.Max(0, txtBlockSlideNameWidth),
                                            FontFamily = new FontFamily("Verdana"),
                                            FontSize = 20,
                                            TextTrimming = TextTrimming.CharacterEllipsis,
                                            Padding = new Thickness(0),
                                            Margin = new Thickness(0),
                                            VerticalAlignment = VerticalAlignment.Bottom
                                        };
            var txtBlockPageNo = new TextBlock
                                     {
                                         Text = noOfTotal,
                                         Width = pageNumberTextWidth,
                                         FontFamily = new FontFamily("Verdana"),
                                         FontSize = noOfTotalFontSize,
                                         TextTrimming = TextTrimming.None,
                                         VerticalAlignment = VerticalAlignment.Bottom,
                                         Padding = new Thickness(0,0,0,2),
                                         Margin = new Thickness(0)
                                     };
            stackPanel.Children.Add(txtBlockSlideName);
            stackPanel.Children.Add(txtBlockPageNo);
            var label = new Label
                            {
                                Width = slideNameAvailableWidth,
                                Content = stackPanel,
                                HorizontalContentAlignment = HorizontalAlignment.Center,
                                Padding = new Thickness(0),
                                Margin = new Thickness(0)
                            };


            FixedPage.SetLeft(label, Constants.CsBook.PageNumberTextLength);
            FixedPage.SetTop(label, 10);
            fp.Children.Add(label);
        }

        private static Rectangle CreateContentRectangle(Brush vb, Rect rect)
        {
            var rc = new Rectangle { Width = rect.Width, Height = rect.Height, Fill = vb };
            FixedPage.SetLeft(rc, rect.X);
            FixedPage.SetTop(rc, rect.Y);
            FixedPage.SetRight(rc, rect.Width);
            FixedPage.SetBottom(rc, rect.Height);
            return rc;
        }

        protected virtual void CreatePageVisual(Rect pageBounds, DrawingVisual source, bool isFooterPage, DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(null, FramePen, new Rect { X = FrameRect.X, Y = FrameRect.Y, Width = FrameRect.Width, Height = FrameRect.Height });
            var offsetX = PageMargins.Left - pageBounds.X - 1;
            var offsetY = PageMargins.Top - pageBounds.Y;
            drawingContext.PushTransform(new TranslateTransform(offsetX, offsetY + HeaderHeight));
            var pg = new Rect(new Point(pageBounds.X, pageBounds.Y), new Size(pageBounds.Width, pageBounds.Height));
            drawingContext.PushClip(new RectangleGeometry(pg));
            drawingContext.PushOpacityMask(Brushes.White);
            drawingContext.DrawDrawing(source.Drawing);
        }

        protected virtual int GetHorizontalPageCount()
        {
            if (IsDrawingNotNull())
                return (int)Math.Ceiling(GetDrawingBounds().Width / PrintablePageWidth);
            throw new NullReferenceException(DrawingVisualNullMessage);
        }

        public FixedDocument GetDocument(int startIndex, int endIndex)
        {
            var document = new FixedDocument();
            for (var i = startIndex; i < endIndex; i++)
            {
                var fp = new FixedPage { ContentBox = FrameRect, BleedBox = FrameRect };
                var page = GetPage(i);
                var vb = new DrawingBrush(DrawingVisuals[i].Drawing) { Stretch = Stretch.Uniform, ViewboxUnits = BrushMappingMode.Absolute, Viewbox = new Rect(page.Size) };
                fp.Children.Add(CreateContentRectangle(vb, FrameRect));
                var pageContent = new PageContent();
                ((IAddChild)pageContent).AddChild(fp);
                document.Pages.Add(pageContent);
            }
            document.DocumentPaginator.PageSize = new Size(PageSize.Width, PageSize.Height);
            return document;
        }
    }

    public class PageEventArgs : EventArgs
    {
        private readonly int _pageNumber;
        public int PageNumber { get { return _pageNumber; }}

        public PageEventArgs(int pageNumber)
        {
            _pageNumber = pageNumber;
        }
    }
}