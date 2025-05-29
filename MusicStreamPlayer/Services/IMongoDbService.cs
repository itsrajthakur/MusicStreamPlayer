using MusicStreamPlayer.Models;

namespace MusicStreamPlayer.Services
{
    public interface IMongoDbService
    {
        Task LogPlayAsync(string userId, CommonModel playHistory);
        Task<IEnumerable<CommonModel>> GetUserPlayHistoryAsync(string userId);
        Task ClearUserPlayHistoryAsync(string userId);
        Task<bool> AddToFavoritesAsync(string userId, CommonModel song);
        Task RemoveFromFavoritesAsync(string userId, string mediaUrl);
        Task<List<CommonModel>> GetFavoriteSongsAsync(string userId);
    }
}