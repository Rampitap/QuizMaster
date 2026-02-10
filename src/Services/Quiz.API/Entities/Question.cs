using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Quiz.API.Entities;

public class Question
{
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Text { get; set; }
    public List<string> Options { get; set; } = [];
    public int CorrectOptionIndex { get; set; }
}
