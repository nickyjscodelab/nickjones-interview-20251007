using System;
using System.Collections.Generic;

namespace Workflow.Wpf.Models;

public class SignOff
{
    public int Id { get; set; }
    public int ProjectRequestId { get; set; }
    public Role Role { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    public Decision Decision { get; set; }
    public string? Comment { get; set; }
    public DateTime TimestampUtc { get; set; }
}

public class ProjectRequest
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; }
    public RequestStatus Status { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public DateTime CreatedUtc { get; set; }
    public DateTime? DueUtc { get; set; }
    public List<SignOff> SignOffs { get; set; } = new();
}

public class CreateProjectRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public DateTime? DueUtc { get; set; }
}

public class CreateSignOff
{
    public Role Role { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    public Decision Decision { get; set; }
    public string? Comment { get; set; }
}

public class User
{
    public string Name { get; set; } = string.Empty;
    public Role Role { get; set; }
}
