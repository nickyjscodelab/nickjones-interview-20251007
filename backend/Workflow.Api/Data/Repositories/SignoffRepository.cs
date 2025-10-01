using Demo.Workflow.Domain;

namespace Demo.Workflow.Data.Repositories;

public class SignOffRepository : ISignOffRepository
{
    private readonly AppDb _db;
    public SignOffRepository(AppDb db) => _db = db;

    public async Task<SignOff> AddAsync(SignOff signOff, CancellationToken ct = default)
    {
        _db.SignOffs.Add(signOff);
        await _db.SaveChangesAsync(ct);
        return signOff;
    }
}
