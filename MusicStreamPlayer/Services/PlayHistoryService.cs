using MongoDB.Driver;
using MusicStreamPlayer.Models;

namespace MusicStreamPlayer.Services
{
    public class PlayHistoryService : IPlayHistoryService
    {
        private readonly IMongoCollection<PlayHistory> _playHistory;

        public PlayHistoryService(IConfiguration configuration)
        {
            var mongoDbSettings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
            var client = new MongoClient(mongoDbSettings.ConnectionString);
            var database = client.GetDatabase(mongoDbSettings.DatabaseName);
            _playHistory = database.GetCollection<PlayHistory>("PlayHistory");
        }

        public async Task LogPlayAsync(string userId, PlayHistory playHistory)
        {
            var existing = await _playHistory.Find(x => x.Title == playHistory.Title && x.UserId == userId).FirstOrDefaultAsync();

            if (existing == null)
            {
                playHistory.UserId = userId;
                playHistory.PlayedAt = DateTime.UtcNow;
                await _playHistory.InsertOneAsync(playHistory);
            }
        }

        public async Task<IEnumerable<PlayHistory>> GetUserPlayHistoryAsync(string userId)
        {
            return await _playHistory.Find(x => x.UserId == userId).SortByDescending(x => x.PlayedAt).Limit(50).ToListAsync();
        }
    }
}