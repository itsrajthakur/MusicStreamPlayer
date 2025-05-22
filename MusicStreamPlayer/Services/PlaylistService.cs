using MongoDB.Driver;
using MusicStreamPlayer.Models;

namespace MusicStreamPlayer.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IMongoCollection<Playlist> _playlists;

        public PlaylistService(IConfiguration configuration)
        {
            var mongoDbSettings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
            var client = new MongoClient(mongoDbSettings.ConnectionString);
            var database = client.GetDatabase(mongoDbSettings.DatabaseName);
            _playlists = database.GetCollection<Playlist>("Playlists");
        }

        public async Task<List<Playlist>> GetUserPlaylistsAsync(string userId)
        {
            return await _playlists.Find(p => p.UserId == userId)
                                 .SortByDescending(p => p.CreatedAt)
                                 .ToListAsync();
        }

        public async Task<Playlist> GetPlaylistByIdAsync(string playlistId)
        {
            return await _playlists.Find(p => p.Id == playlistId).FirstOrDefaultAsync();
        }

        public async Task<Playlist> CreatePlaylistAsync(string userId, string name)
        {
            var playlist = new Playlist
            {
                UserId = userId,
                Name = name,
                CreatedAt = DateTime.UtcNow
            };
            await _playlists.InsertOneAsync(playlist);
            return playlist;
        }

        public async Task<bool> AddSongToPlaylistAsync(string playlistId, PlaylistSong song)
        {
            var playlist = await _playlists.Find(p => p.Id == playlistId).FirstOrDefaultAsync();
            if (playlist == null) return false;

            bool songExists = playlist.Songs.Any(s => s.Title.Equals(song.Title, StringComparison.OrdinalIgnoreCase));
            if (songExists) return false;

            song.AddedAt = DateTime.UtcNow;
            var update = Builders<Playlist>.Update.Push(p => p.Songs, song);
            await _playlists.UpdateOneAsync(p => p.Id == playlistId, update);
            return true;
        }

        public async Task RemoveSongFromPlaylistAsync(string playlistId, string songMediaUrl)
        {
            var update = Builders<Playlist>.Update.PullFilter(
                p => p.Songs, 
                s => s.MediaUrl == songMediaUrl
            );
            await _playlists.UpdateOneAsync(p => p.Id == playlistId, update);
        }

        public async Task DeletePlaylistAsync(string playlistId)
        {
            await _playlists.DeleteOneAsync(p => p.Id == playlistId);
        }
    }
}