using CallApi.Models.Admin;
using CallApi.Models.Admin.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using X.PagedList;
using System;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;
using CallApi.Models.Authen;

namespace CallApi.Controllers.Admin
{
    [Route("Admin")]
    [AdminRole]
    public class ProductController : Controller
    {
        HttpClient Client;
        string domain = "https://localhost:44374/";
        string apiProduct = "/api/admin/products";
        string apiCategory = "/api/admin/categories";
        string apiProductImage = "api/admin/products/image";
        string apiSearch = "api/admin/products/search";
        string apiColor = "api/admin/colors";
        [Route("Product")]
        public async Task<IActionResult> Index(string? name, int? categoryId, bool? status, int page = 1)
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
            //lấy category
            string categoryJson = await Client.GetStringAsync(apiCategory);
            List<Category> categories = JsonConvert.DeserializeObject<List<Category>>(categoryJson);
            ViewBag.categories = categories;
            ViewBag.categoryId = new SelectList(categories, "CategoryId", "CategoryName", categoryId);

            //lấy name
            ViewBag.name = name;
            ViewBag.selectedCatId = categoryId;
            ViewBag.selectedStatus = status;
            //lấy status làm selectlist
            ViewBag.status = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "Còn hàng",Selected = status == true},
                new SelectListItem { Value = "false", Text = "Hết hàng",Selected = status == false}
            };
            if (!string.IsNullOrEmpty(name) || categoryId.HasValue || status.HasValue)
            {
                apiSearch += "?";
                if (!string.IsNullOrEmpty(name))
                {
                    apiSearch += $"name={name}";
                }
                if (categoryId.HasValue)
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        apiSearch += "&";
                    }
                    apiSearch += $"categoryId={categoryId.Value}";
                }
                if (status.HasValue)
                {
                    if (!string.IsNullOrEmpty(name) || categoryId.HasValue)
                    {
                        apiSearch += "&";
                    }
                    apiSearch += $"status={status.Value}";
                }
            }
            // Gọi API
            var response = await Client.GetAsync(apiSearch);
            if (response.IsSuccessStatusCode)
            {
                int pagesize = 5;
                var jsonString = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<Product>>(jsonString).ToPagedList(page,pagesize);
                return View("Views/Admin/Product/Index.cshtml", products);
            }
            return View("Error");
        }
        [HttpGet]
        [Route("Product/{id?}")]
        public async Task<IActionResult> Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
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
                string productJson = await Client.GetStringAsync(apiProduct + "/" + id);
                Product product = JsonConvert.DeserializeObject<Product>(productJson);

                string productImageJson = await Client.GetStringAsync(apiProductImage + "/" + id);
                ProductImage productImage = JsonConvert.DeserializeObject<ProductImage>(productImageJson);
                ViewBag.productImage = productImage;
                ViewBag.domain = domain;
                if (product == null)
                {
                    return NotFound();
                }

                return View("Views/Admin/Product/Detail.cshtml", product);
            }
            catch (HttpRequestException e)
            {
                return BadRequest(e.Message);
            }
        }
        [Route("Product/Create")]
        public async Task<IActionResult> Create()
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(domain);
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "LoginAuth");
            }
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            //color
            string colorJson = await Client.GetStringAsync(apiColor);
            List<Colors> colors = JsonConvert.DeserializeObject<List<Colors>>(colorJson);
            ViewBag.colors = colors;
            //category
            string categoryJson = await Client.GetStringAsync(apiCategory);
            List<Category> categories = JsonConvert.DeserializeObject<List<Category>>(categoryJson);
            if (categories == null || !categories.Any())
            {
                // Xử lý khi không có dữ liệu, có thể trả về thông báo lỗi hoặc danh sách trống
                ModelState.AddModelError("", "No categories available. Please add a category before creating a product.");
                ViewBag.CategoryId = new SelectList(Enumerable.Empty<SelectListItem>());
            }
            else
            {
                // Tạo SelectList từ danh sách categories
                ViewBag.CategoryId = new SelectList(categories, "CategoryId", "CategoryName");
            }
            return View("Views/Admin/Product/Create.cshtml");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Product/Create")]
        public async Task<IActionResult> Create(Product p, IFormFile file, IFormFile[] fileImages, ProductColorViewModel pc)
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
                formdata.Add(new StringContent(p.ProductId), "productId");
                formdata.Add(new StringContent(p.ProductName), "productName");
                formdata.Add(new StringContent(p.Price.ToString()), "price");
                formdata.Add(new StringContent(p.CategoryId.ToString()), "categoryId");
                formdata.Add(new StringContent(p.ProductType.ToString()), "productType");
                formdata.Add(new StringContent(p.Status.ToString()), "status");
                formdata.Add(new StringContent(p.Display), "display");
                formdata.Add(new StringContent(p.FrontCamera), "frontCamera");
                formdata.Add(new StringContent(p.RearCamera), "rearCamera");
                formdata.Add(new StringContent(p.Cpu), "cpu");
                formdata.Add(new StringContent(p.System), "system");
                formdata.Add(new StringContent(p.ConnectType), "connectType");
                formdata.Add(new StringContent(p.Memory), "memory");
                formdata.Add(new StringContent(p.CreatAt.ToString()), "creatAt");
                // Kiểm tra và thêm file
                if (file != null && file.Length > 0)
                {
                    formdata.Add(new StreamContent(file.OpenReadStream()), "file", file.FileName);
                }
                //ảnh phụ
                if (fileImages != null && fileImages.Length > 0)
                {
                    foreach (var item in fileImages)
                    {
                        var fileContent = new StreamContent(item.OpenReadStream());
                        formdata.Add(fileContent, "fileImages", item.FileName);
                    }
                }
                //thêm màu
                IEnumerable<string> checkbox = Request.Form["ProColorName"];
                List<string> colors = checkbox.ToList();
                pc.ProColorName = string.Join(", ", colors);
                formdata.Add(new StringContent(pc.ProColorName), "proColorName");

                var response = await Client.PostAsync(apiProduct, formdata);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Thêm mới thành công";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi thêm sản phẩm: " + errorMessage);
                }
            }
            string categoryJson = await Client.GetStringAsync(apiCategory);
            List<Category> categories = JsonConvert.DeserializeObject<List<Category>>(categoryJson);
            ViewBag.CategoryId = new SelectList(categories, "CategoryId", "CategoryName");
            return View("Views/Admin/Product/Create.cshtml", p);
        }
        [Route("Product/Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
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
                string productJson = await Client.GetStringAsync(apiProduct + "/" + id);
                ProductViewModel product = JsonConvert.DeserializeObject<ProductViewModel>(productJson);
                string categoryJson = await Client.GetStringAsync(apiCategory);
                List<Category> categories = JsonConvert.DeserializeObject<List<Category>>(categoryJson);
                ViewBag.CategoryId = new SelectList(categories, "CategoryId", "CategoryName");

                string productImageJson = await Client.GetStringAsync(apiProductImage + "/" + id);
                ProductImage productImage = JsonConvert.DeserializeObject<ProductImage>(productImageJson);
                ViewBag.productImage = productImage;
                ViewBag.domain = domain;
                if (product == null)
                {
                    return NotFound();
                }

                return View("Views/Admin/Product/Edit.cshtml", product);
            }
            catch (HttpRequestException e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Product/Edit/{id}")]
        public async Task<IActionResult> Edit(ProductViewModel p)
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
                formdata.Add(new StringContent(p.ProductId), "productId");
                formdata.Add(new StringContent(p.ProductName), "productName");
                formdata.Add(new StringContent(p.Price.ToString()), "price");
                formdata.Add(new StringContent(p.CategoryId.ToString()), "categoryId");
                formdata.Add(new StringContent(p.ProductType.ToString()), "productType");
                formdata.Add(new StringContent(p.Status.ToString()), "status");
                formdata.Add(new StringContent(p.Display), "display");
                formdata.Add(new StringContent(p.FrontCamera), "frontCamera");
                formdata.Add(new StringContent(p.RearCamera), "rearCamera");
                formdata.Add(new StringContent(p.Cpu), "cpu");
                formdata.Add(new StringContent(p.System), "system");
                formdata.Add(new StringContent(p.ConnectType), "connectType");
                formdata.Add(new StringContent(p.Memory), "memory");
                formdata.Add(new StringContent(p.CreatAt.ToString()), "creatAt");

                var response = await Client.PutAsync(apiProduct + "/" + p.ProductId, formdata);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Sửa thành công";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(errorMessage); // In ra nội dung lỗi từ API
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi sửa sản phẩm: " + errorMessage);
                }
            }
            string categoryJson = await Client.GetStringAsync(apiCategory);
            List<Category> categories = JsonConvert.DeserializeObject<List<Category>>(categoryJson);
            ViewBag.CategoryId = new SelectList(categories, "CategoryId", "CategoryName");
            return View("Views/Admin/Product/Edit.cshtml", p);
        }
        [Route("Product/Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(domain);
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "LoginAuth");
            }
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await Client.DeleteAsync(apiProduct + "/" + id);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Xóa thành công";
            }
            return RedirectToAction("Index");
        }
    }
}
