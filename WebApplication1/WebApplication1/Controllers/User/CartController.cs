using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Admin.BusinesModels;
using WebApplication1.Models.Admin.DataModels;
using WebApplication1.Models.Admin.ViewModels;

namespace WebApplication1.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly MobileDbContext _context;

        public CartController(MobileDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Order")]
        public async Task<IActionResult> GetOrder()
        {
            var order = _context.orders;
            return Ok(order);
        }
        [HttpGet]
        [Route("Order/{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _context.orders.FirstOrDefaultAsync(x => x.OrderId == id);

            if (order == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy đơn hàng
            }

            return Ok(order); // Trả về đơn hàng duy nhất
        }
        [HttpGet]
        [Route("GetOrderByUserId/{id}")]
        public async Task<IActionResult> GetOrderByUserId(int id)
        {
            var order = _context.orders.Where(x=>x.UserId == id);
            return Ok(order);
        }
        [HttpGet]
        [Route("OrderItem/{id}")]
        public async Task<IActionResult> GetOrderItem(int id)
        {
            var order = _context.orderItems.Where(x => x.OrderId == id);
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Posts([FromForm] OrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    UserId = model.UserId,
                    UserName = model.UserName,
                    TotalAmount = model.TotalAmount,
                    PaymentStatus = model.PaymentStatus,
                    ShippingAddress = model.ShippingAddress,
                    Phone = model.Phone,
                    Email = model.Email,
                    Status = model.Status,
                    OrderDate = DateTime.Now
                };
                _context.orders.Add(order);
                await _context.SaveChangesAsync();
                var orderItems = model.CartItems.Select(item => new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Color = item.Color,
                }).ToList();
                await _context.orderItems.AddRangeAsync(orderItems);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Hoàn tác nếu có lỗi
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi đặt hàng", Error = ex.Message });
            }
        }

        [HttpPut]
        [Route("Order/{id}")]
        public async Task<IActionResult> Put(int id, [FromForm] Order item)
        {
            if (id != item.OrderId)
            {
                return BadRequest("ID không khớp.");
            }
            var order = await _context.orders.FindAsync(id);
            if (order == null)
            {
                return NotFound("Không tồn tại đơn hàng");
            }
            order.UserId = item.UserId;
            order.UserName = item.UserName;
            order.OrderId = id;
            order.Email = item.Email;
            order.OrderDate = item.OrderDate;
            order.ShippingAddress = item.ShippingAddress;
            order.Status = item.Status;
            order.PaymentStatus = item.PaymentStatus;
            order.Phone = item.Phone;
            order.TotalAmount = item.TotalAmount;
            _context.orders.Update(order);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("Order/Search")]
        public async Task<IActionResult> SearchByName(string? phone = "", string? status = "", string? paymentStatus = "")
        {
            var query = _context.orders.AsQueryable();

            if (!string.IsNullOrEmpty(phone))
            {
                query = query.Where(x => x.Phone.ToLower().Contains(phone.ToLower()));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(x => x.Status.ToLower().Contains(status.ToLower()));
            }

            if (!string.IsNullOrEmpty(paymentStatus))
            {
                query = query.Where(x => x.PaymentStatus.ToLower().Contains(paymentStatus.ToLower()));
            }

            var result = await query.ToListAsync();
            return Ok(result);
        }
    }
}
