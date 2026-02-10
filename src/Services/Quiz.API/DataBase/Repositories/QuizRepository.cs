namespace Quiz.API.DataBase.Repositories;

using MongoDB.Driver;
using Quiz.API.DataBase.Intefaces;
using Quiz.API.Entities;

public class QuizRepository : IQuizRepository
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<Quiz> _collectionQuize;


    public QuizRepository(IMongoDatabase database)
    {
        _database = database;
        _collectionQuize = _database.GetCollection<Quiz>("Quizes");
    }

    

    public async Task CreateAsync(Quiz quiz)
    {
        await _collectionQuize.InsertOneAsync(quiz);
    }

    public async Task<IEnumerable<Quiz>> GetAllAsync()
    {
        return await _collectionQuize.Find(_ => true).ToListAsync();
    }

    public async Task<Quiz?> GetByIdAsync(string id)
    {
        return await _collectionQuize.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(string id, Quiz quiz)
    {
        await _collectionQuize.ReplaceOneAsync(x => x.Id == id, quiz);
    }

    public async Task DeleteAsync(string id)
    {
        await _collectionQuize.DeleteOneAsync(x => x.Id == id);
    }
}
