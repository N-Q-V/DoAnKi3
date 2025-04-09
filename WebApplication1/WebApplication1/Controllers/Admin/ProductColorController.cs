using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Admin.BusinesModels;
using WebApplication1.Models.Admin.DataModels;
using WebApplication1.Models.Admin.ViewModels;

namespace WebApplication1.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class ProductColorController : ControllerBase
    {
        private readonly MobileDbContext _context;

        public ProductColorController(MobileDbContext context)
        {
            _context = context;
        }
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetColor(string productId)
        {

            var productColor = await _context.productsColors.FirstOrDefaultAsync(x => x.ProductId == productId);
            if (productColor == null)
            {
                return NotFound();
            }
            return Ok(productColor);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromForm] ProductColorViewModel p)
        {
            var productColor = _context.productsColors.Find(id);
            if (productColor == null)
            {
                return BadRequest();
            }
            productColor.ProColorName = p.ProColorName;
            _context.productsColors.Update(productColor);
            await _context.SaveChangesAsync();
            return Ok(productColor);
        }
    }
}
