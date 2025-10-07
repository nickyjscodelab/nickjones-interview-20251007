namespace Demo.Workflow.Domain;

public class CreateProjectRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; } = Priority.Medium;
    public string RequestedBy { get; set; } = string.Empty;
    public DateTime? DueUtc { get; set; }
}

public class CreateSignOffDto
{
    public Role Role { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    public Decision Decision { get; set; } = Decision.Pending;
    public string? Comment { get; set; }
}

public class RequestStatusUpdateDto
{
    public RequestStatus Status { get; set; }
}
