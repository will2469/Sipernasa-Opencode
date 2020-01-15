using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Sipernasa
{
    public class DateTimeFormatter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime? selectedDate = value as DateTime?;

            if (selectedDate != null)
            {
                string dateTimeFormat = parameter as string;
                return selectedDate.Value.ToString(dateTimeFormat);
            }

            return "Select Date";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {

                var valor = value as string;
                if (!string.IsNullOrEmpty(valor))
                {
                    var retorno = DateTime.Parse(valor);
                    return retorno;
                }

                return null;
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
