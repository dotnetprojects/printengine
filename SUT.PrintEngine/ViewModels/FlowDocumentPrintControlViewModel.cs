using System;
using System.Drawing.Printing;
using System.IO;
using System.IO.Packaging;
using System.Printing;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using Microsoft.Practices.Unity;
using SUT.PrintEngine.Extensions;
using SUT.PrintEngine.Views;

namespace SUT.PrintEngine.ViewModels
{
    public sealed class FlowDocumentPrintControlViewModel : APrintControlViewModel, IFlowDocumentPrintControlViewModel
    {
        public FlowDocument FlowDocument { get; set; }
        public FlowDocumentPrintControlViewModel(PrintControlView view, IUnityContainer unityContainer)
            : base(view, unityContainer)
        {

        }
        
        public override void ReloadPreview()
        {
            if (CurrentPaper != null)
                ReloadPreview(PageOrientation, CurrentPaper);
        }
        MemoryStream _ms;
        Package _pkg;
        XpsDocument _xpsDocument;
        public void ReloadPreview(PageOrientation pageOrientation, PaperSize currentPaper)
        {
            ReloadingPreview = true;
            if (FullScreenPrintWindow != null)
            {
                WaitScreen.Show("Loading Preview");
            }
            
            if (PageOrientation == PageOrientation.Portrait)
            {
                FlowDocument.PageHeight = currentPaper.Height;
                FlowDocument.PageWidth = currentPaper.Width;
            }
            else
            {
                FlowDocument.PageHeight = currentPaper.Width;
                FlowDocument.PageWidth = currentPaper.Height;
            }

            _ms = new MemoryStream();
            _pkg = Package.Open(_ms, FileMode.Create, FileAccess.ReadWrite);
            const string pack = "pack://temp.xps";
            var oldPackage = PackageStore.GetPackage(new Uri(pack));
            if (oldPackage == null)
                PackageStore.AddPackage(new Uri(pack), _pkg);
            else
            {
                PackageStore.RemovePackage(new Uri(pack));
                PackageStore.AddPackage(new Uri(pack), _pkg);
            }
            _xpsDocument = new XpsDocument(_pkg, CompressionOption.SuperFast, pack);
            var xpsWriter = XpsDocument.CreateXpsDocumentWriter(_xpsDocument);

            var documentPaginator = ((IDocumentPaginatorSource)FlowDocument).DocumentPaginator;
            xpsWriter.Write(documentPaginator);
            Paginator = documentPaginator;
            MaxCopies = NumberOfPages = ApproaxNumberOfPages = Paginator.PageCount;
            PagesAcross = 2;
            DisplayPagePreviewsAll(documentPaginator);
            WaitScreen.Hide();
            ReloadingPreview = false;
        }
        

        public void ShowPrintPreview(FlowDocument flowDocument)
        {
            FlowDocument = flowDocument;
            if (FullScreenPrintWindow == null)
                CreatePrintPreviewWindow();
            Loading = true;
            if (FullScreenPrintWindow != null) FullScreenPrintWindow.ShowDialog();
            ApplicationExtention.MainWindow = null;
        }

        public override void FullScreenPrintWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_xpsDocument != null)
                _xpsDocument.Close();
            if (_pkg != null)
                _pkg.Close();
            if (_ms != null)
                _ms.Close();
            base.FullScreenPrintWindowClosing(sender, e);
        }
    }
}