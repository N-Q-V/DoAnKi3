using CallApi.Models.Admin;
using CallApi.Models.Admin.ViewModel;
using CallApi.Models.Authen;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CallApi.Controllers.Admin
{
    [Route("Admin")]
    [AdminRole]
    public class CategoryController : Controller
    {
        HttpClient Client;
        string domain = "https://localhost:44374/";
        string apiCategory = "/api/admin/categories";
        [Route("Category")]
        public async Task<IActionResult> Index()
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(domain);
            ViewBag.domain = domain;
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "LoginAuth");
            }
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string categoryJson = await Client.GetStringAsync(apiCategory);
            List<Category> categories = JsonConvert.DeserializeObject<List<Category>>(categoryJson);
            return View("Views/Admin/Category/Index.cshtml", categories);
        }

        [HttpGet]
        [Route("Category/Create")]
        public async Task<IActionResult> Create()
        {
            return View("Views/Admin/Category/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Category/Create")]
        public async Task<IActionResult> Create(Category c)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(domain);
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "LoginAuth");
            }
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            if (ModelState.IsValid)
            {
                // Kiểm tra và validate ảnh
                if (c.FileImage != null && c.FileImage.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(c.FileImage.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("FileImage", "Định dạng ảnh không hợp lệ. Chỉ chấp nhận .jpg, .jpeg, .png, .gif.");
                        return View("Views/Admin/Category/Create.cshtml", c);
                    }

                    if (c.FileImage.Length > 2 * 1024 * 1024) // Giới hạn kích thước ảnh là 2MB
                    {
                        ModelState.AddModelError("FileImage", "Kích thước ảnh không được vượt quá 2MB.");
                        return View("Views/Admin/Category/Create.cshtml",c);
                    }
                }

                if (c.FileLogo != null && c.FileLogo.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(c.FileLogo.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("FileLogo", "Định dạng logo không hợp lệ. Chỉ chấp nhận .jpg, .jpeg, .png, .gif.");
                        return View("Views/Admin/Category/Create.cshtml",c);
                    }

                    if (c.FileLogo.Length > 2 * 1024 * 1024) // Giới hạn kích thước logo là 2MB
                    {
                        ModelState.AddModelError("FileLogo", "Kích thước logo không được vượt quá 2MB.");
                        return View("Views/Admin/Category/Create.cshtml",c);
                    }
                }

                // Tạo form data
                var formdata = new MultipartFormDataContent();
                formdata.Add(new StringContent(c.CategoryName), "categoryName");
                formdata.Add(new StringContent(c.Status.ToString()), "status");
                formdata.Add(new StringContent(c.CreatDate.ToString()), "creatDate");

                if (c.FileImage != null && c.FileImage.Length > 0)
                {
                    formdata.Add(new StreamContent(c.FileImage.OpenReadStream()), "fileImage", c.FileImage.FileName);
                }

                if (c.FileLogo != null && c.FileLogo.Length > 0)
                {
                    formdata.Add(new StreamContent(c.FileLogo.OpenReadStream()), "fileLogo", c.FileLogo.FileName);
                }

                // Gửi yêu cầu POST
                var response = await Client.PostAsync(apiCategory, formdata);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Thêm mới thành công: ";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi thêm danh mục: " + errorMessage);
                }
            }

            // Trả về view nếu có lỗi
            return View("Views/Admin/Category/Create.cshtml",c);
        }

        [Route("Category/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(domain);
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "LoginAuth");
            }
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                string categoryJson = await Client.GetStringAsync(apiCategory + "/" + id);
                CategoryViewModel category = JsonConvert.DeserializeObject<CategoryViewModel>(categoryJson);

                ViewBag.domain = domain;
                if (category == null)
                {
                    return NotFound();
                }

                return View("Views/Admin/Category/Edit.cshtml", category);
            }
            catch (HttpRequestException e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Category/Edit/{id}")]
        public async Task<IActionResult> Edit(CategoryViewModel c, string ImageOld, string LogoOld)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(domain);
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "LoginAuth");
            }
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            if (ModelState.IsValid)
            {
                var formdata = new MultipartFormDataContent();
                formdata.Add(new StringContent(c.CategoryId.ToString()), "categoryId");
                formdata.Add(new StringContent(c.CategoryName), "categoryName");
                formdata.Add(new StringContent(c.Status.ToString()), "status");
                formdata.Add(new StringContent(c.CreatDate.ToString()), "creatDate");
                formdata.Add(new StringContent(ImageOld ?? string.Empty), "imageOld");
                formdata.Add(new StringContent(LogoOld ?? string.Empty), "logoOld");
                if (c.FileImage != null && c.FileImage.Length > 0)
                {
                    formdata.Add(new StreamContent(c.FileImage.OpenReadStream()), "fileImage", c.FileImage.FileName);
                }
                if (c.FileLogo != null && c.FileLogo.Length > 0)
                {
                    formdata.Add(new StreamContent(c.FileLogo.OpenReadStream()), "fileLogo", c.FileLogo.FileName);
                }
                var response = await Client.PutAsync(apiCategory + "/" + c.CategoryId, formdata);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Sửa thành công: ";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(errorMessage);
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi thêm sản phẩm: " + errorMessage);
                }
            }
            return View("Views/Admin/Category/Edit.cshtml", c);
        }

        [Route("Category/Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(domain);
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "LoginAuth");
            }
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await Client.DeleteAsync(apiCategory + "/" + id);
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = "" + errorMessage;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["SuccessMessage"] = "Xóa danh mục thành công: ";
            }
            return RedirectToAction("Index");
        }
    }
}
