using Grpc.Core;
using EventBus.Messages.Protos;
using Quiz.API.DataBase.Intefaces;
namespace Quiz.API.Grpc;

public class QuizGrpcService : QuizGrpc.QuizGrpcBase
{
    public readonly IQuizRepository _quizRepository;

    public QuizGrpcService(IQuizRepository quizRepository)
    {
        _quizRepository = quizRepository;
    }

    public override async Task<QuizAnswersResponse> GetQuizAnswers(GetQuizAnswersRequest request, ServerCallContext context)
    {
        var quiz = await _quizRepository.GetByIdAsync(request.QuizId);
        if (quiz == null) throw new RpcException(new Status(StatusCode.NotFound, "Quiz not found"));

        var response = new QuizAnswersResponse 
        {
            Title = quiz.Title
        };

        response.Answer.AddRange(quiz.Questions.Select(q => new QuestionAnswer 
        {
            QuestionId = q.Id.ToString(),
            CorrectOptionIndex = q.CorrectOptionIndex
        }));

        return response;
    }
}
