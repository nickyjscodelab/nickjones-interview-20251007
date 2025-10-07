using System;
using System.Net.Http;
using System.Windows;
using Workflow.Wpf.Services;
using Workflow.Wpf.ViewModels;
using Demo.Workflow.Domain;

namespace Workflow.Wpf.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm;

    public MainWindow()
    {
        InitializeComponent();
        var api = new ApiClient(new HttpClient { BaseAddress = ApiClient.DefaultBaseAddress });
        _vm = new MainViewModel(api);
        DataContext = _vm;
        _vm.RequestCreateRequested += OnRequestCreateRequested;
        Loaded += async (_, _) => await _vm.LoadAsync();
    }

    private void OnRequestCreateRequested()
    {
    var vm = new CreateRequestViewModel(new ApiClient(), _vm.CurrentUser);
        var dlg = new CreateRequestWindow { DataContext = vm, Owner = this };
        vm.Saved += async () =>
        {
            dlg.DialogResult = true;
            dlg.Close();
            await _vm.LoadAsync();
        };
        dlg.ShowDialog();
    }

    private void NewRequest_Click(object sender, RoutedEventArgs e) => OnRequestCreateRequested();

    private void RequestCard_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.DataContext is ProjectRequest pr)
        {
            _vm.SelectedRequest = pr;
        }
    }
}

#if !WINDOWS
// On non-Windows (where XAML compilation won't run) provide a stub to avoid editor red squiggles.
public partial class MainWindow
{
    private void InitializeComponent() { }
}
#endif
