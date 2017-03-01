using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Printing;
using System.Drawing.Printing;
using System.Windows.Input;
using System.Windows.Media;
using SUT.PrintEngine.Controls.WaitScreen;
using SUT.PrintEngine.Utils;

namespace SUT.PrintEngine.ViewModels
{
    public class ScaleViewModel : WaitScreenViewModel
    {
        private readonly PrintControlViewModel _printControlViewModel;
        public ScaleViewModel(IWaitScreenView view, PrintControlViewModel printControlViewModel) : base(view)
        {
            ApplyScaleCommand = new DelegateCommand(ExecuteApplyScale);
            CancelScaleCommand = new DelegateCommand(ExecuteCancelScale);
            _printControlViewModel = printControlViewModel;
        }

        public ICommand ApplyScaleCommand { get; set; }

        public ICommand CancelScaleCommand { get; set; }

        public bool FitToPage
        {
            get { return _printControlViewModel.FitToPage; }
            set { _printControlViewModel.FitToPage = value; }
        }

        public ScaleTransform ScaleTransform { get; set; }

        public double Scale
        {
            get { return _printControlViewModel.Scale; }
            set { _printControlViewModel.Scale = value; }
        }

        public string LabelCancel => UiUtil.GetResourceString("Cancel", "Отмена");

        public string LabelApply => UiUtil.GetResourceString("Apply", "Применить");

        public string LabelMoreSmall => UiUtil.GetResourceString("MoreSmall", "Мельче");

        public string LabelMoreLargely => UiUtil.GetResourceString("MoreLargely", "Крупнее");

        public string LabelEnterInThePage => UiUtil.GetResourceString("EnterInThePage", "Вписать в страницу");

        public string LabelContents => UiUtil.GetResourceString("Contents", "Содержимое");

        private void ExecuteApplyScale(object parameter)
        {
            HideNow();
            _printControlViewModel.ExecuteApplyScale(parameter);
        }

        private void ExecuteCancelScale(object parameter)
        {
            HideNow();
            _printControlViewModel.ExecuteCancelScale(parameter);
        }
    }
}
