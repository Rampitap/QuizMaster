namespace Quiz.API.DataBase.Intefaces;
using Quiz.API.Entities;

public interface IQuizRepository
{
    Task<IEnumerable<Quiz>> GetAllAsync();
    Task<Quiz?> GetByIdAsync(string id);
    Task CreateAsync(Quiz quiz);
    Task UpdateAsync(string id, Quiz quiz);
    Task DeleteAsync(string id);
}
