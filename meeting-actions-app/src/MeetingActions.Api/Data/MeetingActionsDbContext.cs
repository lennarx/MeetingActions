using MeetingActions.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetingActions.Api.Data;

public class MeetingActionsDbContext : DbContext
{
    public MeetingActionsDbContext(DbContextOptions<MeetingActionsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobResult> JobResults => Set<JobResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MeetingType).IsRequired().HasMaxLength(30);
            entity.Property(e => e.InputType).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.CreatedAtUtc).IsRequired();
            entity.Property(e => e.UpdatedAtUtc).IsRequired();

            entity.HasOne(e => e.Result)
                .WithOne(r => r.Job)
                .HasForeignKey<JobResult>(r => r.JobId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<JobResult>(entity =>
        {
            entity.HasKey(e => e.JobId);
            entity.Property(e => e.ResultJson).IsRequired();
            entity.Property(e => e.CreatedAtUtc).IsRequired();
        });
    }
}
