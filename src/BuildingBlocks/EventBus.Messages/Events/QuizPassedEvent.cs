namespace EventBus.Messages.Events;

public sealed record QuizPassedEvent(
    string UserId,
    string UserEmail,
    string QuizTitle,
    double Score,
    DateTimeOffset PassedAt
);
