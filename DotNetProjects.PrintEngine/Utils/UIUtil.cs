using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Globalization;
using System.Windows.Controls;
using System.Resources;

namespace SUT.PrintEngine.Utils
{
    public class UiUtil
    {
        private static ResourceManager _resources;
        private static CultureInfo _resourceCulture;

        public static void UpdateSize(FrameworkElement element, double availableWidth)
        {
            var vbox = new Viewbox { Child = element };
            vbox.Measure(new Size(availableWidth, 2000));
            vbox.Arrange(new Rect(0, 0, availableWidth, 2000));
            vbox.UpdateLayout();
        }

        public static void SetResources(ResourceManager resources, CultureInfo culture)
        {
            _resources = resources;
            _resourceCulture = culture;
        }

        public static string GetResourceString(string name, string defaultValue)
        {
            if (_resources != null)
                return _resources.GetString(name, _resourceCulture);

            return defaultValue;
        }
    }
}
