namespace Quiz.API.DataBase.Services;
using MassTransit;
using MassTransit.Testing;
using Quiz.API.Contracts;
using Quiz.API.DataBase.Intefaces;
using Quiz.API.Entities;
using EventBus.Messages.Events;

public class QuizService
{
    private readonly IQuizRepository _quizRepository;
    private readonly IPublishEndpoint _publishEndpoint; 
    public QuizService(IQuizRepository quizRepository, IPublishEndpoint publishEndpoint)
    {
        _quizRepository = quizRepository;
        _publishEndpoint = publishEndpoint;
    }
    public async Task<bool> SumbitQuizAsync(string quizId, SubmitQuizRequest request) 
    {
        var quiz = await _quizRepository.GetByIdAsync(quizId);
        if (quiz == null) return false;
        var submissionEvent = new SubmissionEvent(
            Guid.NewGuid(),
            request.UserId,
            request.UserEmail,
            quizId,
            request.Answers.ToDictionary(a => a.QuestionId, a => a.SelectedOptionIndex.ToString())
        );

        //publishin in RabbitMQ
        await _publishEndpoint.Publish(submissionEvent);
        return true;
    }
    public async Task<IEnumerable<QuizDto>> GetQuizzesAsync()
    {
        var quizzes = await _quizRepository.GetAllAsync();
        return quizzes.Select(q => new QuizDto(q.Id!, q.Title, q.Description ?? "", q.Questions.Count));
    }
    public async Task<QuizForUserResponse?> GetQuizForUserAsync(string id)
    {
        var quiz = await _quizRepository.GetByIdAsync(id);
        if (quiz == null) return null;

        //we don't want CorrectOptionIndex to apear in the response
        return new QuizForUserResponse(
            quiz.Id!,
            quiz.Title,
            quiz.Questions.Select(q => new QuestionForUserDto(q.Id, q.Text, q.Options)).ToList()
        );
    }
    public async Task<string> CreateQuizAsync(CreateQuizRequest request)
    {
        var quiz = new Quiz
        {
            Title = request.Title,
            Description = request.Description,
            Questions = request.Questions.Select(q => new Question
            {
                Text = q.Text,
                Options = q.Options,
                CorrectOptionIndex = q.CorrectOptionIndex
            }).ToList()
        };

        await _quizRepository.CreateAsync(quiz);
        return quiz.Id!;
    }
    public async Task<bool> UpdateQuizAsync(string id, CreateQuizRequest reques) 
    {
        var exists = await _quizRepository.GetByIdAsync(id);

        //if quiz doesn't exist return false
        if (exists == null) return false;

        //updating quiz
        exists.Title = reques.Title;
        exists.Description = reques.Description;
        exists.Questions = reques.Questions.Select(q => new Question
        {
            Text = q.Text,
            Options = q.Options,
            CorrectOptionIndex = q.CorrectOptionIndex
        }).ToList();

        await _quizRepository.UpdateAsync(id, exists);
        return true;
    }
    public async Task<bool> DeleteQuizAsync(string id) 
    {
        var exists = await _quizRepository.GetByIdAsync(id);
        if (exists == null) return false;

        await _quizRepository.DeleteAsync(id);
        return true;
    }
}
