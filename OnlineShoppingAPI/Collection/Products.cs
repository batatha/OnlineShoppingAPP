using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace OnlineShoppingAPI.Collection
{
    public class Products
    {
        // MongoDB ObjectId for identifying documents

        [BsonId]
        [BsonElement("_id")]
        public ObjectId _id { get; set; }    
        
        // Product ID, required field

        [Required]
        public int productId { get; set; }

        // Product name, required field

        [Required]
        [BsonElement("productName")]
        public string productName { get; set; }

        // Product description, required field

        [Required]
        [BsonElement("productDescription")]
        public string productDescription { get; set; }

        // Price of the product, required field

        [Required]
        [BsonElement("price")]
        public double? price { get; set; }

        // Features of the product, required field

        [Required]
        [BsonElement("features")]
        public string features { get; set; }

        // Product status, required field

        [Required]
        [BsonElement("productStatus")]
        public string productStatus { get; set; }

        // Stock count of the product, required field

        [Required]
        [BsonElement("stockCount")]
        public int stockCount { get; set; }
    }
}
