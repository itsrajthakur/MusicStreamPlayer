using MongoDB.Driver;
using MusicStreamPlayer.Models;

namespace MusicStreamPlayer.Services
{
    public class MongoDbService : IMongoDbService
    {
        private readonly IMongoCollection<CommonModel> _playHistory;
        private readonly IMongoCollection<CommonModel> _favorites;

        public MongoDbService(IConfiguration configuration)
        {
            var mongoDbSettings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
            var client = new MongoClient(mongoDbSettings.ConnectionString);
            var database = client.GetDatabase(mongoDbSettings.DatabaseName);
            _playHistory = database.GetCollection<CommonModel>("PlayHistory");
            _favorites = database.GetCollection<CommonModel>("FavoriteSongs");
        }

        public async Task LogPlayAsync(string userId, CommonModel playHistory)
        {
            var existing = await _playHistory.Find(x => x.Title == playHistory.Title && x.UserId == userId).FirstOrDefaultAsync();

            if (existing == null)
            {
                playHistory.UserId = userId;
                playHistory.PlayedAt = DateTime.UtcNow;
                await _playHistory.InsertOneAsync(playHistory);
            }
        }

        public async Task<IEnumerable<CommonModel>> GetUserPlayHistoryAsync(string userId)
        {
            return await _playHistory.Find(x => x.UserId == userId).SortByDescending(x => x.PlayedAt).Limit(50).ToListAsync();
        }

        public async Task ClearUserPlayHistoryAsync(string userId)
        {
            await _playHistory.DeleteManyAsync(x => x.UserId == userId);
        }

        public async Task<bool> AddToFavoritesAsync(string userId, CommonModel song)
        {
            var exists = await _favorites.Find(x => x.UserId == userId && x.Title == song.Title).FirstOrDefaultAsync();
            if (exists != null) return false;

            song.UserId = userId;
            song.PlayedAt = DateTime.UtcNow;
            await _favorites.InsertOneAsync(song);
            return true;
        }

        public async Task RemoveFromFavoritesAsync(string userId, string Title)
        {
            await _favorites.DeleteOneAsync(x => x.UserId == userId && x.Title == Title);
        }

        public async Task<List<CommonModel>> GetFavoriteSongsAsync(string userId)
        {
            return await _favorites.Find(x => x.UserId == userId)
                                   .SortByDescending(x => x.PlayedAt)
                                   .ToListAsync();
        }
    }
}