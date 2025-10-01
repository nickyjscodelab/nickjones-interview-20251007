using Demo.Workflow.Domain;
using Microsoft.EntityFrameworkCore;

namespace Demo.Workflow.Data.Repositories;

public class ProjectRequestRepository : IProjectRequestRepository
{
    private readonly AppDb _db;
    public ProjectRequestRepository(AppDb db) => _db = db;

    public async Task<ProjectRequest?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _db.ProjectRequests.Include(p => p.SignOffs).FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IReadOnlyList<ProjectRequest>> SearchAsync(string? status, string? q, CancellationToken ct = default)
    {
        var query = _db.ProjectRequests.Include(p => p.SignOffs).AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<RequestStatus>(status, true, out var st))
            query = query.Where(p => p.Status == st);

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(p => p.Title.Contains(q) || p.Description.Contains(q));

        return await query.OrderByDescending(p => p.CreatedUtc).ToListAsync(ct);
    }

    public async Task<ProjectRequest> AddAsync(ProjectRequest request, CancellationToken ct = default)
    {
        _db.ProjectRequests.Add(request);
        await _db.SaveChangesAsync(ct);
        return request;
    }

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
