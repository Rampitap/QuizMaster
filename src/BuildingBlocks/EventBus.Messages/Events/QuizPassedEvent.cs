namespace EventBus.Messages.Events;

public sealed record QuizPassedEvent(
    string UserId,
    string UserEmail,
    string FirstName, 
    string LastName,
    string QuizTitle,
    double Score,
    DateTimeOffset PassedAt
);
