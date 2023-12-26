using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Collection;
using OnlineShopping.Services;
using OnlineShoppingAPI.Collection;
namespace OnlineShoppingAPI
{
    public class TestClass
    {

        private readonly MongoDBService _mongoDBService;
        public TestClass(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }
    }
}
