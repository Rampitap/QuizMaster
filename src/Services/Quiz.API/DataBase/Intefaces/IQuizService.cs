using Quiz.API.Contracts;

namespace Quiz.API.DataBase.Intefaces;

public interface IQuizService
{
    public Task<bool> SumbitQuizAsync(string quizId, SubmitQuizRequest request);
    public Task<IEnumerable<QuizDto>> GetQuizzesAsync();
    public Task<QuizForUserResponse?> GetQuizForUserAsync(string id);
    public Task<string> CreateQuizAsync(CreateQuizRequest request);
    public Task<bool> UpdateQuizAsync(string id, CreateQuizRequest reques);
    public Task<bool> DeleteQuizAsync(string id);
}
