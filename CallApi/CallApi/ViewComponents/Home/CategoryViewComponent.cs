using CallApi.Models.Admin;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CallApi.ViewComponents.Home
{
    public class CategoryViewComponent : ViewComponent
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
            string categoryJson = await _client.GetStringAsync(_apiCategory);
            List<Category> categories = JsonConvert.DeserializeObject<List<Category>>(categoryJson);
            ViewBag.Categories = categories;

            string productJson = await _client.GetStringAsync(_apiProduct);
            List<Product> products = JsonConvert.DeserializeObject<List<Product>>(productJson);

            var filteredProducts = products
               .Where(p => p.Status == true && p.ProductType == true)
               .ToList();

            var productImages = new Dictionary<string, string>();

            foreach (var product in filteredProducts)
            {
                string productImageJson = await _client.GetStringAsync($"{_apiProductImage}/{product.ProductId}");
                ProductImage productImage = JsonConvert.DeserializeObject<ProductImage>(productImageJson);
                if (productImage != null)
                {
                    productImages[product.ProductId] = productImage.Thumb;
                }
            }
            // Add images to products
            ViewBag.ProductImages = productImages;
            ViewBag.FilteredProducts = filteredProducts;
            return View();
        }
    }
}
