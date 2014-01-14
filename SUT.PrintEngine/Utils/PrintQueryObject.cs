using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using SUT.PrintEngine.ViewModels;

namespace SUT.PrintEngine.Utils
{
    public class PrintQueryObject : INotifyPropertyChanged
    {
        private System.Drawing.Printing.PrinterSettings _printerSettings;
        private PaperSize _defaultPaperSize;
        public CacheHelper CacheHelper { get; set; }
        public APrintControlViewModel ViewModel { set; get; }
        public string CurrentPrinterName { get; set; }
        private System.Drawing.Printing.PrinterSettings.PaperSizeCollection _paperSizes;
        public IList<PaperSize> PaperSizes { get; set; }
        public PaperSize DefaultPaperSize { get; set; }

        public PrintQueryObject(string currentPrinterFullName, APrintControlViewModel viewModel, CacheHelper cacheHelper)
        {
            CurrentPrinterName = currentPrinterFullName;
            CacheHelper = cacheHelper;
            ViewModel = viewModel;
        }

        public void FetchSetting()
        {
            SetPrinterSettings();
            PaperSizes = GetPaperSizes(); 
            DefaultPaperSize = GetDefaultPaperSize();
        }

        private IList<PaperSize> GetPaperSizes()
        {
            var Pss = new List<PaperSize>();
            var key = string.Format("{0}:PaperSizes", CurrentPrinterName);
            if (!CacheHelper.Contains(key))
            {
                var pss = _printerSettings.PaperSizes;
                foreach (var ps in pss)
                {
                    if (((PaperSize)ps).PaperName != "Custom")
                    {
                        Pss.Add((PaperSize)ps);
                    }

                }
                CacheHelper.Add(key, _printerSettings.PaperSizes);
            }
            return (IList<PaperSize>)CacheHelper.GetData(key);
        }

        protected void SetPrinterSettings()
        {
            var key = string.Format("{0}:PrinterSettings", CurrentPrinterName);
            if (!CacheHelper.Contains(key))
            {
                CacheHelper.Add(key, new System.Drawing.Printing.PrinterSettings { PrinterName = CurrentPrinterName });
            }
            _printerSettings = (System.Drawing.Printing.PrinterSettings)CacheHelper.GetData(key);
        }

        private PaperSize GetDefaultPaperSize()
        {
            var key = string.Format("{0}:DefaultPageSettings", CurrentPrinterName);
            if (!CacheHelper.Contains(key))
            {
                CacheHelper.Add(key, _printerSettings.DefaultPageSettings);
            }
            var pageSettings = (PageSettings)CacheHelper.GetData(key);
            key = string.Format("{0}:PageSize", CurrentPrinterName);
            if (!CacheHelper.Contains(key))
            {
                CacheHelper.Add(key, pageSettings.PaperSize);
            }
            return (PaperSize)CacheHelper.GetData(key);
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}