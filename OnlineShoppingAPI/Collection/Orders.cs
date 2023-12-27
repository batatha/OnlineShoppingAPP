using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
namespace OnlineShoppingAPI.Collection
{
    public class Orders
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId _id { get; set; }
        [BsonElement("orderId")]
        [Required]
        public int orderId { get; set; }
        [BsonElement("productId")]
        public int productId { get; set; }
        [Required]
        [BsonElement("productName")]
        public string productName { get; set; }
        [Required]
        [BsonElement("loginId")]
        public string? loginId { get; set; }
    }
}
