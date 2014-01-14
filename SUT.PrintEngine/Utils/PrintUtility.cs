using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Printing;
using System.Windows;

namespace SUT.PrintEngine.Utils
{
    public class PrintUtility
    {
        private readonly CacheHelper _cacheHelper;

        public PrintUtility()
        {
            _cacheHelper = new CacheHelper();
        }

        public string GetSaveLocation(string printerFullName)
        {
            return String.Format("{0}//{1}_printTicket.xml", Constants.Print.SETTINGS_FOLDER, printerFullName.Replace("\\", "_"));
        }

        private PrinterSettings GetPrinterSettings(string currentPrinterName)
        {
            var key = String.Format("{0}:PrinterSettings", currentPrinterName);
            if (!_cacheHelper.Contains(key))
            {
                _cacheHelper.Add(key, new PrinterSettings { PrinterName = currentPrinterName });
            }
            return (PrinterSettings)_cacheHelper.GetData(key);
        }

        public Thickness GetPageMargin(string currentPrinterName)
        {
            var key = String.Format("{0}:PageMargin", currentPrinterName);
            if (!_cacheHelper.Contains(key))
            {
                float hardMarginX;
                float hardMarginY;
                var printerSettings = GetPrinterSettings(currentPrinterName);
                try
                {
                    hardMarginX = printerSettings.DefaultPageSettings.HardMarginX;
                    hardMarginY = printerSettings.DefaultPageSettings.HardMarginY;
                }
                catch (Exception)
                {
                    hardMarginX = 0;
                    hardMarginY = 0;
                }
                _cacheHelper.Add(key, new Thickness(hardMarginX + 5, hardMarginY + 5, printerSettings.DefaultPageSettings.Margins.Right, printerSettings.DefaultPageSettings.Margins.Bottom + 50));
            }
            var margin = (Thickness)_cacheHelper.GetData(key);
            ////TempFileLogger.Log(String.Format("Paper margin = ({0}, {1}, {2}, {3})", margin.Left, margin.Top, margin.Right, margin.Bottom));
            return margin;
        }

        public IList<PaperSize> GetPaperSizes(string currentPrinterName)
        {
            var paperSizes = new List<PaperSize>();
            var key = String.Format("{0}:PaperSizes", currentPrinterName);
            if (!_cacheHelper.Contains(key))
            {
                var sizes = GetPrinterSettings(currentPrinterName).PaperSizes;
                foreach (var ps in sizes)
                {
                    if (((PaperSize)ps).PaperName != "Custom Size")
                    {
                        paperSizes.Add((PaperSize)ps);
                    }
                }
                _cacheHelper.Add(key, paperSizes);
            }
            ////TempFileLogger.Log("Paper sizes retrieved successfully.");
            return (IList<PaperSize>)_cacheHelper.GetData(key);
        }

        public PrintQueueCollection GetPrinters()
        {
            try
            {
                if (!_cacheHelper.Contains("Printers"))
                {
                    var printServer = new PrintServer();
                    _cacheHelper.Add("Printers", printServer.GetPrintQueues(new[] {EnumeratedPrintQueueTypes.Connections,EnumeratedPrintQueueTypes.Local}));
                }
                var printers = (PrintQueueCollection)_cacheHelper.GetData("Printers");
                return printers;
            }
            catch (Exception ex)
            {
                ////TempFileLogger.LogException(ex);
                throw;
            }
        }

        public PrintQueue GetDefaultPrintQueue(string printerName)
        {
            return LocalPrintServer.GetDefaultPrintQueue();
            ////var printQueue = new PrintServer().GetPrintQueues().Where(pq => pq.FullName == printerName).SingleOrDefault();
            ////return printQueue ?? LocalPrintServer.GetDefaultPrintQueue();
        }

        public PrintTicket GetUserPrintTicket(string printerFullName)
        {
            if (File.Exists(GetSaveLocation(printerFullName)))
            {
                var fileStream = new FileStream(GetSaveLocation(printerFullName), FileMode.Open);
                var userPrintTicket = new PrintTicket(fileStream);
                fileStream.Close();

                return userPrintTicket;
            }

            return null;
        }

        public void SaveUserPrintTicket(PrintQueue currentPrinter)
        {
            Stream outStream = new FileStream(GetSaveLocation(currentPrinter.FullName), FileMode.Create);
            currentPrinter.UserPrintTicket.SaveTo(outStream);
            outStream.Close();
        }
    }
}