using CallApi.Models.Admin;
using CallApi.Models.Admin.ViewModel;
using CallApi.Models.Authen;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CallApi.Controllers.Admin
{
    [Route("Admin")]
    [AdminRole]
    public class ProductColorController : Controller
    {
        HttpClient Client;
        string domain = "https://localhost:44374/";
        string apiProductColor = "api/admin/productColor";
        string apiColor = "api/admin/colors";

        public IActionResult Index()
        {
            return View();
        }
        [Route("ProductColor/{id}")]
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
            string productColorJson = await Client.GetStringAsync(apiProductColor + "/" + id);
            ProductColorViewModel productColor = JsonConvert.DeserializeObject<ProductColorViewModel>(productColorJson);

            //Màu
            string colorJson = await Client.GetStringAsync(apiColor);
            List<Colors> colors = JsonConvert.DeserializeObject<List<Colors>>(colorJson);
            ViewBag.colors = colors;
            return View("Views/Admin/ProductColor/Edit.cshtml", productColor);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("ProductColor/{id}")]
        public async Task<IActionResult> Edit(int id, ProductColorViewModel p, string ProColorNameOld)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(domain);
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "LoginAuth");
            }
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string colorJson = await Client.GetStringAsync(apiColor);
            List<Colors> colors = JsonConvert.DeserializeObject<List<Colors>>(colorJson);
            ViewBag.colors = colors;

            var formdata = new MultipartFormDataContent();
            IEnumerable<string> checkbox = Request.Form["ProColorName"];
            List<string> productColors = checkbox.ToList();
            p.ProColorName = string.Join(", ", productColors);
            if (p.ProColorName.Length > 0 && p.ProColorName != null)
            {
                formdata.Add(new StringContent(p.ProColorName), "proColorName");
            }
            else
            {
                formdata.Add(new StringContent(ProColorNameOld), "proColorName");
            }
            var response = await Client.PutAsync(apiProductColor + "/" + id, formdata);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Sửa thành công";
                return RedirectToAction("Edit", new { p.ProductId });
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra: " + errorMessage);
            }
            return View("Views/Admin/ProductColor/Edit.cshtml", p);
        }
    }
}
