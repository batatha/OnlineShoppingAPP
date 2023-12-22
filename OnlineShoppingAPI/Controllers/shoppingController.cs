using DnsClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using OnlineShopping.Collection;
using OnlineShopping.Services;
using OnlineShoppingAPI.Collection;



namespace OnlineShoppingAPI.Controllers
{
    [Route("api/v1.0/[controller]")]
    [ApiController]
    public class shoppingController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;
        public shoppingController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }
        public async Task<List<Registration>> Get()
        {
            return await _mongoDBService.GetAllUsers();

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




        [HttpGet("login")]
        public async Task<List<Login>> GetLogin()
        {
            return await _mongoDBService.GetLoginUsers();

        }

        //[HttpGet("{Loginid}/forgot")]
        //public async Task<List<Login>> GetPassword(string loginid)
        //{
        //    return await _mongoDBService.SearchPassword(loginid);

        //}
        [HttpGet("{Loginid}/forgot")]
        public async Task<ActionResult<string>> GetPassword(string loginid)
        {
            var password = await _mongoDBService.SearchPassword(loginid);

            if (password != null)
            {
                return Ok(password); // Return HTTP 200 OK with the password
            }
            else
            {
                return NotFound(); // Return HTTP 404 Not Found if loginid is not found
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
        [HttpGet("products/search/{ProductName}")]
        public async Task<List<Products>> Productsearch(string ProductName)
        {
            return await _mongoDBService.SearchProduct(ProductName);

        }

        [HttpPost("{ProductName}/add")]
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

        //[HttpPost("{ProductName}/update/{ProductId}")]
        //public async Task<IActionResult> UpdateProd(int ProductId,  string ProductStatus, int StockCount)
        //{
        //    await _mongoDBService.UpdateProduct(ProductId, ProductStatus, StockCount);
        //    return NoContent();
        //}
        [HttpPut("{ProductName}/update/{ProductId}")]
        public async Task<IActionResult> UpdateProd( ProductsLoginWrapper prodlog)
        {
            var res = await _mongoDBService.UpdateProduct(prodlog.Products, prodlog.Login);
          
            if (res != null)
            {
                return Ok(res);
            }
            else
            {
                return BadRequest("Not Allowed");
            }
        }

        [HttpDelete("{ProductName}/delete/{ProductId}")]
        public async Task<IActionResult> DeleteProd(int ProductId)
        {
            await _mongoDBService.DeleteProduct(ProductId);
            return NoContent();
        }
        public class OrdersProductsLoginWrapper
        {
            public Products Products { get; set; }
            public Orders orders { get; set; }
            public Login login { get; set; }
        }
        //[HttpGet("products/search/{ProductName}")]
        //public async Task<List<Products>> Productsearch(string ProductName)
        //{
        //    return await _mongoDBService.SearchProduct(ProductName);

        //}


        [HttpGet("ListOrders")]
        public async Task<IActionResult> GetOrder(Login login)
        {
            var orderList= await _mongoDBService.GetOrdersPlaced(login);
            if (orderList != null)
            {
                return Ok(orderList);
            }
            else
            {
                return BadRequest("Not Allowed");
            }
        }

        [HttpGet("{ProductName}/CountOrders")]
        public async Task<IActionResult> GetOrderCount(Login login, string ProductName)
        {
            var orderCount= await _mongoDBService.GetOrderCount(login, ProductName);
            if (orderCount != null)
            {
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

    }
}

