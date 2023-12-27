using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
namespace OnlineShoppingAPI.Collection
{
    public class Login
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId _id { get; set; }
        [Required]
        [BsonElement("loginId")]
        public string? loginId { get; set; }
        [Required]
        [BsonElement("password")]
        public string? password { get; set; }
        [BsonDefaultValue("User")]
        [BsonElement("role")]
        public string? role { get; set; }
        public Login()
        {
            role = "User";
        }
    }
}
