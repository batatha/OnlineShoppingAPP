using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace OnlineShopping.Collection
{
    public class Registration
    {
        // MongoDB ObjectId for identifying documents

        [BsonId]
        [BsonElement("_id")]
        public ObjectId _id { get; set; }

        // First name of the user, required field

        [Required]
        [BsonElement("firstName")]
        public string? firstName { get; set; }

        // Last name of the user, required field

        [Required]
        [BsonElement("lastName")]
        public string? lastName { get; set; }

        // Email address of the user, required field

        [Required]
        [BsonElement("emailAddress")]
        public string? emailAddress { get; set; }

        // User login ID, required field

        [Required]
        [BsonElement("loginId")]
        public string? loginId { get; set; }

        // User password, required field

        [Required]
        [BsonElement("password")]
        public string? password { get; set; }

        // User password confirmation, required field

        [Required]
        [BsonElement("confirmPassword")]
        public string? confirmPassword { get; set; }

        // Contact number of the user, required field

        [Required]
        [BsonElement("contactNumber")]
        public int contactNumber { get; set; }

        // User role with a default value of "User"

        [BsonDefaultValue("User")]
        [BsonElement("role")]
        public string? role { get; set; }

        // Constructor to set the default role to "User"

        public Registration()
        {
            role = "User";
        }


    }
}
