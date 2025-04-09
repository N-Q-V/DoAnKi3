using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Admin.BusinesModels;
using WebApplication1.Models.Admin.DataModels;
using WebApplication1.Models.Admin.ViewModels;

namespace WebApplication1.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly MobileDbContext _context;

        public CategoriesController(MobileDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> Getcategories()
        {
            if (_context.categories == null)
            {
                return NotFound();
            }
            return await _context.categories.ToListAsync();
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            if (_context.categories == null)
            {
                return NotFound();
            }
            var category = await _context.categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/Categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, [FromForm] CategoryViewModel c)
        {
            if (id != c.CategoryId)
            {
                return BadRequest("ID không khớp.");
            }

            var categoryToUpdate = await _context.categories.FindAsync(id);
            if (categoryToUpdate == null)
            {
                return NotFound("Danh mục không tồn tại.");
            }

            // Kiểm tra và giữ nguyên đường dẫn ảnh cũ nếu không có ảnh mới
            string imagePath = c.ImageOld; // Sử dụng giá trị cũ nếu không có ảnh mới
            if (c.FileImage != null && c.FileImage.Length > 0)
            {
                // Nếu ImageOld có tồn tại, xóa ảnh cũ
                if (!string.IsNullOrEmpty(categoryToUpdate.Image))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", categoryToUpdate.Image.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Tạo đường dẫn mới cho file ảnh
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(c.FileImage.FileName);
                var pathThumb = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/category/image", uniqueFileName);
                using (var stream = System.IO.File.Create(pathThumb))
                {
                    await c.FileImage.CopyToAsync(stream);
                }
                imagePath = "/images/category/image/" + uniqueFileName;
            }

            // Kiểm tra và giữ nguyên đường dẫn logo cũ nếu không có logo mới
            string logoPath = c.LogoOld; // Sử dụng giá trị cũ nếu không có logo mới
            if (c.FileLogo != null && c.FileLogo.Length > 0)
            {
                // Nếu LogoOld có tồn tại, xóa logo cũ
                if (!string.IsNullOrEmpty(categoryToUpdate.Logo))
                {
                    var oldLogoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", categoryToUpdate.Logo.TrimStart('/'));
                    if (System.IO.File.Exists(oldLogoPath))
                    {
                        System.IO.File.Delete(oldLogoPath);
                    }
                }

                // Tạo đường dẫn mới cho file logo
                var uniqueLogoName = Guid.NewGuid().ToString() + Path.GetExtension(c.FileLogo.FileName);
                var pathLogo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/category/logo", uniqueLogoName);
                using (var stream = System.IO.File.Create(pathLogo))
                {
                    await c.FileLogo.CopyToAsync(stream);
                }
                logoPath = "/images/category/logo/" + uniqueLogoName;
            }

            // Cập nhật dữ liệu của danh mục
            categoryToUpdate.CategoryName = c.CategoryName;
            categoryToUpdate.Status = c.Status;
            categoryToUpdate.Image = imagePath;
            categoryToUpdate.Logo = logoPath;
            categoryToUpdate.CreatDate = c.CreatDate;

            _context.categories.Update(categoryToUpdate);
            await _context.SaveChangesAsync();

            return Ok();
        }



        // POST: api/Categories
        [HttpPost]
        public async Task<ActionResult> PostCategory([FromForm] CategoryViewModel c)
        {
            if (_context.categories == null)
            {
                return Problem("Entity set 'MobileDbContext.categories' is null.");
            }

            string imagePath = "";
            if (c.FileImage != null && c.FileImage.Length > 0)
            {
                // Tạo tên file duy nhất bằng cách sử dụng GUID
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(c.FileImage.FileName);
                var pathThumb = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/category/image", uniqueFileName);

                using (var stream = System.IO.File.Create(pathThumb))
                {
                    await c.FileImage.CopyToAsync(stream);
                }
                imagePath = "/images/category/image/" + uniqueFileName;
            }

            string logoPath = "";
            if (c.FileLogo != null && c.FileLogo.Length > 0)
            {
                // Tạo tên file duy nhất cho logo bằng cách sử dụng GUID
                var uniqueLogoName = Guid.NewGuid().ToString() + Path.GetExtension(c.FileLogo.FileName);
                var pathLogo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/category/logo", uniqueLogoName);

                using (var stream = System.IO.File.Create(pathLogo))
                {
                    await c.FileLogo.CopyToAsync(stream);
                }
                logoPath = "/images/category/logo/" + uniqueLogoName;
            }

            var category = new Category
            {
                CategoryName = c.CategoryName,
                Status = c.Status,
                Image = imagePath,
                Logo = logoPath,
                CreatDate = c.CreatDate,
            };

            _context.categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.CategoryId }, category);
        }


        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (_context.categories == null)
            {
                return NotFound();
            }

            var category = await _context.categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            var hasProducts = await _context.products.AnyAsync(p => p.CategoryId == id);
            if (hasProducts)
            {
                return BadRequest("Không thể xóa danh mục này vì vẫn còn sản phẩm liên quan.");
            }

            // Xóa file ảnh nếu tồn tại
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", category.Image.TrimStart('/'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            // Xóa file logo nếu tồn tại
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", category.Logo.TrimStart('/'));
            if (System.IO.File.Exists(logoPath))
            {
                System.IO.File.Delete(logoPath);
            }

            // Xóa dữ liệu category trong cơ sở dữ liệu
            _context.categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool CategoryExists(int id)
        {
            return (_context.categories?.Any(e => e.CategoryId == id)).GetValueOrDefault();
        }
    }
}
