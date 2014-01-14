using System;
using System.Data;
using System.Drawing.Printing;
using System.Printing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Practices.Unity;
using SUT.PrintEngine.Extensions;
using SUT.PrintEngine.Paginators;
using SUT.PrintEngine.Utils;
using SUT.PrintEngine.Views;

namespace SUT.PrintEngine.ViewModels
{
    public class PrintControlViewModel : APrintControlViewModel, IPrintControlViewModel
    {
        #region Commands

        public DrawingVisual DrawingVisual { get; set; }

        public ICommand ResizeCommand { get; set; }
        public ICommand ApplyScaleCommand { get; set; }
        public ICommand CancelScaleCommand { get; set; }
        #endregion

        #region Dependency Properties
        public double Scale
        {
            get
            {
                return (double)GetValue(ScaleProperty);
            }
            set
            {
                SetValue(ScaleProperty, value);
            }
        }

        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(
            "Scale",
            typeof(double),
            typeof(PrintControlViewModel),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));

        private bool _isCancelPrint;

        #endregion

        public double ScaleFactor { get; set; }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PrintControlViewModel)d).HandlePropertyChanged(d, e);
        }

        public PrintControlViewModel(PrintControlView view, IUnityContainer unityContainer)
            : base(view, unityContainer)
        {

            ResizeCommand = new DelegateCommand<object>(ExecuteResize);
            ApplyScaleCommand = new DelegateCommand<object>(ExecuteApplyScale);
            CancelScaleCommand = new DelegateCommand<object>(ExecuteCancelScale);
            PrintControlView.ResizeButtonVisibility(true);
            PrintControlView.SetPageNumberVisibility(Visibility.Visible);
        }

        public void ExecuteResize(object parameter)
        {
            PrintControlView.ScalePreviewPaneVisibility(true);
        }
        private void ExecuteCancelScale(object parameter)
        {
            ScaleCanceling = true;
            Scale = OldScale;
            PrintControlView.ScalePreviewPaneVisibility(false);
            ScaleCanceling = false;
        }

        private void ExecuteApplyScale(object parameter)
        {
            OldScale = Scale;
            PrintControlView.ScalePreviewPaneVisibility(false);
            ReloadPreview();
        }

        public override void InitializeProperties()
        {
            ResetScale();
            base.InitializeProperties();
        }

        private void ResetScale()
        {
            OldScale = 1;
            Scale = 1;
            PrintControlView.ScalePreviewPaneVisibility(false);
        }

        public override void ReloadPreview()
        {
            if (CurrentPaper != null)
                ReloadPreview(Scale, new Thickness(), PageOrientation, CurrentPaper);
        }

        public void ReloadPreview(double scale, Thickness margin, PageOrientation pageOrientation, PaperSize paperSize)
        {
            try
            {
                ReloadingPreview = true;
                ShowWaitScreen();
                var printSize = GetPrintSize(paperSize, pageOrientation);
                var visual = GetScaledVisual(scale);
                CreatePaginator(visual, printSize);
                var visualPaginator = ((VisualPaginator)Paginator);
                visualPaginator.Initialize(IsMarkPageNumbers);
                PagesAcross = visualPaginator.HorizontalPageCount;

                ApproaxNumberOfPages = MaxCopies = Paginator.PageCount;
                if (Scale == 1)
                    NumberOfPages = ApproaxNumberOfPages;

                DisplayPagePreviewsAll(visualPaginator);
                ReloadingPreview = false;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                WaitScreen.Hide();
            }

        }

        private DrawingVisual GetScaledVisual(double scale)
        {
            if (scale == 1)
                return DrawingVisual;
            var visual = new DrawingVisual();
            using (var dc = visual.RenderOpen())
            {
                dc.PushTransform(new ScaleTransform(scale, scale));
                dc.DrawDrawing(DrawingVisual.Drawing);
            }
            return visual;
        }

        private static Size GetPrintSize(PaperSize paperSize, PageOrientation pageOrientation)
        {
            var printSize = new Size(paperSize.Width, paperSize.Height);
            if (pageOrientation == PageOrientation.Landscape)
            {
                printSize = new Size(paperSize.Height, paperSize.Width);
            }
            return printSize;
        }

        private void ShowWaitScreen()
        {
            if (FullScreenPrintWindow != null)
            {
                WaitScreen.Show("Loading...");
            }
        }

        protected  virtual void CreatePaginator(DrawingVisual visual, Size printSize)
        {
            Paginator = new VisualPaginator(visual, printSize, new Thickness(), PrintUtility.GetPageMargin(CurrentPrinterName));
        }

        public override void HandlePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var presenter = (PrintControlViewModel)o;
            switch (e.Property.Name)
            {
                case "Scale":
                    if (presenter.ScaleCanceling)
                        return;
                    ((IPrintControlView)presenter.View).ScalePreviewNode(new ScaleTransform(presenter.Scale, presenter.Scale));
                    presenter.ApproaxNumberOfPages = Convert.ToInt32(Math.Ceiling(presenter.NumberOfPages * presenter.Scale));
                    break;
            }
            base.HandlePropertyChanged(o, e);
        }

        public override void ExecutePrint(object parameter)
        {
            try
            {
                var printDialog = new System.Windows.Controls.PrintDialog();
                printDialog.PrintQueue = CurrentPrinter;
                printDialog.PrintTicket = CurrentPrinter.UserPrintTicket;
                ShowProgressDialog();
                ((VisualPaginator)Paginator).PageCreated += PrintControlPresenterPageCreated;
                printDialog.PrintDocument(Paginator, "");
            }
            catch (Exception)
            {
            }
            finally
            {
                ProgressDialog.Hide();
            }
        }

        private void PrintControlPresenterPageCreated(object sender, PageEventArgs e)
        {
            ProgressDialog.CurrentProgressValue = e.PageNumber;
            ProgressDialog.Message = GetStatusMessage();
            Application.Current.DoEvents();
        }

        public override void SetProgressDialogCancelButtonVisibility()
        {
            ProgressDialog.CancelButtonVisibility = Visibility.Visible;
        }

        public void ShowPrintPreview()
        {
            if (FullScreenPrintWindow != null)
            {
                FullScreenPrintWindow.Content = null;
            }
            CreatePrintPreviewWindow();
            Loading = true;
            IsSetPrintingOptionsEnabled = false;
            IsCancelPrintingOptionsEnabled = false;
            if (FullScreenPrintWindow != null) FullScreenPrintWindow.ShowDialog();
            ApplicationExtention.MainWindow = null;
        }

        public void ShowPrintPreview(DataTable dataTable)
        {
            DataTableUtil.Validate(dataTable);
        }
        
        public static IPrintControlViewModel Create()
        {
            var unityContainer = new UnityContainer();
            PrintEngineModule.Initialize(unityContainer);
            return unityContainer.Resolve<IPrintControlViewModel>();
        }

        
    }
}