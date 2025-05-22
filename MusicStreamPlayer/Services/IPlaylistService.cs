using MusicStreamPlayer.Models;

namespace MusicStreamPlayer.Services
{
    public interface IPlaylistService
    {
        Task<List<Playlist>> GetUserPlaylistsAsync(string userId);
        Task<Playlist> GetPlaylistByIdAsync(string playlistId);
        Task<Playlist> CreatePlaylistAsync(string userId, string name);
        Task<bool> AddSongToPlaylistAsync(string playlistId, PlaylistSong song);
        Task RemoveSongFromPlaylistAsync(string playlistId, string songMediaUrl);
        Task DeletePlaylistAsync(string playlistId);
    }
}