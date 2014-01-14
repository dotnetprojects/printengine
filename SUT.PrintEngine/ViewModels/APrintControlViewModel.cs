using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;
using System.Printing;
using System.Printing.Interop;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Practices.Unity;
using SUT.PrintEngine.Controls.ProgressDialog;
using SUT.PrintEngine.Controls.WaitScreen;
using SUT.PrintEngine.Extensions;
using SUT.PrintEngine.Utils;
using SUT.PrintEngine.Views;
using Application = System.Windows.Application;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Orientation = System.Windows.Controls.Orientation;

namespace SUT.PrintEngine.ViewModels
{
    public abstract class APrintControlViewModel : AViewModel, IViewModel
    {
        protected DocumentPaginator Paginator;
        protected IUnityContainer UnityContainer { get; set; }
        protected IWaitScreenViewModel WaitScreen { get; set; }
        protected IProgressDialogViewModel ProgressDialog { get; set; }
        public ICommand ChangePaperCommand { get; set; }
        protected bool ScaleCanceling;

        public double OldScale { get; set; }

        [DllImport("winspool.Drv", EntryPoint = "DocumentPropertiesW", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        static extern int DocumentProperties(IntPtr hwnd, IntPtr hPrinter, [MarshalAs(UnmanagedType.LPWStr)] string pDeviceName, IntPtr pDevModeOutput, IntPtr pDevModeInput, int fMode);


        #region Dependency Properties
        public static readonly DependencyProperty IsPotraitProperty = DependencyProperty.Register(
            "IsPotrait",
            typeof(bool),
            typeof(APrintControlViewModel),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnDependencyPropertyChanged)));

