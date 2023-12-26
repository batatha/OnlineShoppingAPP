using OnlineShopping.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using OnlineShopping.Collection;
using DnsClient;
using OnlineShoppingAPI.Collection;
using System.Text.RegularExpressions;
using MongoDB.Bson.Serialization;


namespace OnlineShopping.Services
{
    public class MongoDBService
    {
        private readonly IMongoCollection<Registration> _registrationCollection;
        private readonly IMongoCollection<Login> _loginCollection;
        private readonly IMongoCollection<Products> _productsCollection;
        private readonly IMongoCollection<Orders> _ordersCollection;
        public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionString);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _registrationCollection = database.GetCollection<Registration>(mongoDBSettings.Value.FirstCollectionName);
            _loginCollection = database.GetCollection<Login>(mongoDBSettings.Value.SecondCollectionName);
            _productsCollection = database.GetCollection<Products>(mongoDBSettings.Value.ThirdCollectionName);
            _ordersCollection = database.GetCollection<Orders>(mongoDBSettings.Value.FourthCollectionName);
        }
        public async Task<List<Registration>> GetAllUsers() {
            return await _registrationCollection.Find(new BsonDocument()).ToListAsync();

        }
        private bool IsLoginidExists(string loginid)
        {
            bool exists = _registrationCollection.Find(e => e.Loginid == loginid).Any();
            return exists;
        }
        private bool IsEmaildExists(string email)
        {
            bool exists = _registrationCollection.Find(e => e.Email == email).Any();
            return exists;
        }

        public async Task<string> CreateUser(Registration registration, Login login)
        {
            if ((!IsLoginidExists(registration.Loginid)))
            {
                if (!IsEmaildExists(registration.Email))
                {
                    if (registration.Password == registration.Confirmpassword && registration.Password==login.Password)
                    {
                        await _registrationCollection.InsertOneAsync(registration);
                        await _loginCollection.InsertOneAsync(login);
                        return "User Created Successfully!";
                    }
                    else
                    {
                        return "Passwords must match!";
                    }
                }
                else
                {
                    return "Email id already exists!";
                }
            }
            else
            {
                return "Login id already exists!";

            }
        }

        public async Task<List<Login>> GetLoginUsers()
        {
            return await _loginCollection.Find(new BsonDocument()).ToListAsync();

        }
        public async Task CreateUserLogin(Login login)
        {
            await _loginCollection.InsertOneAsync(login);
            return;
        }
        
        public async Task<string> SearchPassword(string loginid)
        {
            var filter = Builders<Login>.Filter.Eq("Loginid", loginid);

            var result = await _loginCollection.Find(filter).FirstOrDefaultAsync();

            if (result != null)
            {
                string password = result.Password;
                return password;
            }
            else
            {
                return null; // Handle case where loginid is not found
            }
        }


        public async Task<List<Products>> GetProductAll()
        {
            return await _productsCollection.Find(new BsonDocument()).ToListAsync();

        }
        public async Task<List<Products>> SearchProduct(string searchProd)
        {
            var queryExpr = new BsonRegularExpression(new Regex(searchProd, RegexOptions.IgnoreCase));
            var filter = Builders<Products>.Filter.Regex("ProductName", queryExpr);
            return await _productsCollection.Find(filter).ToListAsync();

        }


        public async Task<string> CreateProduct(Products products,Login login)
        {
            bool IsAdminTrue = await IsUserAdmin(login.Loginid, login.Password);
            if (IsAdminTrue)
            {
                await _productsCollection.InsertOneAsync(products);
                return "Product Created!";
            }
            else
            {
                return "Not Allowed";
            }
        }

        public async Task<string> UpdateProduct(Products product,Login login)
        {
            bool IsAdminTrue = await IsUserAdmin(login.Loginid,login.Password);
            if (IsAdminTrue)
            {
                FilterDefinition<Products> filter = Builders<Products>.Filter.Eq("ProductId", product.ProductId);
                UpdateDefinition<Products> updateStockCount = Builders<Products>.Update.Set("StockCount", product.StockCount);
                if (product.StockCount == 0)
                {

                    UpdateDefinition<Products> updateProductStatus = Builders<Products>.Update.Set("ProductStatus", "Out of Stock");
                    await _productsCollection.UpdateOneAsync(filter, updateProductStatus);
                }
                else
                {
                    UpdateDefinition<Products> updateProductStatus = Builders<Products>.Update.Set("ProductStatus", "Hurry up to purchase");
                    await _productsCollection.UpdateOneAsync(filter, updateProductStatus);
                }
                await _productsCollection.UpdateOneAsync(filter, updateStockCount);
                return "Product Updated";
            }
            else
            {
                return null;
            }
           
        }
        public async Task DeleteProduct(int ProductId) 
        {
            FilterDefinition<Products> filter = Builders<Products>.Filter.Eq("ProductId", ProductId);
            await _productsCollection.DeleteOneAsync(filter);
            return;
        }
        //orders

