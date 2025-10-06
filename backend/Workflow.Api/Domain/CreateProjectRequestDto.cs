namespace Demo.Workflow.Domain;

public class CreateProjectRequestDto
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public Priority Priority { get; set; } = Priority.Medium;
    public string RequestedBy { get; set; } = default!;
    public DateTime? DueUtc { get; set; }
}