        public static readonly DependencyProperty IsLandscapeProperty = DependencyProperty.Register(
            "IsLandscape",
            typeof(bool),
            typeof(APrintControlViewModel),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnDependencyPropertyChanged)));

        public static readonly DependencyProperty IsAdvancedPrintingOptionOpenProperty = DependencyProperty.Register(
            "IsAdvancedPrintingOptionOpen",
            typeof(bool),
            typeof(APrintControlViewModel),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnDependencyPropertyChanged)));

        public static readonly DependencyProperty IsMarkPageNumbersProperty = DependencyProperty.Register(
            "IsMarkPageNumbers",
            typeof(bool),
            typeof(APrintControlViewModel),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDependencyPropertyChanged)));

        public static readonly DependencyProperty PageOrientationProperty = DependencyProperty.Register(
            "PageOrientation",
            typeof(PageOrientation),
            typeof(APrintControlViewModel),
            new FrameworkPropertyMetadata(PageOrientation.Portrait, new PropertyChangedCallback(OnDependencyPropertyChanged)));
        public static readonly DependencyProperty PageOrientationStringProperty = DependencyProperty.Register(
            "PageOrientationString",
            typeof(string),
            typeof(APrintControlViewModel),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDependencyPropertyChanged)));
        public static readonly DependencyProperty ApproaxNumberOfPagesProperty = DependencyProperty.Register(
            "ApproaxNumberOfPages",
            typeof(Int32),
            typeof(APrintControlViewModel),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDependencyPropertyChanged)));

        public static readonly DependencyProperty PrintCopyCountProperty = DependencyProperty.Register(
            "PrintCopyCount",
            typeof(int),
            typeof(APrintControlViewModel),
            new FrameworkPropertyMetadata(1, new PropertyChangedCallback(OnDependencyPropertyChanged)));

        public static readonly DependencyProperty PaperSizesProperty = DependencyProperty.Register(
            "PaperSizes",
            typeof(IList<PaperSize>),
            typeof(APrintControlViewModel),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDependencyPropertyChanged)));

        public static readonly DependencyProperty CurrentPaperProperty = DependencyProperty.Register(
            "CurrentPaper",
            typeof(PaperSize),
            typeof(APrintControlViewModel),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDependencyPropertyChanged)));

        public static readonly DependencyProperty DefaultPaperSizeProperty = DependencyProperty.Register(
            "DefaultPaperSize",
            typeof(PaperSize),
            typeof(APrintControlViewModel),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDependencyPropertyChanged)));

        public static readonly DependencyProperty CurrentPrinterNameProperty = DependencyProperty.Register(
            "CurrentPrinterName",
            typeof(string),
            typeof(APrintControlViewModel),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDependencyPropertyChanged)));

        public static readonly DependencyProperty CurrentPrinterProperty = DependencyProperty.Register(
            "CurrentPrinter",
            typeof(PrintQueue),
            typeof(APrintControlViewModel),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDependencyPropertyChanged)));

        public static readonly DependencyProperty PrintersProperty = DependencyProperty.Register(
            "Printers",
            typeof(PrintQueueCollection),
            typeof(APrintControlViewModel),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDependencyPropertyChanged)));

        public static readonly DependencyProperty PagesAcrossProperty = DependencyProperty.Register(
            "PagesAcross",
            typeof(Int32),
            typeof(APrintControlViewModel),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDependencyPropertyChanged)));

        public static readonly DependencyProperty NumberOfPagesProperty = DependencyProperty.Register(
            "NumberOfPages",
            typeof(Int32),
            typeof(APrintControlViewModel),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDependencyPropertyChanged)));
        public static readonly DependencyProperty IsPrintingOptionsOpenProperty = DependencyProperty.Register(
            "IsPrintingOptionsOpen",
            typeof(bool),
            typeof(APrintControlViewModel));
        public static readonly DependencyProperty IsCancelPrintingOptionsEnabledProperty = DependencyProperty.Register(
            "IsCancelPrintingOptionsEnabled",
            typeof(bool),
            typeof(APrintControlViewModel));
        public static readonly DependencyProperty IsSetPrintingOptionsEnabledProperty = DependencyProperty.Register(
            "IsSetPrintingOptionsEnabled",
            typeof(bool),
            typeof(APrintControlViewModel));
        public static readonly DependencyProperty CanScaleProperty = DependencyProperty.Register(
            "CanScale",
            typeof(bool),
            typeof(APrintControlViewModel),
            new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty PrinterErrorVisibilityProperty = DependencyProperty.Register(
            "PrinterErrorVisibility",
            typeof(Visibility),
            typeof(APrintControlViewModel));
        public Visibility PrinterErrorVisibility
        {
            get
            {
                return (Visibility)GetValue(PrinterErrorVisibilityProperty);
            }
            set
            {
                SetValue(PrinterErrorVisibilityProperty, value);
            }
        }
        public bool CanScale
        {
            get
            {
                return (bool) GetValue(CanScaleProperty);
            }
            set
            {
                SetValue(CanScaleProperty, value);
            }
        }
        public bool IsPotrait
        {
            get
            {
                return (bool)GetValue(IsPotraitProperty);
            }
            set
            {
                SetValue(IsPotraitProperty, value);
            }
        }
        public bool IsLandscape
        {
            get
            {
                return (bool)GetValue(IsLandscapeProperty);
            }
            set
            {
                SetValue(IsLandscapeProperty, value);
            }
        }
        public bool IsAdvancedPrintingOptionOpen
        {
            get
            {
                return (bool)GetValue(IsAdvancedPrintingOptionOpenProperty);
            }
            set
            {
                SetValue(IsAdvancedPrintingOptionOpenProperty, value);
            }
        }
        public bool IsMarkPageNumbers
        {
            get
            {
                return (bool)GetValue(IsMarkPageNumbersProperty);
            }
            set
            {
                SetValue(IsMarkPageNumbersProperty, value);
            }
        }
        public bool IsPrintingOptionsOpen
        {
            get
            {
                return (bool)GetValue(IsPrintingOptionsOpenProperty);
            }
            set
            {
                SetValue(IsPrintingOptionsOpenProperty, value);
            }
        }
        public bool IsCancelPrintingOptionsEnabled
        {
            get
            {
                return (bool)GetValue(IsCancelPrintingOptionsEnabledProperty);
            }
            set
            {
                SetValue(IsCancelPrintingOptionsEnabledProperty, value);
            }
        }
        public bool IsSetPrintingOptionsEnabled
        {
            get
            {
                return (bool)GetValue(IsSetPrintingOptionsEnabledProperty);
            }
            set
            {
                SetValue(IsSetPrintingOptionsEnabledProperty, value);
            }
        }
        public string PageOrientationString
        {
            get
            {
                return (string)GetValue(PageOrientationStringProperty);
            }
            set
            {
                SetValue(PageOrientationStringProperty, value);
            }
        }
        public PageOrientation PageOrientation
        {
            get
            {
                return (PageOrientation)GetValue(PageOrientationProperty);
            }
            set
            {
                SetValue(PageOrientationProperty, value);
                PageOrientationString = PageOrientation.ToString();
            }
        }

        public int ApproaxNumberOfPages
        {
            get
            {
                return (int)GetValue(ApproaxNumberOfPagesProperty);
            }
            set
            {
                SetValue(ApproaxNumberOfPagesProperty, value);
            }
        }

        public int PrintCopyCount
        {
            get
            {
                return (int)GetValue(PrintCopyCountProperty);
            }
            set
            {
                SetValue(PrintCopyCountProperty, value);
            }
        }

        public IList<PaperSize> PaperSizes
        {
            get
            {
                return (IList<PaperSize>)GetValue(PaperSizesProperty);
            }
            set
            {
                SetValue(PaperSizesProperty, value);
            }
        }

        public PaperSize CurrentPaper
        {
            get
            {
                return (PaperSize)GetValue(CurrentPaperProperty);
            }
            set
            {
                SetValue(CurrentPaperProperty, value);
            }
        }

        public PaperSize DefaultPaperSize
        {
            get
            {
                return (PaperSize)GetValue(DefaultPaperSizeProperty);
            }
            set
            {
                SetValue(DefaultPaperSizeProperty, value);
            }
        }

        public string CurrentPrinterName
        {
            get
            {
                return GetValue(CurrentPrinterNameProperty).ToString();
            }
            set
            {
                SetValue(CurrentPrinterNameProperty, value);
            }
        }

        public PrintQueue CurrentPrinter
        {
            get
            {
                return (PrintQueue)GetValue(CurrentPrinterProperty);
            }
            set
            {
                SetValue(CurrentPrinterProperty, value);
            }
        }

        public PrintQueueCollection Printers
        {
            get
            {
                return (PrintQueueCollection)GetValue(PrintersProperty);
            }
            set
            {
                SetValue(PrintersProperty, value);
            }
        }

        public int PagesAcross
        {
            get
            {
                return (int)GetValue(PagesAcrossProperty);
            }
            set
            {
                SetValue(PagesAcrossProperty, value);
            }
        }

        public int NumberOfPages
        {
            get
            {
                return (int)GetValue(NumberOfPagesProperty);
            }
            set
            {
                SetValue(NumberOfPagesProperty, value);
            }
        }
        #endregion

        #region Member Variables
        public int MaxCopies;
        private bool IsMarkPageNumbersChanged;
        private bool IsPageOrientationChanged;
        private bool IsPrintCopyCountChanged;
        private bool IsCurrentPaperChanged;
        private bool IsCurrentPrinterChanged;
        private bool IsCurrentPrinterNameChanged;

        #endregion

        private static void OnDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var presenter = ((APrintControlViewModel)d);
            if (!presenter._settingOptions)
            {
                switch (e.Property.Name)
                {
                    case "IsMarkPageNumbers":
                        presenter.PrintOptionsSetterIsEnable(true);
                        if (e.OldValue != null && !presenter.IsMarkPageNumbersChanged)
                        {
                            presenter.IsMarkPageNumbersChanged = true;
                            presenter._oldPrintingOptions.IsMarkPageNumbers = (bool)e.OldValue;
                        }
                        presenter._newPrintingOptions.IsMarkPageNumbers = (bool)e.NewValue;
                        break;
                    case "PageOrientation":
                        presenter.PrintOptionsSetterIsEnable(true);
                        if (e.OldValue != null && !presenter.IsPageOrientationChanged)
                        {
                            presenter.IsPageOrientationChanged = true;
                            presenter._oldPrintingOptions.PageOrientation = (PageOrientation)e.OldValue;
                        }
                        presenter._newPrintingOptions.PageOrientation = (PageOrientation)e.NewValue;
                        break;
                    case "CurrentPaper":
                        presenter.PrintOptionsSetterIsEnable(true);
                        if (e.OldValue != null && !presenter.IsCurrentPaperChanged)
                        {
                            presenter.IsCurrentPaperChanged = true;
                            presenter._oldPrintingOptions.CurrentPaper = (PaperSize)e.OldValue;
                        }
                        presenter._newPrintingOptions.CurrentPaper = (PaperSize)e.NewValue;
                        break;
                    case "PrintCopyCount":
                        presenter.PrintOptionsSetterIsEnable(true);
                        if (e.OldValue != null && !presenter.IsPrintCopyCountChanged)
                        {
                            presenter.IsPrintCopyCountChanged = true;
                            presenter._oldPrintingOptions.PrintCopyCount = (int)e.OldValue;
                        }
                        presenter._newPrintingOptions.PrintCopyCount = (int)e.NewValue;
                        break;
                    case "CurrentPrinter":
                        if (e.NewValue != null)
                        {
                            presenter.PrintOptionsSetterIsEnable(true);
                            try
                            {
                                if (e.OldValue != null && !presenter.IsCurrentPrinterChanged)
                                {
                                    presenter.IsCurrentPrinterChanged = true;
                                    presenter._oldPrintingOptions.CurrentPrinter = (PrintQueue)e.OldValue;
                                }
                                presenter.FetchSetting();
                                presenter._newPrintingOptions.CurrentPrinter = (PrintQueue)e.NewValue;
                                presenter.SetPrintError(false);
                            }
                            catch (Exception)
                            {
                                presenter.SetPrintError(true);
                            }

                        }
                        break;
                    case "CurrentPrinterName":
                        presenter.PrintOptionsSetterIsEnable(true);
                        if (e.OldValue != null && !presenter.IsCurrentPrinterNameChanged)
                        {
                            presenter.IsCurrentPrinterNameChanged = true;
                            presenter._oldPrintingOptions.CurrentPrinterName = (string)e.OldValue;
                        }
                        presenter._newPrintingOptions.CurrentPrinterName = (string)e.NewValue;
                        break;

                }
            }
            else
            {
                if (e.Property.Name == "CurrentPrinter")
                {
                    var currentPrinter = e.NewValue as PrintQueue;
                    if (currentPrinter != null)
                    {
                        try
                        {
                            presenter.FetchSetting();
                            presenter.SetPrintError(false);
                        }
                        catch (Exception)
                        {
                            presenter.SetPrintError(true);
                        }
                    }
                }
            }
        }

        private void SetPrintError(bool isError)
        {
            if (isError)
            {
                PrinterErrorVisibility = Visibility.Visible;
                ((PrintControlView)View).EnablePrintingOptionsSet(false);
                ((PrintControlView)View).PrintingOptionsWaitCurtainVisibility(false);
            }
            else
            {
                PrinterErrorVisibility = Visibility.Hidden;
                ((PrintControlView)View).EnablePrintingOptionsSet(true);
            }

        }

        public virtual void HandlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        #region Class Members

        public PrintQueue LocalPrinter;
        public Window FullScreenPrintWindow;
        private PrintingOptions _oldPrintingOptions;
        private PrintingOptions _newPrintingOptions;
        private bool _settingOptions;
        public bool Loading;
        public bool ReloadingPreview;
        public ICommand PrintDocumentCommand { get; set; }
        public ICommand PrintSetupCommand { get; set; }
        public ICommand CancelPrintCommand { get; set; }
        public ICommand PageOrientationCommand { get; set; }
        public ICommand SetPrintingOptionsCommand { get; set; }
        public ICommand CancelPrintingOptionsCommand { get; set; }
        public ICommand MarkPageNumbersCommand { get; set; }
        public ICommand AllPagesCommand { get; set; }
        public ICommand ActualPageSizeCommand { get; set; }
        #endregion

        protected APrintControlViewModel(PrintControlView view, IUnityContainer unityContainer)
            : base(view)
        {
            PrintControlView = view;
            PrintControlView.Loaded += PrintControlViewLoaded;
            _oldPrintingOptions = new PrintingOptions();
            _newPrintingOptions = new PrintingOptions();
            UnityContainer = unityContainer;
            WaitScreen = UnityContainer.Resolve<IWaitScreenViewModel>();
            PrintUtility = unityContainer.Resolve<PrintUtility>();
            CancelPrintCommand = new DelegateCommand<object>(ExecuteCancelPrint);
            PrintDocumentCommand = new DelegateCommand<object>(ExecutePrint);
            PageOrientationCommand = new DelegateCommand<object>(ExecutePageOrientation);
            SetPrintingOptionsCommand = new DelegateCommand<object>(ExecuteSetPrintingOptions);
            CancelPrintingOptionsCommand = new DelegateCommand<object>( ExecuteCancelPrintingOptions);
            MarkPageNumbersCommand = new DelegateCommand<object>(ExecuteMarkPageNumbers);
            AllPagesCommand = new DelegateCommand<object>(ExecuteAllPages);
            ActualPageSizeCommand = new DelegateCommand<object>(ExecuteActualPageSizeCommand);
            ChangePaperCommand = new DelegateCommand<object>(ExecuteChangePaper);

        }

        private void ExecuteChangePaper(object obj)
        {
            try
            {
                var ptc = new PrintTicketConverter(CurrentPrinter.FullName, CurrentPrinter.ClientPrintSchemaVersion);
                var mainWindowPtr = new WindowInteropHelper(FullScreenPrintWindow).Handle;
                var myDevMode = ptc.ConvertPrintTicketToDevMode(CurrentPrinter.UserPrintTicket, BaseDevModeType.UserDefault);
                var pinnedDevMode = GCHandle.Alloc(myDevMode, GCHandleType.Pinned);
                var pDevMode = pinnedDevMode.AddrOfPinnedObject();
                var result = DocumentProperties(mainWindowPtr, IntPtr.Zero, CurrentPrinter.FullName, pDevMode, pDevMode, 14);
                if (result == 1)
                {
                    CurrentPrinter.UserPrintTicket = ptc.ConvertDevModeToPrintTicket(myDevMode);
                    pinnedDevMode.Free();
                    PrintCopyCount = CurrentPrinter.UserPrintTicket.CopyCount.Value;
                    SetPageOrientation(CurrentPrinter.UserPrintTicket.PageOrientation);
                    SetCurrentPaper(CurrentPrinter.UserPrintTicket.PageMediaSize);
                    ExecuteSetPrintingOptions(null);
                }
            }
            catch (Exception)
            {

            }
        }

        public void ExecutePageOrientation(object parameter)
        {
            PageOrientation = parameter.ToString() == "Portrait" ? PageOrientation.Portrait : PageOrientation.Landscape;
        }

        protected virtual void ExecuteActualPageSizeCommand(object obj)
        {
            ShowAllPages = false;
            ReloadPreview();
        }

        public abstract void ReloadPreview();

        public virtual void InitializeProperties()
        {
            try
            {
                Printers = PrintUtility.GetPrinters();
                SetLocalPrinter();
                var defaultPrintQueue = PrintUtility.GetDefaultPrintQueue(string.Empty);
                IsMarkPageNumbers = true;
                var defaultPrinterFullName = defaultPrintQueue.FullName;
                var defaultExists = false;
                foreach (var printer in Printers)
                {
                    if (printer.Name == defaultPrintQueue.Name)
                    {
                        defaultExists = true;
                        break;
                    }
                }
                if (!defaultExists)
                {
                    Printers.Add(defaultPrintQueue);
                    var temp = Printers;
                    Printers = null;
                    Printers = temp;
                }
                CurrentPrinterName = defaultPrinterFullName;
                CurrentPrinter = Printers.First(e => e.FullName == defaultPrinterFullName);
                PrintOptionsSetterIsEnable(false);
                SetPrintError(false);
                var userPrintTicket = PrintUtility.GetUserPrintTicket(CurrentPrinter.FullName);
                if(userPrintTicket != null)
                    CurrentPrinter.UserPrintTicket = userPrintTicket;
                SetCurrentPaper(CurrentPrinter.UserPrintTicket.PageMediaSize);
                SetPageOrientation(CurrentPrinter.UserPrintTicket.PageOrientation);
                ExecuteSetPrintingOptions(false);
            }
            catch (Exception ex)
            {
                SetPrintError(true);
            }

        }

        private void SetLocalPrinter()
        {
            foreach (var printer in Printers)
            {
                if (printer.HostingPrintServer.Name.Contains(SystemInformation.ComputerName))
                {
                    LocalPrinter = printer;
                    break;
                }
            }
        }

        #region Command Execution
        public virtual void ExecuteAllPages(object parameter)
        {
            ShowAllPages = true;
            ReloadPreview();
        }

        private void SetPageOrientation(PageOrientation? pageOrientation)
        {
            if (pageOrientation == PageOrientation.Portrait && PageOrientation != PageOrientation.Portrait)
            {
                PageOrientation = PageOrientation.Portrait;
            }
            if (pageOrientation == PageOrientation.Landscape && PageOrientation != PageOrientation.Landscape)
            {
                PageOrientation = PageOrientation.Landscape;
            }
        }

        private void SetCurrentPaper(PageMediaSize pageMediaSize)
        {
            var widthInInch = Math.Round(pageMediaSize.Width.Value / 96 * 100);
            var heightInInch = Math.Round(pageMediaSize.Height.Value / 96 * 100);
            var paperSize = PaperSizes.FirstOrDefault(p => p.Width == widthInInch && p.Height == heightInInch);
            if (paperSize != null)
                CurrentPaper = PaperSizes[PaperSizes.IndexOf(paperSize)];
        }

        public void ExecuteSetPrintingOptions(object parameter)
        {
            _settingOptions = true;
            if (IsPrintCopyCountChanged)
            {
                PrintCopyCount = _newPrintingOptions.PrintCopyCount;
                CurrentPrinter.UserPrintTicket.CopyCount = PrintCopyCount;
            }
            if (IsMarkPageNumbersChanged)
            {
                IsMarkPageNumbers = _newPrintingOptions.IsMarkPageNumbers;
            }

            if (IsPageOrientationChanged)
            {
                PageOrientation = _newPrintingOptions.PageOrientation;
                if (PageOrientation == PageOrientation.Portrait)
                {
                    ((PrintControlView)View).Portrait.IsChecked = true;
                }
                else
                {
                    ((PrintControlView)View).Landscape.IsChecked = true;
                }
                SetupPrintOrientation(PageOrientation);
            }
            if (IsCurrentPaperChanged)
            {
                CurrentPaper = _newPrintingOptions.CurrentPaper;
            }

            PrintUtility.SaveUserPrintTicket(CurrentPrinter);

            ResetPrintingOptions();
            ReloadPreview();
        }

        public void ExecuteCancelPrintingOptions(object parameter)
        {
            _settingOptions = true;
            if (IsPrintCopyCountChanged)
                PrintCopyCount = _oldPrintingOptions.PrintCopyCount;
            if (IsMarkPageNumbersChanged)
                IsMarkPageNumbers = _oldPrintingOptions.IsMarkPageNumbers;
            if (IsPageOrientationChanged)
            {
                PageOrientation = _oldPrintingOptions.PageOrientation;
                if (PageOrientation == PageOrientation.Portrait)
                    ((PrintControlView)View).Portrait.IsChecked = true;
                else
                    ((PrintControlView)View).Landscape.IsChecked = true;
            }
            if (IsCurrentPrinterChanged)
            {
                PaperSizes = _oldPrintingOptions.PaperSizes;
                CurrentPrinter = _oldPrintingOptions.CurrentPrinter;
                CurrentPrinterName = _oldPrintingOptions.CurrentPrinterName;
                CurrentPaper = _oldPrintingOptions.CurrentPaper;
            }
            ResetPrintingOptions();
            _settingOptions = false;
        }

        public void ResetPrintingOptions()
        {
            _settingOptions = true;
            IsSetPrintingOptionsEnabled = false;
            IsCancelPrintingOptionsEnabled = false;
            IsAdvancedPrintingOptionOpen = false;
            _oldPrintingOptions = new PrintingOptions();
            _newPrintingOptions = new PrintingOptions();
            IsMarkPageNumbersChanged = false;
            IsPageOrientationChanged = false;
            IsPrintCopyCountChanged = false;
            IsCurrentPaperChanged = false;
            IsCurrentPrinterChanged = false;
            IsCurrentPrinterNameChanged = false;
            _settingOptions = false;
            PrintOptionsSetterIsEnable(false);
            ((PrintControlView)View).EnablePrintingOptionsSet(true);
        }

        private void PrintOptionsSetterIsEnable(bool isEnabled)
        {
            ((PrintControlView)View).SetButton.Visibility = Visibility.Visible;
            ((PrintControlView)View).SetButton.IsEnabled = isEnabled;
            ((PrintControlView)View).CancelSetButton.IsEnabled = isEnabled;
        }

        public virtual void ExecuteMarkPageNumbers(object parameter)
        {
            ReloadPreview();
        }

        public void ExecuteCancelPrint(object parameter)
        {
            if (FullScreenPrintWindow != null)
                FullScreenPrintWindow.Close();
        }

        #region Exec Print

        public virtual void ExecutePrint(object parameter)
        {
            try
            {
                var printDialog = new System.Windows.Controls.PrintDialog();
                printDialog.PrintQueue = CurrentPrinter;
                printDialog.PrintTicket = CurrentPrinter.UserPrintTicket;
                ShowProgressDialog();
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

        private void SetupPrintOrientation(PageOrientation orientation)
        {
            if (orientation == PageOrientation.Portrait)
            {
                CurrentPrinter.UserPrintTicket.PageOrientation = PageOrientation.Portrait;
            }
            else
            {
                CurrentPrinter.UserPrintTicket.PageOrientation = PageOrientation.Landscape;
            }
        }

        public void ShowProgressDialog()
        {
            ProgressDialog = UnityContainer.Resolve<IProgressDialogViewModel>();
            var cancelAsyncPrintCommand = new DelegateCommand<object>(ExecuteCancelAsyncPrint);
            ProgressDialog.CancelCommand = cancelAsyncPrintCommand;
            ProgressDialog.MaxProgressValue = ApproaxNumberOfPages;
            ProgressDialog.CurrentProgressValue = 0;
            ProgressDialog.Message = GetStatusMessage();
            ProgressDialog.DialogTitle = "Printing...";
            ProgressDialog.CancelButtonCaption = "Cancel";
            SetProgressDialogCancelButtonVisibility();
            ProgressDialog.Show();
        }

        public virtual void SetProgressDialogCancelButtonVisibility()
        {
            ProgressDialog.CancelButtonVisibility = CurrentPrinter.IsXpsDevice ? Visibility.Visible : Visibility.Hidden;
        }

        public virtual void ExecuteCancelAsyncPrint(object obj)
        {
            try
            {
                ProgressDialog.Hide();
            }
            catch
            {
            }
        }

        public string GetStatusMessage()
        {
            return string.Format("Printing pages {0} / {1}", ProgressDialog.CurrentProgressValue, ProgressDialog.MaxProgressValue);
        }

        #endregion

        #endregion

        #region Show View

        public void CreatePrintPreviewWindow()
        {
            FullScreenPrintWindow = new Window();
            FullScreenPrintWindow.Activated += FullScreenPrintWindowActivated;
            FullScreenPrintWindow.Closing += FullScreenPrintWindowClosing;
            FullScreenPrintWindow.Title = "Print Preview";
            FullScreenPrintWindow.MinWidth = 600;
            FullScreenPrintWindow.MinHeight = 600;
            FullScreenPrintWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            FullScreenPrintWindow.ShowInTaskbar = false;
            FullScreenPrintWindow.WindowStyle = WindowStyle.SingleBorderWindow;
            FullScreenPrintWindow.WindowState = WindowState.Maximized;
            FullScreenPrintWindow.Owner = Application.Current.MainWindow;
            FullScreenPrintWindow.Content = View;
            ApplicationExtention.MainWindow = FullScreenPrintWindow;
        }

        public virtual void FullScreenPrintWindowClosing(object sender, CancelEventArgs e)
        {
            if (!ReloadingPreview)
            {
                IsAdvancedPrintingOptionOpen = false;
            }
        }

        private void FullScreenPrintWindowActivated(object sender, EventArgs e)
        {
            if (Loading)
            {
                ApplicationExtention.MainWindow = FullScreenPrintWindow;
            }
        }

        private void LoadDocument()
        {
            ////TempFileLogger.Log("Starting Load Document");
            _settingOptions = true;
            ReloadPreview();
            Loading = false;
            IsSetPrintingOptionsEnabled = false;
            IsCancelPrintingOptionsEnabled = false;
            _settingOptions = false;
            ////TempFileLogger.Log("End Load Document");

        }
        #endregion

        #region updating pane visibility
        private Dispatcher _dispatcher;
        private const bool IsShown = false;
        private System.Timers.Timer _hideTimer;
        public void ShowPrintOptionCurtain()
        {
            if (_dispatcher == null)
            {
                _dispatcher = Application.Current.Dispatcher;
                _hideTimer = new System.Timers.Timer();
                _hideTimer.Elapsed += HideTimerElapsed;
                _hideTimer.Interval = 300;
                _hideTimer.Enabled = false;
            }
            if (_dispatcher != null)
            {
                _dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(ShowUpdatingPaneHandler));
                Block();
            }
        }

        private static void Block()
        {
            if (Application.Current == null)
                return;
            while (true)
            {
                ApplicationExtention.DoEvents(Application.Current);
                System.Threading.Thread.Sleep(5);
                break;
            }
        }

        private void ShowUpdatingPaneHandler()
        {
            try
            {
                if (IsShown)
                {
                    _hideTimer.Stop();
                    return;
                }
                ((IPrintControlView)View).PrintingOptionsWaitCurtainVisibility(true);
            }
            catch
            {
            }
        }

        public void HidePrintOptionCurtain()
        {
            _hideTimer.Start();
        }
        void HideTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _hideTimer.Stop();
            if (_dispatcher != null)
                _dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(HideUpdatingPaneHandler));
        }

        private void HideUpdatingPaneHandler()
        {
            ((IPrintControlView)View).PrintingOptionsWaitCurtainVisibility(false);
        }
        #endregion

        #region Asynchronous Printer Data

        public IDocumentPaginatorSource PaginatorSource;
        public double Pageaccrosswith;
        protected PrintControlView PrintControlView;
        protected bool ShowAllPages = true;
        private readonly PrinterPreferences _printerPreferences;
        protected PrintUtility PrintUtility;

        public void FetchSetting()
        {
            ShowPrintOptionCurtain();
            CurrentPrinterName = CurrentPrinter.FullName;
            PaperSizes = PrintUtility.GetPaperSizes(CurrentPrinterName);
            var userPrintTicket = PrintUtility.GetUserPrintTicket(CurrentPrinter.FullName);
            if (userPrintTicket != null)
                CurrentPrinter.UserPrintTicket = userPrintTicket;
            SetCurrentPaper(CurrentPrinter.UserPrintTicket.PageMediaSize);
            SetPageOrientation(CurrentPrinter.UserPrintTicket.PageOrientation);
            PrintCopyCount = CurrentPrinter.UserPrintTicket.CopyCount !=null? CurrentPrinter.UserPrintTicket.CopyCount.Value:PrintCopyCount;
            ExecuteSetPrintingOptions(null);
            HidePrintOptionCurtain();
        }

        #endregion

        protected void PrintControlViewLoaded(object sender, RoutedEventArgs e)
        {
            PrintControlView.SetPrintingOptionsWaitCurtainVisibility(Visibility.Collapsed);
            InitializeProperties();
            ResetPrintingOptions();
            LoadDocument();
        }

        protected void DisplayPagePreviewsAll(DocumentPaginator paginator)
        {
            double scale;
            var rowCount = GetRowCount(paginator);
            var container = ((IPrintControlView)View).GetPagePreviewContainer();
            container.Children.Clear();
            for (var i = 0; i < rowCount; i++)
            {

                container.Children.Add(new StackPanel
                                           {
                                               Orientation = Orientation.Horizontal,
                                               HorizontalAlignment = HorizontalAlignment.Stretch,
                                               VerticalAlignment = System.Windows.VerticalAlignment.Center
                                           });
                Application.Current.DoEvents();
            }

            var totalWidth = PagesAcross * (paginator.PageSize.Width + 40);
            var totalHeight = rowCount * (paginator.PageSize.Height + 40);
            if (totalWidth > totalHeight)
            {
                scale = ((ScrollViewer)((Border)container.Parent).Parent).ActualWidth / totalWidth;
            }
            else
            {
                scale = ((ScrollViewer)((Border)container.Parent).Parent).ActualHeight / totalHeight;
            }

            scale = ShowAllPages ? scale : 1;

            for (var i = 0; i < paginator.PageCount; i++)
            {
                Application.Current.DoEvents();
                var pageElement = GetPageUiElement(i, paginator, scale);
                pageElement.HorizontalAlignment = HorizontalAlignment.Center;
                pageElement.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                var rowIndex = i / PagesAcross;
                InsertPageToPreviewContainer(rowIndex, pageElement, container);

            }
        }

        private static void InsertPageToPreviewContainer(int rowIndex, Border pageElement, StackPanel container)
        {
            ((StackPanel)container.Children[rowIndex]).Children.Add(pageElement);
        }

        private int GetRowCount(DocumentPaginator paginator)
        {
            return (int)(Math.Ceiling((double)paginator.PageCount / PagesAcross));
        }

        private Border GetPageUiElement(int i, DocumentPaginator paginator, double scale)
        {
            var source = paginator.GetPage(i);
            var border = new Border() { Background = Brushes.White };
            border.Margin = new Thickness(10 * scale);
            border.BorderBrush = Brushes.DarkGray;
            border.BorderThickness = new Thickness(1);
            //var margin = PrintUtility.GetPageMargin(CurrentPrinterName);
            var margin = new Thickness();
            var rectangle = new Rectangle();
            rectangle.Width = ((source.Size.Width * 0.96 - (margin.Left + margin.Right)) * scale);
            rectangle.Height = ((source.Size.Height * 0.96 - (margin.Top + margin.Bottom)) * scale);
            rectangle.Margin = new Thickness(margin.Left * scale, margin.Top * scale, margin.Right * scale, margin.Bottom * scale);
            rectangle.Fill = Brushes.White;
            var vb = new VisualBrush(source.Visual);
            vb.Opacity = 1;
            vb.Stretch = Stretch.Uniform;
            rectangle.Fill = vb;
            border.Child = rectangle;
            return border;
        }

        public void PrintControlLoaded()
        {
            LoadDocument();
        }
    }
}