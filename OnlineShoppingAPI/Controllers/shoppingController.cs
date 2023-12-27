using Confluent.Kafka;
using System.Text.Json;
using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Collection;
using OnlineShopping.Services;
using OnlineShoppingAPI.Collection;
using OnlineShoppingAPI.Services;

namespace OnlineShoppingAPI.Controllers
{
    [ApiController]
    [Route("api/v1.0/[controller]")]
    public class shoppingController : ControllerBase
    {
        // MongoDB service for database operations

        private readonly MongoDBService _mongoDBService;
        // Kafka configuration

        private readonly string bootstrapServers = "localhost:9092";
        private readonly string topic = "OnlineShoppingApp";

        //EmailService for demo purpose
        private readonly IEmailService _emailService;

        // Constructor to inject MongoDB service

        public shoppingController(MongoDBService mongoDBService, IEmailService emailService)
        {
            _mongoDBService = mongoDBService;
            _emailService = emailService;
        }
        // Endpoint to retrieve all users

        [HttpGet]
        public async Task<List<Registration>> Get()
        {
            return await _mongoDBService.GetAllUsers();

        }
        // Endpoint to send login request via Kafka producer

        [HttpPost("producer")]
        public async Task<IActionResult> Post([FromBody] Login loginRequest)
        {
            string message = JsonSerializer.Serialize(loginRequest);
            return Ok(await SendLoginRequest(topic, message));
        }
        // Method to send login request using Kafka producer

        private async Task<bool> SendLoginRequest(string topic, string message)
        {
            // Kafka producer configuration

            ProducerConfig config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                ClientId = Dns.GetHostName()
            };

