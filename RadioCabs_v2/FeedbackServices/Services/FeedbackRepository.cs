using FeedbackServices.Models;
using MongoDB.Driver;

namespace FeedbackServices.Services
{
    public class FeedbackRepository
    {
        private readonly IMongoCollection<Feedback> _feedbacks;

        public FeedbackRepository(DatabaseContext context)
        {
            _feedbacks = context.Feedbacks;
        }

        public async Task<List<Feedback>> GetAllAsync()
        {
            return await _feedbacks.Find(f => true).ToListAsync();
        }

        public async Task<Feedback?> GetByIdAsync(string id)
        {
            return await _feedbacks.Find(f => f.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Feedback feedback)
        {
            await _feedbacks.InsertOneAsync(feedback);
        }

        public async Task UpdateAsync(string id, Feedback feedback)
        {
            await _feedbacks.ReplaceOneAsync(f => f.Id == id, feedback);
        }

        public async Task DeleteAsync(string id)
        {
            await _feedbacks.DeleteOneAsync(f => f.Id == id);
        }

        public async Task<List<Feedback>> GetByCustomerEmailAsync(string emailPart)
        {
            var filter = Builders<Feedback>.Filter.Regex(f => f.Email, new MongoDB.Bson.BsonRegularExpression(emailPart, "i"));
            return await _feedbacks.Find(filter).ToListAsync();
        }
    }
}