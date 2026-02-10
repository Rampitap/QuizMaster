namespace Quiz.API.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
public class Quiz
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public List<Question> Questions { get; set; } = [];
}

