namespace OnlineShopping.Models
{
    public class MongoDBSettings

    {
        // MongoDB connection string

        public string ConnectionString { get; set; }
        // MongoDB database name

        public string DatabaseName { get; set; }
        // Collection names for different entities

        public string FirstCollectionName { get; set; }
        public string SecondCollectionName { get; set; }
        public string ThirdCollectionName { get; set; }
        public string FourthCollectionName { get; set; }


    }
}
