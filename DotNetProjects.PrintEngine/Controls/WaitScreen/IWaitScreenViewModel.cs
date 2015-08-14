using SUT.PrintEngine.ViewModels;

namespace SUT.PrintEngine.Controls.WaitScreen
{
    public interface IWaitScreenViewModel:IViewModel
    {
        bool Hide();
        bool Show();
        bool Show(string message);
        bool Show(string message, bool disableParent);
        string Message { get; set; }
        void HideNow();
    }
}
