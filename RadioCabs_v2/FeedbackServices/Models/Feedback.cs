namespace FeedbackServices.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


public class Feedback
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string? Name { get; set; }
    
    public string? Email { get; set; }
    
    public string? Text { get; set; }
    
    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime CreatedAt { get; set; }
}
