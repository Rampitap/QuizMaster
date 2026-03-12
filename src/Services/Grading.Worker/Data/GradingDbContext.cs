using Grading.Worker.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Grading.Worker.Data;

public class GradingDbContext(DbContextOptions<GradingDbContext> options) : DbContext(options)
{
    public DbSet<QuizResult> Results => Set<QuizResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
