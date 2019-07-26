using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CardLifeAltLaunch
{
    public class EmptyVisibilityConverter : IValueConverter
    {

        public Visibility HideType { get; set; }
        public bool Reverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool anIsEmpty;
            if (value is string)
            {
                string aStr = (string)value;
                anIsEmpty = string.IsNullOrEmpty(aStr);
            }
            else if (value is SecureString)
            {
                SecureString aSecureStr = (SecureString)value;
                anIsEmpty = (aSecureStr == null || aSecureStr.Length == 0);
            }
            else
            {
                anIsEmpty = true;
            }

            return (anIsEmpty ^ Reverse) ? HideType : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
