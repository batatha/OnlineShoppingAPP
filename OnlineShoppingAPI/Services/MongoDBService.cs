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
        // MongoDB collections for different entities
        private readonly IMongoCollection<Registration> _registrationCollection;
        private readonly IMongoCollection<Login> _loginCollection;
        private readonly IMongoCollection<Products> _productsCollection;
        private readonly IMongoCollection<Orders> _ordersCollection;
       
        // Constructor to initialize MongoDB collections using configuration settings

        public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionString);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            // Initialize collections
            _registrationCollection = database.GetCollection<Registration>(mongoDBSettings.Value.FirstCollectionName);
            _loginCollection = database.GetCollection<Login>(mongoDBSettings.Value.SecondCollectionName);
            _productsCollection = database.GetCollection<Products>(mongoDBSettings.Value.ThirdCollectionName);
            _ordersCollection = database.GetCollection<Orders>(mongoDBSettings.Value.FourthCollectionName);
        }
        // Method to retrieve all users

        public async Task<List<Registration>> GetAllUsers() {
            return await _registrationCollection.Find(new BsonDocument()).ToListAsync();

        }
        // Helper method to check if login ID exists

        private bool IsloginIdExists(string loginId)
        {
            bool exists = _registrationCollection.Find(e => e.loginId == loginId).Any();
            return exists;
        }
        // Helper method to check if email address exists

        private bool IsemailAddressdExists(string emailAddress)
        {
            bool exists = _registrationCollection.Find(e => e.emailAddress == emailAddress).Any();
            return exists;
        }
        // Method to create a new user

        public async Task<string> CreateUser(Registration registration, Login login)
        {
            // Check if login ID and email address already exist

            if ((!IsloginIdExists(registration.loginId)))
            {
                if (!IsemailAddressdExists(registration.emailAddress))
                {
                    // Check if passwords match

                    if (registration.password == registration.confirmPassword && registration.password==login.password)
                    {                       
                        // Insert user data into respective collections

                        await _registrationCollection.InsertOneAsync(registration);
                        await _loginCollection.InsertOneAsync(login);
                        return "User Created Successfully!";
                    }
                    else
                    {
                        return "passwords must match!";
                    }
                }
                else
                {
                    return "emailAddress id already exists!";
                }
            }
            else
            {
                return "Login id already exists!";

            }
        }
        // Method to retrieve all login users

        public async Task<List<Login>> GetLoginUsers()
        {
            return await _loginCollection.Find(new BsonDocument()).ToListAsync();

        }
        // Method to create a new login entry

        public async Task CreateUserLogin(Login login)
        {
            await _loginCollection.InsertOneAsync(login);
            return;
        }
        // Method to search for a password based on firstName

        public async Task<string> Searchpassword(string firstName)
        {
            var filter = Builders<Login>.Filter.Eq("firstName", firstName);
            var result = await _loginCollection.Find(filter).FirstOrDefaultAsync();
            if (result != null)
            {
                string password = result.password;
                return password;
            }
            else
            {
                return null; // Handles case where firstName of customer is not found
            }
        }

        // Method to get user password based on login ID
        public async Task<Login> GetUserByLoginId(string loginId)
        {
            var filter = Builders<Login>.Filter.Eq("loginId", loginId);

            return await _loginCollection.Find(filter).FirstOrDefaultAsync();
        }


        // Method to retrieve all products

        public async Task<List<Products>> GetProductAll()
        {
            return await _productsCollection.Find(new BsonDocument()).ToListAsync();

        }
        // Method to search for products based on a given search term

        public async Task<List<Products>> SearchProduct(string searchProd)
        {
            var queryExpr = new BsonRegularExpression(new Regex(searchProd, RegexOptions.IgnoreCase));
            var filter = Builders<Products>.Filter.Regex("productName", queryExpr);
            return await _productsCollection.Find(filter).ToListAsync();

        }

        // Method to create a new product

        public async Task<string> CreateProduct(Products products,Login login)
        {            
            // Check if the user is an admin before allowing product creation

            bool IsAdminTrue = await IsUserAdmin(login.loginId, login.password);
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
        // Method to update product information

        public async Task<string> UpdateProduct(Products product,Login login)
        {
            // Check if the user is an admin before allowing product update

            bool IsAdminTrue = await IsUserAdmin(login.loginId,login.password);
            if (IsAdminTrue)
            {
                // Update stock count

                FilterDefinition<Products> filter = Builders<Products>.Filter.Eq("productId", product.productId);
                UpdateDefinition<Products> updateStockCount = Builders<Products>.Update.Set("StockCount", product.stockCount);
                // Update product status based on stock count

                if (product.stockCount == 0)
                {

                    UpdateDefinition<Products> updateproductStatus = Builders<Products>.Update.Set("productStatus", "Out of Stock");
                    await _productsCollection.UpdateOneAsync(filter, updateproductStatus);
                }
                else
                {
                    UpdateDefinition<Products> updateproductStatus = Builders<Products>.Update.Set("productStatus", "Hurry up to purchase");
                    await _productsCollection.UpdateOneAsync(filter, updateproductStatus);
                }
                // Apply stock count update

                await _productsCollection.UpdateOneAsync(filter, updateStockCount);
                return "Product Updated";
            }
            else
            {
                return null;
            }
           
        }

        // Method to delete a product

        public async Task DeleteProduct(int productId) 
        {
            FilterDefinition<Products> filter = Builders<Products>.Filter.Eq("productId", productId);
            await _productsCollection.DeleteOneAsync(filter);
            return;
        }
      
        //method to add products, condition checking if the user is admin
        private async Task<bool> IsUserAdmin(string loginId,string password)

        {

            var filter1 = Builders<Login>.Filter.Where(x => x.loginId == loginId && x.role == "Admin" && x.password==password);


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
        //method to check if the product exisits in the database to avoid duplicate entries
        public async Task<bool> ProductExists(int pid, Orders order)
        {
            var filter2 = Builders<Products>.Filter.Eq(x => x.stockCount, pid);
            Console.Write(filter2);
            var orderIdCheck = _productsCollection.Find(p => p.productId == order.productId).Any();
         


            return orderIdCheck;
        }
        //Method to check if the user exists in the login table with the role = USER
        private async Task<bool> IsUserExist(string loginId,string password)
        {
            var filter1 = Builders<Login>.Filter.Where(x => x.loginId == loginId && x.role == "User" && x.password==password);
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

        //method for checking if the user exists in the login table
        public async Task<string> UserLoginCheck(string loginId,string password)
        {
            var filter= Builders<Login>.Filter.Where(x => x.loginId == loginId && x.password == password);

            var loginCheck = await _loginCollection.Find(filter).FirstOrDefaultAsync();

            if (loginCheck != null)
            {
                return "Login is successful";
            }
            else
            {
                return null; // Handle case where loginId is not found
            }
        }

        //method to create a new order
        public async Task<string> CreateOrder(Orders orders, Products product, Login login)
        {
            //method that checks if the user exists and if their role is USER so that only USER's can make an order
            bool IsUserTrue = await IsUserExist(login.loginId,login.password);
            bool IsProdTrue = await ProductExists(product.productId, orders);
            if (IsProdTrue && IsUserTrue)
            {
                FilterDefinition<Products> filter = Builders<Products>.Filter.Eq("StockCount", product.stockCount);
                var desc = product.stockCount - 1;
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
        //method that allows admins to check the orders that has been placed
        public async Task<List<Orders>> GetOrdersPlaced(Login login)
        {
            bool IsAdminTrue = await IsUserAdmin(login.loginId,login.password);
            if (IsAdminTrue)
            {
                return await _ordersCollection.Find(new BsonDocument()).ToListAsync();

            }
            else
            {
                return null;
            }


        }
        //method that allows admin to get the count of orders placed
        public async Task<string> GetOrderCount(Login login,string productName)
        {
            bool IsAdminTrue = await IsUserAdmin(login.loginId, login.password);
            if (IsAdminTrue)
            {
                var filter = Builders<Orders>.Filter.Eq("productName", productName);
                var count = await _ordersCollection.Find(filter).CountDocumentsAsync();
                return "Number of "+ productName +": " + Convert.ToInt32(count);

            }
            else
            {
                return null;
            }


        }
        //method that allows admin to see the available products
        public async Task<List<Products>> GetAvailable(Login login)
        {
            bool IsAdminTrue = await IsUserAdmin(login.loginId, login.password);
            if (IsAdminTrue)
            {
                var filter1 = Builders<Products>.Filter.Where(x => x.productStatus== "Hurry up to purchase");
                return await _productsCollection.Find(filter1).ToListAsync();
            }
            else
            {
                return null;
            }
        }
        //method that allows the admin to get the stock count
        public async Task<List<string>> GetStock(Login login)
        {
            bool IsAdminTrue = await IsUserAdmin(login.loginId, login.password);
            if (IsAdminTrue)
            {
                var filter = Builders<Products>.Filter.Exists(x => x.stockCount);
                var list = await _productsCollection.Find(filter).ToListAsync();
                List<string> plist = new List<string>();
                foreach (var prd in list)
                {
                    plist.Add(prd.productName + " : " + prd.stockCount);
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
