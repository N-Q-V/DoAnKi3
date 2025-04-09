using CallApi.Models.Admin;
using CallApi.Models.Admin.ViewModel;
using CallApi.Models.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Xml.Linq;
using X.PagedList;

namespace CallApi.Controllers.User
{
    public class UserProductController : Controller
    {
        HttpClient Client;
        string domain = "https://localhost:44374/";
        string apiProduct = "/api/UserProducts";
        string apiCategory = "/api/UserCategories";
        string apiProductImage = "api/UserProducts/image";
        string apiProductColor = "api/UserProductColor";
        string apiSearch = "api/UserProducts/search";
        string apiSearch2 = "api/UserProducts/search2";
        string apiComment = "api/comment";
        public async Task<IActionResult> Index(int? categoryId, string? system, string? name, int page = 1)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(domain);
            ViewBag.domain = domain;
            string categoryJson = await Client.GetStringAsync(apiCategory);
            List<Category> categories = JsonConvert.DeserializeObject<List<Category>>(categoryJson);
            ViewBag.Categories = categories;
            ViewBag.categoryId = new SelectList(categories, "CategoryId", "CategoryName", categoryId);

            apiSearch2 += "?";
            if (!string.IsNullOrEmpty(system))
            {
                apiSearch2 += $"system={system}";
            }
            if (!string.IsNullOrEmpty(name))
            {
                apiSearch2 += $"name={name}";
            }
            if (categoryId.HasValue)
            {
                if (!string.IsNullOrEmpty(system))

                {
                    apiSearch2 += "&";
                }
                apiSearch2 += $"categoryId={categoryId.Value}";
            }
            var response = await Client.GetAsync(apiSearch2);
            if (response.IsSuccessStatusCode)
            {
                int pagesize = 3;
                string productJson = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<Product>>(productJson).ToPagedList(page, pagesize);

                string productImageJson = await Client.GetStringAsync(apiProductImage);
                List<ProductImage> productImages = JsonConvert.DeserializeObject<List<ProductImage>>(productImageJson);

                ViewBag.productImages = productImages;
                ViewBag.products = products;

                return View("Views/User/Product/Index.cshtml", products);
            }
            return View("Error");
        }
        public async Task<IActionResult> Detail(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return NotFound();
            }
            Client = new HttpClient();
            Client.BaseAddress = new Uri(domain);
            ViewBag.domain = domain;
            string productJson = await Client.GetStringAsync(apiProduct + "/" + productId);
            Product product = JsonConvert.DeserializeObject<Product>(productJson);

            string productImageJson = await Client.GetStringAsync(apiProductImage + "/" + productId);
            ProductImage productImage = JsonConvert.DeserializeObject<ProductImage>(productImageJson);

            string productColorJson = await Client.GetStringAsync(apiProductColor + "/" + productId);
            ProductColorViewModel productColor = JsonConvert.DeserializeObject<ProductColorViewModel>(productColorJson);

            string commentJson = await Client.GetStringAsync(apiComment + "/" + productId);
            List<ProductComments> productComments = JsonConvert.DeserializeObject<List<ProductComments>>(commentJson);

            ViewBag.productColor = productColor;
            ViewBag.product = product;
            ViewBag.productImage = productImage;
            ViewBag.comment = productComments;
            ViewBag.domain = domain;
            if (product == null)
            {
                return NotFound();
            }
            return View("Views/User/Product/Detail.cshtml");
        }

        public async Task<IActionResult> Search(string system)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(domain);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CreatComment(string productId, ProductComments p)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(domain);
            if (ModelState.IsValid)
            {
                var formdata = new MultipartFormDataContent();
                formdata.Add(new StringContent(p.ProductId), "productId");
                formdata.Add(new StringContent(p.CustomerName), "customerName");
                formdata.Add(new StringContent(p.Content), "content");
                var response = await Client.PostAsync(apiComment + "/" + p.ProductId, formdata);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Comment thành công";
                    return RedirectToAction("Detail", new { productId = p.ProductId });
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra: " + errorMessage);
                }
            }
            return RedirectToAction("Detail", new { productId = p.ProductId });
        }
    }
}
