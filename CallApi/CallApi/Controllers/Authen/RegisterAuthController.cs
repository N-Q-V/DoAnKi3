using CallApi.Models.Admin.ViewModel;
using CallApi.Models.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using CallApi.Models.Authen;
using NuGet.Protocol.Plugins;
using System.Reflection;
using CallApi.Models.User;
using CallApi.Models.User.ViewModel;

namespace CallApi.Controllers.Authen
{
    public class RegisterAuthController : Controller
    {
        HttpClient Client;
        string domain = "https://localhost:44374/";
        string register = "/api/account";
        string apiProduct = "/api/product";
        string apiOrderById = "api/Cart/Order";
        string apiOrder = "api/Cart/GetOrderByUserId";
        string apiOrderItem = "api/Cart/OrderItem";
        string apiProductImage = "api/UserProducts/image";
        public async Task<IActionResult> Register()
        {
            return View();
        }
        public async Task<IActionResult> ProfileReceipt()
        {
            var userId = HttpContext.Session.GetString("UserId");
            ViewBag.userId = userId;
            Client = new HttpClient();
            Client.BaseAddress = new Uri(domain);
            string userJson = await Client.GetStringAsync(register + "/" + userId);
            var user = JsonConvert.DeserializeObject<UpdateUserViewModel>(userJson);
            string cartJson = await Client.GetStringAsync(apiOrder + "/" + userId);
            var carts = JsonConvert.DeserializeObject<List<Cart>>(cartJson);
            ViewBag.user = user;
            ViewBag.carts = carts;
            return View("Views/Profile/ProfileReceipt.cshtml");
        }
        public async Task<IActionResult> ProfileReceiptDetails(int id)
        {
            var userId = HttpContext.Session.GetString("UserId");
            ViewBag.userId = userId;
            ViewBag.domain = domain;
            Client = new HttpClient();
            Client.BaseAddress = new Uri(domain);

            string userJson = await Client.GetStringAsync(register + "/" + userId);
            var user = JsonConvert.DeserializeObject<UpdateUserViewModel>(userJson);
            ViewBag.user = user;

            string orderItemJson = await Client.GetStringAsync(apiOrderItem + "/" + id);
            var orderItems = JsonConvert.DeserializeObject<List<CartItem>>(orderItemJson);
            ViewBag.orderItems = orderItems;

            string orderJson = await Client.GetStringAsync(apiOrderById + "/" + id);
            Cart order = JsonConvert.DeserializeObject<Cart>(orderJson);
            ViewBag.order = order;

            string productImageJson = await Client.GetStringAsync(apiProductImage);
            var productImage = JsonConvert.DeserializeObject<List<ProductImage>>(productImageJson);
            ViewBag.productImage = productImage;

            return View("Views/Profile/ProfileReceiptDetails.cshtml", orderItems);
        }
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetString("UserId");
            ViewBag.userId = userId;

            Client = new HttpClient();
            Client.BaseAddress = new Uri(domain);
            string userJson = await Client.GetStringAsync(register + "/" + userId);
            UpdateUserViewModel user = JsonConvert.DeserializeObject<UpdateUserViewModel>(userJson);
            return View("Views/Profile/Profile.cshtml", user);
        }
        public IActionResult UpdateUser()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(UpdateUserViewModel u)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Profile", new { userId = u.UserId });
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(domain);

                var formdata = new MultipartFormDataContent
                {
                    { new StringContent(u.FullName), "FullName" },
                    { new StringContent(u.Phonenumber), "Phonenumber" },
                    { new StringContent(u.Adress), "Adress" }
                };
                var response = await client.PutAsync($"{register}/{u.UserId}", formdata);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Sửa thành công!";
                    return RedirectToAction("Profile", new { userId = u.UserId });
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(errorMessage); 
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra: " + errorMessage);
                }
            }
            return RedirectToAction("Profile", new { userId = u.UserId });
        }



        [HttpPost]
        public async Task<IActionResult> Register(Users u)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(domain);

                var registerData = new
                {
                    Username = u.Username,
                    FullName = u.FullName,
                    PasswordHash = u.PasswordHash,
                    Email = u.Email,
                    Phonenumber = u.Phonenumber,
                    Adress = u.Adress
                };

                var response = await client.PostAsJsonAsync(register, registerData);

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage = "Đăng ký thành công!";
                    return View("register");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ViewBag.ErrorMessage = string.IsNullOrWhiteSpace(errorMessage) ?
                                           "Đã xảy ra lỗi khi đăng ký." :
                                           errorMessage;
                    return View("register");
                }
            }
        }

    }
}
