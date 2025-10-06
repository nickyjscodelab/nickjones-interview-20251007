namespace Demo.Workflow.Domain;

public class CreateSignOffDto
{
    public Role Role { get; set; }
    public string ReviewerName { get; set; } = default!;
    public Decision Decision { get; set; } = Decision.Pending;
    public string? Comment { get; set; }
}