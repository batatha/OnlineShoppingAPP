using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
namespace OnlineShoppingAPI.Collection
{
    public class Login
    {
        // MongoDB ObjectId for identifying documents
        [BsonId]
        [BsonElement("_id")]
        public ObjectId _id { get; set; }

        // User login ID, required field
        [Required]
        [BsonElement("loginId")]
        public string? loginId { get; set; }

        // User password, required field
        [Required]
        [BsonElement("password")]
        public string? password { get; set; }

        // User role with a default value of "User"

        [BsonDefaultValue("User")]
        [BsonElement("role")]
        public string? role { get; set; }

        //for reset password function
        [BsonElement("emailAddress")]
        public string? emailAddress { get; set; }

        //for reset password function with customer name
        [BsonElement("firstName")]
        public string? firstName { get; set; }

        // Constructor to set the default role to "User"

        public Login()
        {
            role = "User";
        }
    }
}
