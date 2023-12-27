using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
namespace OnlineShoppingAPI.Collection
{
    public class Orders
    {
        // MongoDB ObjectId for identifying documents
        [BsonId]
        [BsonElement("_id")]
        public ObjectId _id { get; set; }
        // Order ID, required field

        [BsonElement("orderId")]
        [Required]
        public int orderId { get; set; }
        // Product ID associated with the order

        [BsonElement("productId")]
        public int productId { get; set; }
        // Product name associated with the order, required field

        [Required]
        [BsonElement("productName")]
        public string productName { get; set; }
        // User login ID associated with the order, required field

        [Required]
        [BsonElement("loginId")]
        public string? loginId { get; set; }
    }
}
