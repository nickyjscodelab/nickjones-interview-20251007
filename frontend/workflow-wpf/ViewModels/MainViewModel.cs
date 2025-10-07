using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Demo.Workflow.Domain;
using Workflow.Wpf.Services;

namespace Workflow.Wpf.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly IApiClient _api;
    private readonly object _lock = new();

    public ObservableCollection<ProjectRequest> Requests { get; } = new();
    public ListCollectionView RequestsView { get; }

    private ProjectRequest? _selectedRequest;
    public ProjectRequest? SelectedRequest { get => _selectedRequest; set { if (SetField(ref _selectedRequest, value)) Raise(nameof(CanSignOff)); } }

    private string? _statusFilter;
    public string? StatusFilter { get => _statusFilter; set { if (SetField(ref _statusFilter, value)) RequestsView.Refresh(); } }

    private string _search = string.Empty;
    public string Search { get => _search; set { if (SetField(ref _search, value)) RequestsView.Refresh(); } }

    public (string Name, Role Role) CurrentUser { get; } = ("alice@demo", Role.Engineer);

    private bool _isBusy;
    public bool IsBusy { get => _isBusy; private set { if (SetField(ref _isBusy, value)) { LoadCommand.RaiseCanExecuteChanged(); RefreshCommand.RaiseCanExecuteChanged(); } } }

    public AsyncRelayCommand LoadCommand { get; }
    public RelayCommand OpenCreateCommand { get; }
    public AsyncRelayCommand RefreshCommand { get; }
    public AsyncRelayCommand AddSignOffCommand { get; }

    private Decision _newSignOffDecision = Decision.Approved;
    public Decision NewSignOffDecision { get => _newSignOffDecision; set => SetField(ref _newSignOffDecision, value); }
    private string? _newSignOffComment;
    public string? NewSignOffComment { get => _newSignOffComment; set => SetField(ref _newSignOffComment, value); }

    public bool CanSignOff => SelectedRequest != null && SelectedRequest.Status == RequestStatus.Submitted && !SelectedRequest.SignOffs.Any(s => s.Role == CurrentUser.Role && s.ReviewerName == CurrentUser.Name);

    public event Action? RequestCreateRequested;
    public event Action? SignOffSucceeded;

    public MainViewModel(IApiClient api)
    {
        _api = api;
        BindingOperations.EnableCollectionSynchronization(Requests, _lock);
        RequestsView = (ListCollectionView)CollectionViewSource.GetDefaultView(Requests);
        RequestsView.Filter = FilterRequest;

        LoadCommand = new AsyncRelayCommand(LoadAsync, () => !IsBusy);
        RefreshCommand = new AsyncRelayCommand(LoadAsync, () => !IsBusy);
        OpenCreateCommand = new RelayCommand(() => RequestCreateRequested?.Invoke());
        AddSignOffCommand = new AsyncRelayCommand(AddSignOffAsync, () => CanSignOff);
    }

    private bool FilterRequest(object obj)
    {
        if (obj is not ProjectRequest pr) return false;
        if (!string.IsNullOrWhiteSpace(StatusFilter) && pr.Status.ToString() != StatusFilter) return false;
        if (!string.IsNullOrWhiteSpace(Search))
        {
            var q = Search.Trim().ToLowerInvariant();
            if (!(pr.Title.ToLowerInvariant().Contains(q) || pr.Description.ToLowerInvariant().Contains(q) || pr.RequestedBy.ToLowerInvariant().Contains(q))) return false;
        }
        return true;
    }

    public async Task LoadAsync()
    {
        try
        {
            IsBusy = true;
            Requests.Clear();
            var list = await _api.GetRequestsAsync();
            foreach (var r in list.OrderByDescending(r => r.CreatedUtc)) Requests.Add(r);
            RequestsView.Refresh();
        }
        finally { IsBusy = false; }
    }

    private async Task AddSignOffAsync()
    {
        if (!CanSignOff || SelectedRequest == null) return;
        await _api.AddSignOffAsync(SelectedRequest.Id, new CreateSignOffDto
        {
            Role = CurrentUser.Role,
            ReviewerName = CurrentUser.Name,
            Decision = NewSignOffDecision,
            Comment = string.IsNullOrWhiteSpace(NewSignOffComment) ? null : NewSignOffComment
        });
        await LoadAsync();
        SelectedRequest = Requests.FirstOrDefault(r => r.Id == SelectedRequest.Id);
        SignOffSucceeded?.Invoke();
    }
}