//for adding products to check if user is admin from login table

        private async Task<bool> IsUserAdmin(string loginid,string password)

        {

            var filter1 = Builders<Login>.Filter.Where(x => x.Loginid == loginid && x.Role == "Admin" && x.Password==password);


            var loginCheck = await _loginCollection.Find(filter1).FirstOrDefaultAsync();



            if (loginCheck != null)

            {

                return true;

            }

            else

            {

                return false;

            }

        }

        public async Task<bool> ProductExists(int pid, Orders order)
        {
            var filter2 = Builders<Products>.Filter.Eq(x => x.StockCount, pid);
            Console.Write(filter2);
            var orderIdCheck = _productsCollection.Find(p => p.ProductId == order.ProductId).Any();
         


            return orderIdCheck;
        }
        private async Task<bool> IsUserExist(string loginid,string password)
        {
            var filter1 = Builders<Login>.Filter.Where(x => x.Loginid == loginid && x.Role == "User" && x.Password==password);
            var loginCheck = await _loginCollection.Find(filter1).FirstOrDefaultAsync();



            if (loginCheck != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<string> CreateOrder(Orders orders, Products product, Login login)
        {
            bool IsUserTrue = await IsUserExist(login.Loginid,login.Password);
            bool IsProdTrue = await ProductExists(product.ProductId, orders);
            if (IsProdTrue && IsUserTrue)
            {
                FilterDefinition<Products> filter = Builders<Products>.Filter.Eq("StockCount", product.StockCount);
                var desc = product.StockCount - 1;
                UpdateDefinition<Products> updateStockCount = Builders<Products>.Update.Set("StockCount", desc);
                await _productsCollection.UpdateOneAsync(filter, updateStockCount);
                await _ordersCollection.InsertOneAsync(orders);
                return "Order created";
            }
            else
            {
                return "Failed";
            }
        }
        public async Task<List<Orders>> GetOrdersPlaced(Login login)
        {
            bool IsAdminTrue = await IsUserAdmin(login.Loginid,login.Password);
            if (IsAdminTrue)
            {
                return await _ordersCollection.Find(new BsonDocument()).ToListAsync();

            }
            else
            {
                return null;
            }


        }

        public async Task<string> GetOrderCount(Login login,string productName)
        {
            bool IsAdminTrue = await IsUserAdmin(login.Loginid, login.Password);
            if (IsAdminTrue)
            {
                var filter = Builders<Orders>.Filter.Eq("ProductName", productName);
                var count = await _ordersCollection.Find(filter).CountDocumentsAsync();
                return "Number of "+ productName +": " + Convert.ToInt32(count);

            }
            else
            {
                return null;
            }


        }
        public async Task<List<Products>> GetAvailable(Login login)
        {
            bool IsAdminTrue = await IsUserAdmin(login.Loginid, login.Password);
            if (IsAdminTrue)
            {
                var filter1 = Builders<Products>.Filter.Where(x => x.ProductStatus== "Hurry up to purchase");
                return await _productsCollection.Find(filter1).ToListAsync();
            }
            else
            {
                return null;
            }
        }
        public async Task<List<string>> GetStock(Login login)
        {
            bool IsAdminTrue = await IsUserAdmin(login.Loginid, login.Password);
            if (IsAdminTrue)
            {
                var filter = Builders<Products>.Filter.Exists(x => x.StockCount);
                var list = await _productsCollection.Find(filter).ToListAsync();
                List<string> plist = new List<string>();
                foreach (var prd in list)
                {
                    plist.Add(prd.ProductName + " : " + prd.StockCount);
                }
                return plist;
            }
            else
            {
                return null;
            }
        }

    }
}
