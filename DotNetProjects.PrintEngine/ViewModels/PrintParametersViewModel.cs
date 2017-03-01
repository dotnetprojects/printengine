using System.Printing;
using System.Drawing.Printing;
using System.Windows.Input;
using SUT.PrintEngine.Controls.WaitScreen;
using SUT.PrintEngine.Utils;

namespace SUT.PrintEngine.ViewModels
{
    public class PrintParametersViewModel : WaitScreenViewModel
    {
        private readonly PrintControlViewModel _printControlViewModel;
        public PrintParametersViewModel(IWaitScreenView view, PrintControlViewModel printControlViewModel) : base(view)
        {
            _printControlViewModel = printControlViewModel;
            CancelCommand = new DelegateCommand(ExecuteCancelCommand);
            SetPrintingOptionsCommand = new DelegateCommand(ExecuteSetPrintingOptionsCommand);
        }

        public PrintQueueCollection Printers => _printControlViewModel.Printers;

        public PaperSize CurrentPaper => _printControlViewModel.CurrentPaper;

        public bool IsMarkPageNumbers
        {
            get { return _printControlViewModel.IsMarkPageNumbers; }
            set { _printControlViewModel.IsMarkPageNumbers = value; }
        }

        public PrintQueue CurrentPrinter
        {
            get { return _printControlViewModel.CurrentPrinter; }
            set { _printControlViewModel.CurrentPrinter = value; }
        }

        public ICommand PageOrientationCommand => _printControlViewModel.PageOrientationCommand;

        public ICommand ChangePaperCommand => _printControlViewModel.ChangePaperCommand;

        public ICommand SetPrintingOptionsCommand { get; set; }

        public int PrintCopyCount
        {
            get { return _printControlViewModel.PrintCopyCount; }
            set { _printControlViewModel.PrintCopyCount = value; }
        }

        private void ExecuteCancelCommand(object parameter)
        {
            HideNow();
        }

        private void ExecuteSetPrintingOptionsCommand(object parameter)
        {
            HideNow();
            _printControlViewModel.ExecuteSetPrintingOptions(parameter);
        }

        public ICommand CancelCommand { get; set; }

        public string LabelProperties => UiUtil.GetResourceString("Properties", "Свойства");

        public string LabelLandscape => UiUtil.GetResourceString("Landscape", "Альбомная");

        public string LabelPortrait => UiUtil.GetResourceString("Portrait", "Книжная");

        public string LabelOrientation => UiUtil.GetResourceString("PageOrientation", "Ориентация");

        public string LabelCancel => UiUtil.GetResourceString("Cancel", "Отмена");

        public string LabelApply => UiUtil.GetResourceString("Apply", "Применить");

        public string LabelPrinter => UiUtil.GetResourceString("Printer", "Принтер");

        public string LabelNumberPage => UiUtil.GetResourceString("NumberPage", "Номер страницы");

        public string LabelCopies => UiUtil.GetResourceString("Copies", "Копии");

        public string LabelParametersPrint => UiUtil.GetResourceString("PrintParameters", "Настройки печати");
    }
}
