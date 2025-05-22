using MusicStreamPlayer.Models;

namespace MusicStreamPlayer.Services
{
    public interface IPlayHistoryService
    {
        Task LogPlayAsync(string userId, PlayHistory playHistory);
        Task<IEnumerable<PlayHistory>> GetUserPlayHistoryAsync(string userId);
    }
}