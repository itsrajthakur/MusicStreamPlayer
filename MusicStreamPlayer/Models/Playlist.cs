using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MusicStreamPlayer.Models
{
    public class Playlist
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string UserId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PlaylistSong> Songs { get; set; } = new List<PlaylistSong>();
    }

    public class PlaylistSong
    {
        public string Title { get; set; }
        public string Image { get; set; }
        public string MediaUrl { get; set; }
        public string Artist { get; set; }
        public DateTime AddedAt { get; set; }
    }
}