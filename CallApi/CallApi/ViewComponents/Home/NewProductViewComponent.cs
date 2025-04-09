using CallApi.Models.Admin;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CallApi.ViewComponents.Home
{
    public class NewProductViewComponent : ViewComponent
    {
        HttpClient _client;
        string _domain = "https://localhost:44374/";
        string _apiCategory = "/api/UserCategories";
        string _apiProduct = "/api/UserProducts";
        string _apiProductImage = "api/UserProducts/image";
        public async Task<IViewComponentResult> InvokeAsync()
        {
            _client = new HttpClient { BaseAddress = new Uri(_domain) };
            ViewBag.domain = _domain;
            string productJson = await _client.GetStringAsync(_apiProduct);
            List<Product> products = JsonConvert.DeserializeObject<List<Product>>(productJson).OrderBy(p => p.CreatAt).Take(4).ToList();
            string productImageJson = await _client.GetStringAsync(_apiProductImage);
            List<ProductImage> productImages = JsonConvert.DeserializeObject<List<ProductImage>>(productImageJson);

            ViewBag.productImages = productImages;

            return View(products);
        }
    }
}
