using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Demo.Workflow.Domain;

namespace Workflow.Wpf.Converters;

public class DecisionToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Decision d)
        {
            return d switch
            {
                Decision.Pending => Brushes.Khaki,
                Decision.Approved => Brushes.LightGreen,
                Decision.Rejected => Brushes.LightCoral,
                _ => Brushes.Transparent
            };
        }
        return Brushes.Transparent;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
}
