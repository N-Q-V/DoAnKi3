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
using static System.Net.Mime.MediaTypeNames;

namespace WebApplication1.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly MobileDbContext _context;

        public ProductsController(MobileDbContext context)
        {
            _context = context;
        }

        // GET: api/admin/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> Getproducts()
        {
            if (_context.products == null)
            {
                return NotFound();
            }
            return await _context.products.ToListAsync();
        }

        // GET: api/admin/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(string id)
        {
            if (_context.products == null)
            {
                return NotFound();
            }
            var product = await _context.products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        [HttpGet("image")]
        public async Task<ActionResult<IEnumerable<ProductImages>>> GetProductImage()
        {
            if(_context.productsImages == null)
            {
                return NotFound();
            }
            return await _context.productsImages.ToListAsync();
        }
        [HttpGet("image/{productId}")]
        public async Task<IActionResult> GetProductImage(string productId)
        {
            if (_context.productsImages == null)
            {
                return NotFound();
            }
            var productImage = await _context.productsImages.FirstOrDefaultAsync(x => x.ProductId == productId);
            if (productImage == null)
            {
                return NotFound();
            }
            return Ok(productImage);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByName(string? name = "", int? categoryId = null, bool? status = null)
        {
            // Tìm kiếm sản phẩm theo tên, danh mục, trạng thái
            var query = _context.products.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.ProductName.ToLower().Contains(name.ToLower()));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == categoryId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(x=>x.Status == status.Value);
            }

            var result = await query.ToListAsync();
            return Ok(result);
        }

        // POST: api/admin/Products
        [HttpPost]
        public async Task<IActionResult> Posts([FromForm] ProductViewModel p, IFormFile[] fileimages, [FromForm] ProductColorViewModel pc)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Tạo đối tượng Product từ ProductViewModel
                    var product = new Product
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        Price = p.Price,
                        ConnectType = p.ConnectType,
                        CategoryId = p.CategoryId,
                        Cpu = p.Cpu,
                        Display = p.Display,
                        FrontCamera = p.FrontCamera,
                        RearCamera = p.RearCamera,
                        Memory = p.Memory,
                        ProductType = p.ProductType,
                        Status = p.Status,
                        CreatAt = p.CreatAt,
                        System = p.System,
                    };

                    var existingProduct = await _context.products
                        .FirstOrDefaultAsync(pr => pr.ProductId == product.ProductId);

                    if (existingProduct != null)
                    {
                        return BadRequest(new { message = "ProductId đã tồn tại. Vui lòng chọn một giá trị khác." });
                    }

                    // Thêm sản phẩm vào database
                    _context.products.Add(product);
                    await _context.SaveChangesAsync(); // Lưu để ProductId được tạo

                    // Xử lý upload ảnh thumb
                    string thumbPath = "";
                    if (p.file != null && p.file.Length > 0)
                    {
                        // Tạo đường dẫn và kiểm tra file trùng tên
                        var uniqueThumbName = Guid.NewGuid().ToString() + Path.GetExtension(p.file.FileName);
                        var pathThumb = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/product/thumb", uniqueThumbName);

                        using (var stream = System.IO.File.Create(pathThumb))
                        {
                            await p.file.CopyToAsync(stream);
                        }
                        thumbPath = "/images/product/thumb/" + uniqueThumbName;
                    }

                    //Xử lý upload ảnh phụ
                    string imagesPath = "";
                    if (fileimages.Length > 0)
                    {
                        foreach (var file in fileimages)
                        {
                            var uniqueImageName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            var pathImages = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/product/images", uniqueImageName);
                            using (var stream = System.IO.File.Create(pathImages))
                            {
                                await file.CopyToAsync(stream);
                            }
                            imagesPath += "/images/product/images/" + uniqueImageName + ";";
                        }
                    }

                    // Tạo đối tượng ProductImages với ProductId vừa được tạo
                    var productImage = new ProductImages
                    {
                        ProductId = product.ProductId,
                        Thumb = thumbPath,
                        Images = imagesPath,
                    };
                    _context.productsImages.Add(productImage);
                    await _context.SaveChangesAsync();

                    // Thêm màu
                    var productColor = new ProductColors
                    {
                        ProductId = product.ProductId,
                        ProColorName = pc.ProColorName,
                    };
                    _context.productsColors.Add(productColor);
                    await _context.SaveChangesAsync();

                    // Commit transaction nếu không có lỗi
                    await transaction.CommitAsync();

                    return Ok(product);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    Console.WriteLine(innerException); // Log ra console để kiểm tra
                                                       // Trả lại thông báo lỗi với chi tiết inner exception
                    return BadRequest(new { message = "Error occurred while saving product: " + innerException });
                }
            }
        }
        // PUT: api/admin/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromForm] ProductViewModel p)
        {
            if (id != p.ProductId)
            {
                return BadRequest();
            }
            var product = new Product
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Price = p.Price,
                ConnectType = p.ConnectType,
                CategoryId = p.CategoryId,
                Cpu = p.Cpu,
                Display = p.Display,
                FrontCamera = p.FrontCamera,
                RearCamera = p.RearCamera,
                Memory = p.Memory,
                ProductType = p.ProductType,
                Status = p.Status,
                CreatAt = p.CreatAt,
                System = p.System,
            };

            // Cập nhật sản phẩm vào database
            _context.products.Update(product);
            await _context.SaveChangesAsync();
            return Ok(product);
        }

        // DELETE: api/admin/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            if (_context.products == null)
            {
                return NotFound();
            }
            var product = await _context.products.FindAsync(id);
            var productImage = await _context.productsImages.Where(x => x.ProductId == id).FirstOrDefaultAsync();
            var imageList = productImage.Images.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var image in imageList)
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            var thumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", productImage.Thumb.TrimStart('/'));
            if (System.IO.File.Exists(thumbPath))
            {
                System.IO.File.Delete(thumbPath);
            }
            if (product == null)
            {
                return NotFound();
            }
            _context.products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(string id)
        {
            return (_context.products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
