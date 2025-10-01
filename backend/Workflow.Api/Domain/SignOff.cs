// Domain/Entities.cs
namespace Demo.Workflow.Domain;

public class SignOff
{
    public int Id { get; set; }
    public int ProjectRequestId { get; set; }
    public ProjectRequest ProjectRequest { get; set; } = default!;
    public Role Role { get; set; }
    public string ReviewerName { get; set; } = default!;
    public Decision Decision { get; set; } = Decision.Pending;
    public string? Comment { get; set; }
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
}
