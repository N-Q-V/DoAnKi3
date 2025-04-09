using CallApi.Models.Admin;
using CallApi.Models.Authen;
using CallApi.Models.User;
using CallApi.Models.User.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Text;

namespace CallApi.Controllers.User
{
    public class CartController : Controller
    {
        HttpClient Client;
        string domain = "https://localhost:44374/";
        string apiProduct = "/api/UserProducts";
        string apiProductImage = "api/UserProducts/image";
        string apiCart = "api/Cart";
        string apiUser = "/api/account";
        private const string CartSessionKey = "Cart";
        public IActionResult Index()
        {
            ViewBag.domain = domain;
            var cart = GetCart();
            return View("Views/User/ShoppingCart/Index.cshtml", cart);
        }
        public async Task<IActionResult> Checkout(UpdateUserViewModel u)
        {
            var userId = HttpContext.Session.GetString("UserId");
            ViewBag.userId = userId;
            var cart = GetCart();
            if (cart.Items.Count == 0) 
            {
                ViewBag.Message = "Giỏ hàng của bạn hiện đang rỗng. Hãy mua hàng trước khi thanh toán.";
                return View("Views/User/ShoppingCart/EmptyCart.cshtml");
            }
            Client = new HttpClient();
            Client.BaseAddress = new Uri(domain);

            if (userId == null)
            {
                string userJson = await Client.GetStringAsync(apiUser + "/" + 1);
                UpdateUserViewModel user = JsonConvert.DeserializeObject<UpdateUserViewModel>(userJson);
                ViewBag.user = user;
            }
            else
            {
                string userJson = await Client.GetStringAsync(apiUser + "/" + userId);
                UpdateUserViewModel user = JsonConvert.DeserializeObject<UpdateUserViewModel>(userJson);
                ViewBag.user = user;
            }
            return View("Views/User/ShoppingCart/Checkout.cshtml", cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Cart o, List<CartItem> CartItems)
        {
            using var client = new HttpClient(); 
            client.BaseAddress = new Uri(domain);
            if (ModelState.IsValid)
            {
                var formdata = new MultipartFormDataContent();
                formdata.Add(new StringContent(o.UserId.ToString()), "UserId");
                formdata.Add(new StringContent(o.UserName), "UserName");
                formdata.Add(new StringContent(o.TotalAmount.ToString()), "TotalAmount");
                formdata.Add(new StringContent(o.PaymentStatus), "PaymentStatus");
                formdata.Add(new StringContent(o.ShippingAddress), "ShippingAddress");
                formdata.Add(new StringContent(o.Phone.ToString()), "Phone");
                formdata.Add(new StringContent(o.Email), "Email");
                formdata.Add(new StringContent(o.Status), "Status");

                foreach (var item in CartItems)
                {
                    formdata.Add(new StringContent(item.ProductId.ToString()), $"CartItems[{CartItems.IndexOf(item)}].ProductId");
                    formdata.Add(new StringContent(item.ProductName), $"CartItems[{CartItems.IndexOf(item)}].ProductName");
                    formdata.Add(new StringContent(item.Color), $"CartItems[{CartItems.IndexOf(item)}].Color");
                    formdata.Add(new StringContent(item.Quantity.ToString()), $"CartItems[{CartItems.IndexOf(item)}].Quantity");
                    formdata.Add(new StringContent(item.Price.ToString()), $"CartItems[{CartItems.IndexOf(item)}].Price");
                }
                var response = await client.PostAsync(apiCart, formdata);
                if (response.IsSuccessStatusCode)
                {
                    HttpContext.Session.Remove("Cart");
                    return RedirectToAction("Success");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Error: {errorContent}");
                    return View("Views/User/ShoppingCart/Checkout.cshtml", o);
                }
            }
            else
            {
                return View("Views/User/ShoppingCart/Checkout.cshtml", o);
            }
        }
        public ActionResult Success()
        {
            return View("Views/User/ShoppingCart/Success.cshtml");
        }

        private Cart GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new Cart();
            }
            return JsonConvert.DeserializeObject<Cart>(cartJson);
        }
        public async Task<IActionResult> AddToCart(string productId, int quantity, string color)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(domain);

            var cart = GetCart();
            string productJson = await Client.GetStringAsync(apiProduct + "/" + productId);
            Product product = JsonConvert.DeserializeObject<Product>(productJson);
            string productImageJson = await Client.GetStringAsync(apiProductImage + "/" + productId);
            ProductImage productImage = JsonConvert.DeserializeObject<ProductImage>(productImageJson);
            if (product != null)
            {
                cart.AddItem(product, quantity, color, productImage);
                SaveCart(cart);
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> UpdateCart(string productId, int quantity)
        {
            var cart = GetCart();
            string productJson = await Client.GetStringAsync(apiProduct + "/" + productId);
            Product product = JsonConvert.DeserializeObject<Product>(productJson);

            if (product != null)
            {
                var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (item != null)
                {
                    if (quantity > 0)
                    {
                        item.Quantity = quantity;
                    }
                    else
                    {
                        cart.Items.Remove(item); // Remove item if quantity is zero or less
                    }
                    SaveCart(cart);
                }
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult UpdateCart(Dictionary<string, int> quantities)
        {
            var cart = GetCart();
            foreach (var item in cart.Items.ToList())
            {
                if (quantities.TryGetValue(item.ProductId, out int newQuantity))
                {
                    if (newQuantity > 0)
                    {
                        item.Quantity = newQuantity;
                    }
                    else
                    {
                        cart.Items.Remove(item); // Remove item if quantity is zero or less
                    }
                }
            }
            SaveCart(cart);
            return RedirectToAction("Index");
        }
        private void SaveCart(Cart cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }
        public IActionResult RemoveFromCart(string productId)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item != null)
            {
                cart.Items.Remove(item);
                SaveCart(cart);
            }

            return RedirectToAction("Index");
        }
    }
}
