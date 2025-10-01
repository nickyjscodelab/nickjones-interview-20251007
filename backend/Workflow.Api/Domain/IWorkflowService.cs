using Demo.Workflow.Domain;

namespace Demo.Workflow.Domain;

public interface IWorkflowService
{
    Task<ProjectRequest> CreateRequestAsync(ProjectRequest request, CancellationToken ct = default);
    Task<ProjectRequest?> GetAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<ProjectRequest>> SearchAsync(string? status, string? q, CancellationToken ct = default);
    Task SetStatusAsync(int id, RequestStatus status, CancellationToken ct = default);
    Task<SignOff> AddSignOffAsync(int requestId, SignOff signOff, CancellationToken ct = default);
}