            try
            {
                // Using statement ensures proper disposal of Kafka producer resources

                using (var producer = new ProducerBuilder
                <Null, string>(config).Build())
                {
                    var result = await producer.ProduceAsync
                    (topic, new Message<Null, string>
                    {
                        Value = message
                    });

                    Debug.WriteLine($"Delivery Timestamp:{result.Timestamp.UtcDateTime}");
                    return await Task.FromResult(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured: {ex.Message}");
            }

            return await Task.FromResult(false);
        }
        // Wrapper class to handle registration and login data together

        public class RegistrationLoginWrapper
        {
            public Registration Registration { get; set; }
            public Login Login { get; set; }
        }

        // Endpoint to register a new user


        [HttpPost("register")]
        public async Task<ActionResult<Registration>> PostRegistration([FromBody] RegistrationLoginWrapper wrapper)
        {

            var register = await _mongoDBService.CreateUser(wrapper.Registration, wrapper.Login);

            if (register == "User Created Successfully!")
            {
                return Ok(register); // Return HTTP 200 OK with the password
            }
            else
            {
                return BadRequest(register);
            }
        }

        // Endpoint to retrieve all login users

        [HttpGet("login")]
        public async Task<List<Login>> GetLogin()
        {
            return await _mongoDBService.GetLoginUsers();

        }

        // Endpoint to retrieve password based on customername
        [HttpGet("{firstName}/forgot")]
        public async Task<ActionResult<string>> Getpassword(string firstName)
        {
            var password = await _mongoDBService.Searchpassword(firstName);
            if (password != null)
            {
                return Ok(password); // Return HTTP 200 OK with the password
            }
            else
            {
                return NotFound("User does not exists, please try again"); // Return HTTP 404 Not Found if loginId is not found
            }
        }

        // Endpoint to retrieve password based on login ID

        [HttpGet("{loginId}/forgotPassword")]
        public async Task<ActionResult<string>> ForgotPassword(string loginId)
        {
            var user = await _mongoDBService.GetUserByLoginId(loginId);

            if (user != null && !string.IsNullOrEmpty(user.emailAddress))
            {
                // Generate and send a password reset token to the user
                string resetToken = GeneratePasswordResetToken();

                await _emailService.SendPasswordResetEmail(user.emailAddress, resetToken);

                return Ok("Password reset token sent successfully.");
            }
            else
            {
                return NotFound("Invalid Login Information.");
            }
        }

        // For demo, generating a simple reset token
        private string GeneratePasswordResetToken()
        {
            // Logic to generate a secure reset token
            return Guid.NewGuid().ToString();
        }

        // Endpoint to create a new login entry

        [HttpPost]
        public async Task<ActionResult<Login>> PostLogin(Login login)
        {
            await _mongoDBService.CreateUserLogin(login);
            return CreatedAtAction(nameof(GetLogin), new { id = login._id }, login);
        }
        // Wrapper class to handle products and login data together

        public class ProductsLoginWrapper
        {
            public Products Products { get; set; }
            public Login Login { get; set; }
        }
        
        // Endpoint to retrieve all products

        [HttpGet("all")]
        public async Task<List<Products>> GetproductsAll()
        {
            return await _mongoDBService.GetProductAll();

        }

        // Endpoint to search for products based on a given product name

        [HttpGet("products/search/{productName}")]
        public async Task<List<Products>> Productsearch(string productName)
        {
            return await _mongoDBService.SearchProduct(productName);

        }

        // Endpoint to add a new product

        [HttpPost("{productName}/add")]
        public async Task<ActionResult<Products>> PostProducts([FromBody] ProductsLoginWrapper prodloginWrapper)
        {
            var prod = await _mongoDBService.CreateProduct(prodloginWrapper.Products, prodloginWrapper.Login);
            if (prod == "Product Created!")
            {
                return Ok(prod); // Return HTTP 200 OK with the password
            }
            else
            {
                return BadRequest(prod);
            }
        }


        // Endpoint to update product information

        [HttpPut("{productName}/update/{productId}")]
        public async Task<IActionResult> UpdateProd(ProductsLoginWrapper prodlog)
        {
            var res = await _mongoDBService.UpdateProduct(prodlog.Products, prodlog.Login);

            if (res != null)
            {
                // kafka displaying message in order to show  product status changed if stock count is less than or equal to 5 or 0
                ProducerConfig config = new ProducerConfig
                {
                    BootstrapServers = bootstrapServers,
                    ClientId = Dns.GetHostName()
                };
                using var producer = new ProducerBuilder<Null, string>(config).Build();
                var deliveryReport = producer.ProduceAsync(topic, new Message<Null, string> { Value = res }).Result;

                return Ok(res);
            }
            else
            {
                return BadRequest("Not Allowed");
            }
        }

        // Endpoint to delete a product


        [HttpDelete("{productName}/delete/{productId}")]
        public async Task<IActionResult> DeleteProd(int productId)
        {
            await _mongoDBService.DeleteProduct(productId);
            return NoContent();
        }

        // Wrapper class to handle orders, products, and login data together

        public class OrdersProductsLoginWrapper
        {
            public Products Products { get; set; }
            public Orders orders { get; set; }
            public Login login { get; set; }
        }

        // Endpoint to retrieve a list of orders

        [HttpGet("ListOrders")]
        public async Task<IActionResult> GetOrder(Login login)
        {
            var orderList = await _mongoDBService.GetOrdersPlaced(login);
            if (orderList != null)
            {
                return Ok(orderList);
            }
            else
            {
                return BadRequest("Not Allowed");
            }
        }

        // Endpoint to retrieve the count of orders for a specific product

        [HttpGet("{productName}/CountOrders")]
        public async Task<IActionResult> GetOrderCount(Login login, string productName)
        {
            var orderCount = await _mongoDBService.GetOrderCount(login, productName);
            if (orderCount != null)
            {
                // Kafka log to display order count

                ProducerConfig config = new ProducerConfig
                {
                    BootstrapServers = bootstrapServers,
                    ClientId = Dns.GetHostName()
                };
                using var producer = new ProducerBuilder<Null, string>(config).Build();
                var message = orderCount;
                var deliveryReport = producer.ProduceAsync(topic, new Message<Null, string> { Value = message }).Result;

                return Ok(orderCount);
            }
            else
            {
                return BadRequest("Not Allowed");
            }
        }
        // Endpoint to create a new order

        [HttpPost("orders")]
        public async Task<IActionResult> PostOrder(OrdersProductsLoginWrapper orpl)
        {
            var res = await _mongoDBService.CreateOrder(orpl.orders, orpl.Products, orpl.login);
            if (res == "Order created")
            {
                return Ok(res); // Return HTTP 200 OK with the password
            }
            else
            {
                return BadRequest(res);
            }
        }

        //call for admin to see the available products
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableProducts(Login login)
        {
            var orderList = await _mongoDBService.GetAvailable(login);
            if (orderList != null)
            {
                return Ok(orderList);
            }
            else
            {
                return BadRequest("Not Allowed");
            }
        }
        //call for kafka message
        [HttpGet("getStockCount")]
        public async Task<IActionResult> GetStockCount(Login login)
        {
            var stockList = await _mongoDBService.GetStock(login);
            if (stockList != null)
            {
                //kafka message for orderCount
                ProducerConfig config = new ProducerConfig
                {
                    BootstrapServers = bootstrapServers,
                    ClientId = Dns.GetHostName()
                };
                using var producer = new ProducerBuilder<Null, string>(config).Build();
                string message = String.Join(", ", stockList);
                var deliveryReport = producer.ProduceAsync(topic, new Message<Null, string> { Value = message }).Result;
                return Ok(stockList);
            }
            else
            {
                return BadRequest("Not Allowed");
            }
        }

    }
}

