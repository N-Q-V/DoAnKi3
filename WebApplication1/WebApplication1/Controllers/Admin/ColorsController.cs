using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.Admin.BusinesModels;

namespace WebApplication1.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class ColorsController : ControllerBase
    {
        private readonly MobileDbContext _context;

        public ColorsController(MobileDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var color = _context.colors;
            return Ok(color);
        }
    }
}
