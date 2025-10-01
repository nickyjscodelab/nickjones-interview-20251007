using Demo.Workflow.Domain;

namespace Demo.Workflow.Domain;

public interface ISignOffRepository
{
    Task<SignOff> AddAsync(SignOff signOff, CancellationToken ct = default);
}