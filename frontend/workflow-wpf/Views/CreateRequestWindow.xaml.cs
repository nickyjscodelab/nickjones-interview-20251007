using System.Windows;

namespace Workflow.Wpf.Views;

public partial class CreateRequestWindow : Window
{
    public CreateRequestWindow()
    {
        InitializeComponent();
    }
}

#if !WINDOWS
public partial class CreateRequestWindow
{
    private void InitializeComponent() { }
}
#endif
