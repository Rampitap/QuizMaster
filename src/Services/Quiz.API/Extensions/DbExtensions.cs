namespace Quiz.API.Extensions;

using MongoDB.Driver;
using Quiz.API.DataBase.Intefaces;
using Quiz.API.DataBase.Repositories;
using Quiz.API.DataBase.Services;

public static class DbExtensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["DatabaseSettings:ConnectionString"];
        var databaseName = configuration["DatabaseSettings:DatabaseName"];

        var mongoClient = new MongoClient(connectionString);
        services.AddSingleton<IMongoDatabase>(mongoClient.GetDatabase(databaseName));

        services.AddScoped<IQuizRepository, QuizRepository>();
        services.AddScoped<QuizService>();

        return services;
    }
}
