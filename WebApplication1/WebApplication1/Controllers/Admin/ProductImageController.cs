using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Admin")]
    public class ProductImageController : ControllerBase
    {
        private readonly MobileDbContext _context;
        public ProductImageController(MobileDbContext context)
        {
            _context = context;
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> EditImage(string id,[FromForm] IFormFile? file, [FromForm] IFormFile? fileThumb)
        {
            
            var productImage = await _context.productsImages.FirstOrDefaultAsync(x => x.ProductId == id);
            if (productImage == null)
            {
                return NotFound(); // Không tìm thấy sản phẩm
            }

            if (file != null && file.Length > 0)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/product/images", uniqueFileName);

                // Lưu ảnh vào thư mục wwwroot/images
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var imageUrl = "/images/product/images/" + uniqueFileName;

                // Nếu trường Images của productImage không rỗng, nối thêm imageUrl mới
                if (!string.IsNullOrEmpty(productImage.Images))
                {
                    imageUrl = productImage.Images + ";" + imageUrl;
                }

                // Cập nhật trường Images của sản phẩm
                productImage.Images = imageUrl;
            }
            if (fileThumb != null && fileThumb.Length > 0)
            {
                // Xóa ảnh thumbnail cũ nếu có
                if (!string.IsNullOrEmpty(productImage.Thumb))
                {
                    var oldThumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", productImage.Thumb.TrimStart('/'));
                    if (System.IO.File.Exists(oldThumbPath))
                    {
                        System.IO.File.Delete(oldThumbPath); // Xóa thumbnail cũ
                    }
                }

                // Tạo tên file duy nhất và đường dẫn cho ảnh thumbnail mới
                var uniqueThumbName = Guid.NewGuid().ToString() + Path.GetExtension(fileThumb.FileName);
                var thumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/product/thumb", uniqueThumbName);

                // Lưu ảnh thumbnail vào thư mục wwwroot/images
                using (var stream = new FileStream(thumbPath, FileMode.Create))
                {
                    await fileThumb.CopyToAsync(stream);
                }

                var thumbUrl = "/images/product/thumb/" + uniqueThumbName;

                // Cập nhật trường Thumb của sản phẩm với ảnh mới
                productImage.Thumb = thumbUrl;
            }

            // Cập nhật sản phẩm
            _context.productsImages.Update(productImage);
            await _context.SaveChangesAsync();

            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, string imagePath)
        {
            var productImages = await _context.productsImages.FirstOrDefaultAsync(p => p.ProductId == id);
            if (productImages == null)
            {
                return NotFound(); // Không tìm thấy sản phẩm
            }

            var imageList = productImages.Images.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (imageList.Contains(imagePath))
            {
                // Xóa ảnh từ hệ thống file nếu ảnh tồn tại trong danh sách
                var fullImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));
                if (System.IO.File.Exists(fullImagePath))
                {
                    System.IO.File.Delete(fullImagePath); // Xóa file ảnh
                }

                // Xóa ảnh khỏi danh sách và cập nhật chuỗi hình ảnh
                imageList.Remove(imagePath);
                productImages.Images = string.Join(";", imageList);

                _context.productsImages.Update(productImages);
                await _context.SaveChangesAsync();

                return Ok();
            }
            else
            {
                return NotFound(); // Không tìm thấy ảnh trong danh sách
            }
        }
    }
}
