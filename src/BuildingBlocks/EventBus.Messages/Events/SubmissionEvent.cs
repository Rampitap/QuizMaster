namespace EventBus.Messages.Events;

public sealed record SubmissionEvent(
    Guid AttemptId,
    string UserId,
    string UserEmail,
    string QuizId,
    Dictionary<Guid, string> Answers
);