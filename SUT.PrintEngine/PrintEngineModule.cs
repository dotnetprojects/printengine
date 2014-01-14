using Microsoft.Practices.Unity;
using SUT.PrintEngine.Controls.ProgressDialog;
using SUT.PrintEngine.Controls.WaitScreen;
using SUT.PrintEngine.Extensions;
using SUT.PrintEngine.Utils;
using SUT.PrintEngine.ViewModels;
using SUT.PrintEngine.Views;

namespace SUT.PrintEngine
{
    public class PrintEngineModule
    {
        public static IUnityContainer Container { get; set; }
        private static void RegisterGlobalClasses(IUnityContainer unityContainer)
        {
            unityContainer.RegisterType(typeof(IProgressDialogView), typeof(ProgressDialogView), true);
            unityContainer.RegisterType(typeof(IProgressDialogViewModel), typeof(ProgressDialogViewModel), true);
            unityContainer.RegisterType(typeof(PrintUtility));
            unityContainer.RegisterType(typeof(IPrintControlView), typeof(PrintControlView), true);
            unityContainer.RegisterType(typeof(IWaitScreenView), typeof(WaitScreenView), true);
            unityContainer.RegisterType(typeof(IWaitScreenViewModel), typeof(WaitScreenViewModel), true);

            unityContainer.RegisterType(typeof(IPrintControlViewModel), typeof(PrintControlViewModel), true);
            unityContainer.RegisterType(typeof(IGridPrintControlViewModel), typeof(GridPrintControlViewModel), false);
            unityContainer.RegisterType(typeof(IItemsPrintControlViewModel), typeof(ItemsPrintControlViewModel), false);
            unityContainer.RegisterType(typeof(IFlowDocumentPrintControlViewModel), typeof(FlowDocumentPrintControlViewModel), false);
            unityContainer.RegisterType(typeof(IDataTablePrintControlViewModel), typeof(DataTablePrintControlViewModel), false);
        }

        public static void Initialize(UnityContainer unityContainer)
        {
            Container = unityContainer;
            RegisterGlobalClasses(unityContainer);
        }
    }
}
