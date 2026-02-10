using EventBus.Messages.Events;
using EventBus.Messages.Protos;
using Grading.Worker.Data;
using Grading.Worker.Models;
using MassTransit;
using static EventBus.Messages.Protos.QuizGrpc;

namespace Grading.Worker.Consumer;

public class SubmissionConsumer : IConsumer<SubmissionEvent>
{
    private readonly GradingDbContext _gradinDbContext;
    private readonly ILogger<SubmissionConsumer> _logger;
    private readonly QuizGrpcClient _quizGrpcClient;
    public SubmissionConsumer(GradingDbContext gradinDbContext, ILogger<SubmissionConsumer> logger, QuizGrpcClient quizGrpcClient) 
    {
        _gradinDbContext = gradinDbContext;
        _logger = logger;
        _quizGrpcClient = quizGrpcClient;
    }
    public async Task Consume(ConsumeContext<SubmissionEvent> context)
    {
        var data = context.Message;

        var grpcRequest = new GetQuizAnswersRequest { QuizId = data.QuizId };
        var correctAnswers = await _quizGrpcClient.GetQuizAnswersAsync(grpcRequest);

        _logger.LogInformation("Processing submission for Quiz {QuizId} from User {UserId}", data.QuizId, data.UserId);

        //evaluation logic(grading)
        int correctCount = 0;
        foreach (var answer in correctAnswers.Answer) 
        {
            if(data.Answers.TryGetValue(Guid.Parse(answer.QuestionId), out var selectedIndex) 
                && selectedIndex == answer.CorrectOptionIndex.ToString())  
            { 
                correctCount++; 
            }
        }
        int score = 0;
        if(correctAnswers.Answer.Count > 0)
        {
            score = (int)((double)correctCount / correctAnswers.Answer.Count * 100);
        }

        var result = new QuizResult 
        {
            Id = Guid.NewGuid(),
            UserId = data.UserId,
            UserEmail = data.UserEmail,
            QuizId = data.QuizId,
            Score = score,
            IsPassed = score >= 70,
            CreatedAt = DateTimeOffset.UtcNow
        };
        _gradinDbContext.Add(result);
        await _gradinDbContext.SaveChangesAsync();

        _logger.LogInformation("Result saved for user {UserId}, score: {score}", data.UserId, score);

        if (score >= 70) 
        {
            var passedEvent = new QuizPassedEvent(
                data.UserId,
                data.UserEmail,
                correctAnswers.Title,
                score,
                DateTimeOffset.UtcNow
            );

            await context.Publish(passedEvent);
        }
    }
}
