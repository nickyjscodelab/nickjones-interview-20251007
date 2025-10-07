using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Demo.Workflow.Domain;

namespace Workflow.Wpf.Converters;

public class PriorityToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Priority p)
        {
            return p switch
            {
                Priority.Low => Brushes.Gray,
                Priority.Medium => Brushes.Goldenrod,
                Priority.High => Brushes.OrangeRed,
                Priority.Critical => Brushes.Red,
                _ => Brushes.Gray
            };
        }
        return Brushes.Transparent;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
}
