using Demo.Workflow.Domain;

namespace Demo.Workflow.Endpoints;

public static class RequestEndpoints
{
    public static IEndpointRouteBuilder MapRequestEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/requests");

        group.MapGet("/", async (IWorkflowService svc, string? status, string? q, CancellationToken ct) =>
            Results.Ok(await svc.SearchAsync(status, q, ct)));

        group.MapGet("/{id:int}", async (IWorkflowService svc, int id, CancellationToken ct) =>
        {
            var pr = await svc.GetAsync(id, ct);
            return pr is null ? Results.NotFound() : Results.Ok(pr);
        });

        group.MapPost("/", async (IWorkflowService svc, CreateProjectRequestDto dto, CancellationToken ct) =>
        {
            var request = new ProjectRequest
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                RequestedBy = dto.RequestedBy,
                DueUtc = dto.DueUtc,
                CreatedUtc = DateTime.UtcNow,
                Status = RequestStatus.Draft
            };
            var created = await svc.CreateRequestAsync(request, ct);
            return Results.Created($"/api/requests/{created.Id}", created);
        });

        group.MapPut("/{id:int}/status", async (IWorkflowService svc, int id, RequestStatusUpdateDto dto, CancellationToken ct) =>
        {
            await svc.SetStatusAsync(id, dto.Status, ct);
            return Results.NoContent();
        });

        group.MapPost("/{id:int}/signoffs", async (IWorkflowService svc, int id, CreateSignOffDto dto, CancellationToken ct) =>
        {
            var signOff = new SignOff
            {
                ProjectRequestId = id,
                Role = dto.Role,
                ReviewerName = dto.ReviewerName,
                Decision = dto.Decision,
                Comment = dto.Comment,
                TimestampUtc = DateTime.UtcNow
            };
            var created = await svc.AddSignOffAsync(id, signOff, ct);
            return Results.Created($"/api/requests/{id}/signoffs/{created.Id}", created);
        });

        return routes;
    }
}
