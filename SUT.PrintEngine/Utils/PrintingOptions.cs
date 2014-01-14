using System.Collections.Generic;
using System.Drawing.Printing;
using System.Printing;

namespace SUT.PrintEngine.Utils
{
    public class PrintingOptions
    {
        public PaperSize PaperSize { get; set; }
        public IList<PaperSize> PaperSizes { get; set; }
        public string CurrentPaperName { get; set; }
        public PaperSize CurrentPaper { get; set; }
        public PageOrientation PageOrientation { get; set; }
        public string CurrentPrinterName { get; set; }
        public PrintQueue CurrentPrinter { get; set; }
        public int PrintCopyCount { get; set; }
        public string PageRangeStart { get; set; }
        public string PageRangeEnd { get; set; }
        public bool IsMarkPageNumbers { get; set; }
        public PaperSource PaperSource { get; set; }
        public PageMediaSize CurrentPageMediaSize { get; set; }

        public PrintingOptions()
        {

        }
    }
}
