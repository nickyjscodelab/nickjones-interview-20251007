using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Demo.Workflow.Domain;
using Workflow.Wpf.Services;

namespace Workflow.Wpf.ViewModels;

public class CreateRequestViewModel : BaseViewModel
{
    private readonly IApiClient _api;
    public string Title { get => _title; set { if (SetField(ref _title, value)) Raise(nameof(CanSave)); } } private string _title = string.Empty;
    public string Description { get => _description; set { if (SetField(ref _description, value)) Raise(nameof(CanSave)); } } private string _description = string.Empty;
    public Priority Priority { get => _priority; set { if (SetField(ref _priority, value)) Raise(nameof(CanSave)); } } private Priority _priority = Priority.Medium;
    public DateTime? DueDate { get => _dueDate; set => SetField(ref _dueDate, value); } private DateTime? _dueDate;
    public (string Name, Role Role) CurrentUser { get; }
    public bool IsSaving { get => _isSaving; private set { if (SetField(ref _isSaving, value)) SaveCommand.RaiseCanExecuteChanged(); } } private bool _isSaving;
    public bool CanSave => !IsSaving && !string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(Description);

    public AsyncRelayCommand SaveCommand { get; }
    public event Action? Saved;

    public CreateRequestViewModel(IApiClient api, (string Name, Role Role) currentUser)
    {
        _api = api;
        CurrentUser = currentUser;
        SaveCommand = new AsyncRelayCommand(SaveAsync, () => CanSave);
    }

    private async Task SaveAsync()
    {
        if (!CanSave) return;
        try
        {
            IsSaving = true;
            await _api.CreateRequestAsync(new CreateProjectRequestDto
            {
                Title = Title.Trim(),
                Description = Description.Trim(),
                Priority = Priority,
                RequestedBy = CurrentUser.Name,
                DueUtc = DueDate
            });
            Saved?.Invoke();
        }
        finally { IsSaving = false; }
    }
}
