using Confluent.Kafka;
using System.Text.Json;
using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Collection;
using OnlineShopping.Services;
using OnlineShoppingAPI.Collection;



namespace OnlineShoppingAPI.Controllers
{
    [ApiController]
    [Route("api/v1.0/[controller]")]
    public class shoppingController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;
        private readonly string bootstrapServers = "localhost:9092";
        private readonly string topic = "OnlineShoppingApp";
        public shoppingController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpGet]
        public async Task<List<Registration>> Get()
        {
            return await _mongoDBService.GetAllUsers();

        }
        [HttpPost("producer")]
        public async Task<IActionResult> Post([FromBody] Login loginRequest)
        {
            string message = JsonSerializer.Serialize(loginRequest);
            return Ok(await SendLoginRequest(topic, message));
        }
        private async Task<bool> SendLoginRequest(string topic, string message)
        {
            ProducerConfig config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                ClientId = Dns.GetHostName()
            };

            try
            {
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

        public class RegistrationLoginWrapper
        {
            public Registration Registration { get; set; }
            public Login Login { get; set; }
        }

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


        //[HttpGet("login")]
        //public async Task<List<Login>> GetLogin()
        //{
        //    return await _mongoDBService.GetLoginUsers();

        //}
        [HttpGet("login")]
        public async Task<ActionResult<string>> GetLogin(string loginId, string password)
        {
            
           var result=  await _mongoDBService.UserLoginCheck(loginId,password);
            if (result != null)
            {
                return Ok("Login is successful");
            }
            else
            {
                return NotFound("User does not exist, please try again");
            }
        }

        [HttpGet("{loginId}/forgot")]
        public async Task<ActionResult<string>> Getpassword(string loginId)
        {
            var password = await _mongoDBService.Searchpassword(loginId);

            if (password != null)
            {
                return Ok(password); // Return HTTP 200 OK with the password
            }
            else
            {
                return NotFound("User does not exists, please try again"); // Return HTTP 404 Not Found if loginId is not found
            }
        }


        [HttpPost]
        public async Task<ActionResult<Login>> PostLogin(Login login)
        {
            await _mongoDBService.CreateUserLogin(login);
            return CreatedAtAction(nameof(GetLogin), new { id = login._id }, login);
        }

        public class ProductsLoginWrapper
        {
            public Products Products { get; set; }
            public Login Login { get; set; }
        }


        [HttpGet("all")]
        public async Task<List<Products>> GetproductsAll()
        {
            return await _mongoDBService.GetProductAll();

        }
        [HttpGet("products/search/{productName}")]
        public async Task<List<Products>> Productsearch(string productName)
        {
            return await _mongoDBService.SearchProduct(productName);

        }

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

     
        [HttpPut("{productName}/update/{productId}")]
        public async Task<IActionResult> UpdateProd(ProductsLoginWrapper prodlog)
        {
            var res = await _mongoDBService.UpdateProduct(prodlog.Products, prodlog.Login);

            if (res != null)
            {
                //Display message for kafka  to show product status changed if stock count is less than or equal to 5 or 0
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

        [HttpDelete("{productName}/delete/{productId}")]
        public async Task<IActionResult> DeleteProd(int productId)
        {
            await _mongoDBService.DeleteProduct(productId);
            return NoContent();
        }
        public class OrdersProductsLoginWrapper
        {
            public Products Products { get; set; }
            public Orders orders { get; set; }
            public Login login { get; set; }
        }
   

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

        [HttpGet("{productName}/CountOrders")]
        public async Task<IActionResult> GetOrderCount(Login login, string productName)
        {
            var orderCount = await _mongoDBService.GetOrderCount(login, productName);
            if (orderCount != null)
            {
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
        //API call for kafka
        [HttpGet("getStockCount")]
        public async Task<IActionResult> GetStockCount(Login login)
        {
            var stockList = await _mongoDBService.GetStock(login);
            if (stockList != null)
            {
                //kafka log to display orderCount
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

