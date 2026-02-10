namespace Grading.Worker.Models;

public class QuizResult
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = default!;
    public string UserEmail { get; set; } = default!;
    public string QuizId { get; set; } = default!;
    public int Score { get; set; }
    public bool IsPassed { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
