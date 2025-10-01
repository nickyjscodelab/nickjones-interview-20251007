using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Demo.Workflow.Data;
using Demo.Workflow.Data.Repositories;
using Demo.Workflow.Domain;
using Demo.Workflow.Services;

namespace Workflow.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<AppDb>(opt => opt.UseInMemoryDatabase("workflow"));
        services.AddScoped<IProjectRequestRepository, ProjectRequestRepository>();
        services.AddScoped<ISignOffRepository, SignOffRepository>();

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IWorkflowService, WorkflowService>();
        return services;
    }
}
