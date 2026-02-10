using Grading.Worker.Models;
using Microsoft.EntityFrameworkCore;

namespace Grading.Worker.Data;

public class GradingDbContext(DbContextOptions<GradingDbContext> options) : DbContext(options)
{
    public DbSet<QuizResult> Results => Set<QuizResult>();
}
