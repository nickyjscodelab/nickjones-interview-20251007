// Data/AppDb.cs
using Demo.Workflow.Domain;
using Microsoft.EntityFrameworkCore;

namespace Demo.Workflow.Data;

public class AppDb : DbContext
{
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }

    public DbSet<ProjectRequest> ProjectRequests => Set<ProjectRequest>();
    public DbSet<SignOff> SignOffs => Set<SignOff>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<ProjectRequest>()
         .HasMany(p => p.SignOffs)
         .WithOne(s => s.ProjectRequest)
         .HasForeignKey(s => s.ProjectRequestId);
    }
}
