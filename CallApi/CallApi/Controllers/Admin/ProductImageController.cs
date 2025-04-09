using CallApi.Models.Admin;
using CallApi.Models.Authen;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CallApi.Controllers.Admin
{
    [Route("Admin")]
    [AdminRole]
    public class ProductImageController : Controller
    {
        HttpClient Client;
        string domain = "https://localhost:44374/";
        string apiProductImage = "api/admin/products/image";
        string apiProduct = "/api/admin/products";
        string apiEditImage = "api/admin/ProductImage";
        public IActionResult Index()
        {
            return View();
        }
        [Route("ProductImage/Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(domain);
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "LoginAuth");
            }
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string productImageJson = await Client.GetStringAsync(apiProductImage + "/" + id);
            ProductImage productImage = JsonConvert.DeserializeObject<ProductImage>(productImageJson);
            //lấy productName
            string productJson = await Client.GetStringAsync(apiProduct + "/" + id);
            Product product = JsonConvert.DeserializeObject<Product>(productJson);
            ViewBag.product = product;
            ViewBag.productImage = productImage;
            ViewBag.domain = domain;
            return View("Views/Admin/ProductImage/Edit.cshtml");
        }
        [Route("ProductImage/Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, IFormFile? fileImage, IFormFile? fileThumb)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(domain);
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "LoginAuth");
            }
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string productImageJson = await Client.GetStringAsync(apiProductImage + "/" + id);
            ProductImage productImage = JsonConvert.DeserializeObject<ProductImage>(productImageJson);
            var formdata = new MultipartFormDataContent();
            // Kiểm tra và thêm file
            if (fileImage != null && fileImage.Length > 0)
            {
                formdata.Add(new StreamContent(fileImage.OpenReadStream()), "file", fileImage.FileName);
            }
            else if (fileThumb != null && fileThumb.Length > 0)
            {
                formdata.Add(new StreamContent(fileThumb.OpenReadStream()), "fileThumb", fileThumb.FileName);
            }
            else
            {
                return RedirectToAction("Edit", new { id });
            }

            var response = await Client.PutAsync(apiEditImage + "/" + id, formdata);
            if (response.IsSuccessStatusCode)
            {

                return RedirectToAction("Edit");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorMessage); // In ra nội dung lỗi từ API
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi sửa ảnh: " + errorMessage);
            }
            return View("Views/Admin/ProductImage/Edit.cshtml");
        }
        [Route("ProductImage/Delete")]
        public async Task<IActionResult> Delete(string id, string imagePath)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(domain);
                    var token = HttpContext.Session.GetString("JWTToken");
                    if (string.IsNullOrEmpty(token))
                    {
                        return RedirectToAction("Login", "LoginAuth");
                    }
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var url = $"{apiEditImage}/{id}?imagePath={imagePath}";
                    var response = await client.DeleteAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Edit", new { id });
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError(string.Empty, $"Có lỗi xảy ra khi xóa hình ảnh: {errorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Có lỗi xảy ra: {ex.Message}");
            }

            // Nếu có lỗi, hãy trả về view Error và truyền ModelState
            return View("Error", ModelState);
        }
    }
}
