namespace SUT.PrintEngine.Controls.ProgressDialog
{
    public interface IProgressContext
    {
        void SetProgress(double value);
        void SetMaxProgressValue(double value);
    }
}