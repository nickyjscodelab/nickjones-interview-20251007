using Demo.Workflow.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Workflow.Infrastructure.Data;

public static class SeedData
{
    public static async Task EnsureSeededAsync(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDb>();
        await db.Database.EnsureCreatedAsync();

        if (!await db.ProjectRequests.AnyAsync())
        {
            // add your two sample ProjectRequests + SignOffs here
            await db.SaveChangesAsync();
        }
    }
}
