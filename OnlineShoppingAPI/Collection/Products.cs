using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace OnlineShoppingAPI.Collection
{
    public class Products
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId _id { get; set; }
        [Required]
        public int productId { get; set; }
        [Required]
        [BsonElement("productName")]
        public string productName { get; set; }
        [Required]
        [BsonElement("productDescription")]
        public string productDescription { get; set; }
        [Required]
        [BsonElement("price")]
        public double? price { get; set; }
        [Required]
        [BsonElement("features")]
        public string features { get; set; }
        [Required]
        [BsonElement("productStatus")]
        public string productStatus { get; set; }
        [Required]
        [BsonElement("stockCount")]
        public int stockCount { get; set; }
    }
}
