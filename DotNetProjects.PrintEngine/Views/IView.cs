using SUT.PrintEngine.ViewModels;

namespace SUT.PrintEngine.Views
{
    public interface IView
    {
        IViewModel ViewModel { set; }
    }
}
