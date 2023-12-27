using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace OnlineShopping.Collection
{
    public class Registration
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId _id { get; set; }
        [Required]
        [BsonElement("firstName")]
        public string? firstName { get; set; }
        [Required]
        [BsonElement("lastName")]
        public string? lastName { get; set; }
        [Required]
        [BsonElement("emailAddress")]
        public string? emailAddress { get; set; }
        [Required]
        [BsonElement("loginId")]
        public string? loginId { get; set; }
        [Required]
        [BsonElement("password")]
        public string? password { get; set; }
        [Required]
        [BsonElement("confirmPassword")]
        public string? confirmPassword { get; set; }
        [Required]
        [BsonElement("contactNumber")]
        public int contactNumber { get; set; }
        [BsonDefaultValue("User")]
        [BsonElement("role")]
        public string? role { get; set; }
        public Registration()
        {
            role = "User";
        }


    }
}
