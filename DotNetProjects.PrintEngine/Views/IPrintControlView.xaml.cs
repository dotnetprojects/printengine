using System.Windows.Controls;
using System.Windows.Media;

namespace SUT.PrintEngine.Views
{
    public interface IPrintControlView : IView
    {
        DocumentViewer DocumentViewer { get; }
        void PrintingOptionsWaitCurtainVisibility(bool b);
        StackPanel GetPagePreviewContainer();
        void ScalePreviewNode(ScaleTransform scaleTransform);
    }
}