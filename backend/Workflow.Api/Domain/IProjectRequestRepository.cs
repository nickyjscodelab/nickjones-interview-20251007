using Demo.Workflow.Domain;

namespace Demo.Workflow.Domain;

public interface IProjectRequestRepository
{
    Task<ProjectRequest?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<ProjectRequest>> SearchAsync(string? status, string? q, CancellationToken ct = default);
    Task<ProjectRequest> AddAsync(ProjectRequest request, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
