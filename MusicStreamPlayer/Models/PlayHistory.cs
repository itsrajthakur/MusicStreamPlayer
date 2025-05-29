using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MusicStreamPlayer.Models
{
    public class CommonModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string UserId { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string MediaUrl { get; set; }
        public string Artist { get; set; }
        public DateTime PlayedAt { get; set; }
    }
}