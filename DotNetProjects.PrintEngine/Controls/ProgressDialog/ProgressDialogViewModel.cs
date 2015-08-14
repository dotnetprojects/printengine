using System;
using System.Windows;
using System.Windows.Input;
using SUT.PrintEngine.Controls.WaitScreen;
using SUT.PrintEngine.Resources;

namespace SUT.PrintEngine.Controls.ProgressDialog
{
    public class ProgressDialogViewModel : WaitScreenViewModel, IProgressDialogViewModel
    {
        #region IProgressDialogPresenter Properties

        public ProgressDialogViewModel(IProgressDialogView view) : base(view)
        {
        }

        public string DialogTitle
        {
            get { return (string)GetValue(DialogTitleProperty); }
            set
            {
                SetValue(DialogTitleProperty, value);
            }
        }


        public Visibility CancelButtonVisibility
        {
            get { return (Visibility)GetValue(CancelButtonVisibilityProperty); }
            set
            {
                SetValue(CancelButtonVisibilityProperty, value);
            }
        }

        public ICommand CancelCommand { get; set; }

        public string CancelButtonCaption
        {
            get { return (string)GetValue(CancelButtonCaptionProperty); }
            set
            {
                SetValue(CancelButtonCaptionProperty, value);
            }
        }

        public double MaxProgressValue
        {
            get { return (double)GetValue(MaxProgressValueProperty); }
            set
            {
                SetValue(MaxProgressValueProperty, value);
            }
        }

        public double CurrentProgressValue
        {
            get { return (double)GetValue(CurrentProgressValueProperty); }
            set
            {
                SetValue(CurrentProgressValueProperty, value);
            }
        }


        public static readonly DependencyProperty DialogTitleProperty = DependencyProperty.Register(
            "DialogTitle",
            typeof(string),
            typeof(ProgressDialogViewModel)
           );
        public static readonly DependencyProperty CancelButtonCaptionProperty = DependencyProperty.Register(
            "CancelButtonCaption",
            typeof(string),
            typeof(ProgressDialogViewModel),
            new PropertyMetadata("Cancel")
           );
        public static readonly DependencyProperty CancelButtonVisibilityProperty = DependencyProperty.Register(
            "CancelButtonVisibility",
            typeof(Visibility),
            typeof(ProgressDialogViewModel)
           );
        public static readonly DependencyProperty MaxProgressValueProperty = DependencyProperty.Register(
            "MaxProgressValue",
            typeof(double),
            typeof(ProgressDialogViewModel)
           );
        public static readonly DependencyProperty CurrentProgressValueProperty = DependencyProperty.Register(
            "CurrentProgressValue",
            typeof(double),
            typeof(ProgressDialogViewModel), new PropertyMetadata(OnPropertyChanged)
           );

        private static void OnPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var presener = (ProgressDialogViewModel) dependencyObject;
            switch (e.Property.Name)
            {
                case "CurrentProgressValue":
                    presener.UpdateProgressText();
                    break;
            }
        }

        private void UpdateProgressText()
        {
            var percentage = Convert.ToInt32(CurrentProgressValue / MaxProgressValue * 100);
            //percentage = Math.Max(percentage, 100);
            ProgressText = string.Format(ProgressTextFormat, percentage);
        }

        #endregion



        public string ProgressText
        {
            get { return (string)GetValue(ProgressTextProperty); }
            set { SetValue(ProgressTextProperty, value); }
        }

        public static readonly DependencyProperty ProgressTextProperty =
            DependencyProperty.Register("ProgressText", typeof(string), typeof(ProgressDialogViewModel), new UIPropertyMetadata("0% Completed"));

        private const string ProgressTextFormat = "{0}% Completed";



        public void Initialize(ICommand cancelCommand, int maxProgressValue)
        {
            MaxProgressValue = maxProgressValue;
            CurrentProgressValue = 0;            
            CancelCommand=cancelCommand;
            CancelButtonCaption = StringTable.AbortButtonCaption;
            Message = StringTable.WaitScreenMessage;
            ////CancelCommand.RaiseCanExecuteChanged();
        }
    }
}
