using System.Text.Json.Serialization;

namespace Demo.Workflow.Domain;

public class ProjectRequest
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; } = Priority.Medium;
    public RequestStatus Status { get; set; } = RequestStatus.Submitted;
    public string RequestedBy { get; set; } = string.Empty;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? DueUtc { get; set; }
    public List<SignOff> SignOffs { get; set; } = new();
}

public class SignOff
{
    public int Id { get; set; }
    public int ProjectRequestId { get; set; }
    [JsonIgnore]
    public ProjectRequest? ProjectRequest { get; set; }
    public Role Role { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    public Decision Decision { get; set; } = Decision.Pending;
    public string? Comment { get; set; }
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
}
