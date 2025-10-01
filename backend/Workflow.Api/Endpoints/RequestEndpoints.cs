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

        group.MapPost("/", async (IWorkflowService svc, ProjectRequest dto, CancellationToken ct) =>
            Results.Created($"/api/requests", await svc.CreateRequestAsync(dto, ct)));

        group.MapPut("/{id:int}/status", async (IWorkflowService svc, int id, RequestStatus status, CancellationToken ct) =>
        {
            await svc.SetStatusAsync(id, status, ct);
            return Results.NoContent();
        });

        group.MapPost("/{id:int}/signoffs", async (IWorkflowService svc, int id, SignOff dto, CancellationToken ct) =>
            Results.Created($"/api/requests/{id}", await svc.AddSignOffAsync(id, dto, ct)));

        return routes;
    }
}
