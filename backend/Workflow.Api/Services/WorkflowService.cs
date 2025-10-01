using Demo.Workflow.Domain;

namespace Demo.Workflow.Services;

public class WorkflowService : IWorkflowService
{
    private readonly IProjectRequestRepository _requests;
    private readonly ISignOffRepository _signoffs;

    public WorkflowService(IProjectRequestRepository requests, ISignOffRepository signoffs)
    {
        _requests = requests;
        _signoffs = signoffs;
    }

    public Task<ProjectRequest?> GetAsync(int id, CancellationToken ct = default) =>
        _requests.GetByIdAsync(id, ct);

    public Task<IReadOnlyList<ProjectRequest>> SearchAsync(string? status, string? q, CancellationToken ct = default) =>
        _requests.SearchAsync(status, q, ct);

    public async Task<ProjectRequest> CreateRequestAsync(ProjectRequest request, CancellationToken ct = default)
    {
        request.Id = 0;
        request.CreatedUtc = DateTime.UtcNow;
        if (request.Status == 0) request.Status = RequestStatus.Submitted;
        return await _requests.AddAsync(request, ct);
    }

    public async Task SetStatusAsync(int id, RequestStatus status, CancellationToken ct = default)
    {
        var pr = await _requests.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Request not found");
        pr.Status = status;
        await _requests.SaveChangesAsync(ct);
    }

    public async Task<SignOff> AddSignOffAsync(int requestId, SignOff signOff, CancellationToken ct = default)
    {
        var pr = await _requests.GetByIdAsync(requestId, ct) ?? throw new KeyNotFoundException("Request not found");

        signOff.Id = 0;
        signOff.ProjectRequestId = requestId;
        signOff.TimestampUtc = DateTime.UtcNow;

        var result = await _signoffs.AddAsync(signOff, ct);

        // Refresh request and apply simple state transitions
        pr = (await _requests.GetByIdAsync(requestId, ct))!;

        if (pr.SignOffs.Any(s => s.Decision == Decision.Rejected))
            pr.Status = RequestStatus.Rejected;
        else if (pr.SignOffs.All(s => s.Decision == Decision.Approved))
            pr.Status = RequestStatus.Approved;

        await _requests.SaveChangesAsync(ct);
        return result;
    }
}
