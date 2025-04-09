using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Admin.BusinesModels;
using WebApplication1.Models.Admin.DataModels;
using WebApplication1.Models.Admin.ViewModels;

namespace WebApplication1.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly MobileDbContext _context;
        public CommentController(MobileDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return Ok();
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetComment(string id)
        {
            var comment = _context.productComments.Where(x=>x.ProductId == id).ToList();
            return Ok(comment);
        }
        [HttpPost("{id}")]
        public async Task<IActionResult> PostComment([FromForm] ProductCommentViewModel p)
        {
            var comment = new ProductComment()
            {
                ProductId = p.ProductId,
                CustomerName = p.CustomerName,
                Content = p.Content,
            };
            _context.productComments.Add(comment);
            await _context.SaveChangesAsync();
            return Ok(comment);
        }
    }
}
