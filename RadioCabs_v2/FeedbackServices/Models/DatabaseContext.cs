using FeedbackServices.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FeedbackServices.Models
{
    public class DatabaseContext
    {
        private readonly IMongoDatabase _mongoDatabase;

        public DatabaseContext(IOptions<MongoDbSettings> settings)
        {
            var mongoSettings = settings.Value;
            var client = new MongoClient(mongoSettings.ConnectionString);
            _mongoDatabase = client.GetDatabase(mongoSettings.DatabaseName);
        }

        public IMongoCollection<Feedback> Feedbacks => 
            _mongoDatabase.GetCollection<Feedback>("Feedbacks");
    }
}