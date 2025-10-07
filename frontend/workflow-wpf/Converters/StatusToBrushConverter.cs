using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Demo.Workflow.Domain;

namespace Workflow.Wpf.Converters;

public class StatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is RequestStatus s)
        {
            return s switch
            {
                RequestStatus.Draft => Brushes.LightGray,
                RequestStatus.Submitted => Brushes.LightBlue,
                RequestStatus.Approved => Brushes.LightGreen,
                RequestStatus.Rejected => Brushes.Pink,
                _ => Brushes.Transparent
            };
        }
        return Brushes.Transparent;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
}
