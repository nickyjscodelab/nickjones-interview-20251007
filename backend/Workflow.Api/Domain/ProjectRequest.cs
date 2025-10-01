// Domain/Entities.cs
namespace Demo.Workflow.Domain;

public class ProjectRequest
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public Priority Priority { get; set; } = Priority.Medium;
    public RequestStatus Status { get; set; } = RequestStatus.Submitted;
    public string RequestedBy { get; set; } = "alice@demo";
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? DueUtc { get; set; }

    public List<SignOff> SignOffs { get; set; } = new();
}
