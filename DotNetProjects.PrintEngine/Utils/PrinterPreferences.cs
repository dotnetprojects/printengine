using System;

namespace SUT.PrintEngine.Utils
{
    [Serializable]
    public class PrinterPreferences
    {
        public string PrinterName { get; set; }
        public bool IsMarkPageNumbers { get; set; }
    }
}
