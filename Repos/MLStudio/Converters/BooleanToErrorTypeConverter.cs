
namespace Newtera.MLStudio.Converters
{
    using System;
    using System.IO;
    using System.Windows.Data;
    using Newtera.MLStudio.Properties;

    public class BooleanToErrorTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? isWarning = value as bool?;
            if (isWarning.Value)
            {
                return Resources.WarningValidationItem;
            }
            else
            {
                return Resources.ErrorValidationItem;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
