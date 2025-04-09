using CallApi.Models.Admin;
using CallApi.Models.Authen;
using CallApi.Models.User;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Xml.Linq;
using X.PagedList;

namespace CallApi.Controllers.Admin
{
    [Route("Admin")]
    [AdminRole]
    public class OrderController : Controller
    {
        HttpClient Client;
        string domain = "https://localhost:44374/";
        string apiOrder = "api/Cart/Order";
        string apiOrderItem = "api/Cart/OrderItem";
        string apiOrderSearch = "api/Cart/Order/Search";
        [Route("Order")]
        public async Task<IActionResult> Index(string? phone, string? status, string? paymentStatus, int page = 1)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(domain);
            ViewBag.phone = phone;
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "LoginAuth");
            }
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var queryParameters = new List<string>();

            if (!string.IsNullOrEmpty(phone))
            {
                queryParameters.Add($"phone={Uri.EscapeDataString(phone)}");
            }
            if (!string.IsNullOrEmpty(status))
            {
                queryParameters.Add($"status={Uri.EscapeDataString(status)}");
            }
            if (!string.IsNullOrEmpty(paymentStatus))
            {
                queryParameters.Add($"paymentStatus={Uri.EscapeDataString(paymentStatus)}");
            }

            if (queryParameters.Count > 0)
            {
                apiOrderSearch += "?" + string.Join("&", queryParameters);
            }

            var response = await Client.GetAsync(apiOrderSearch);

            if (response.IsSuccessStatusCode)
            {
                int pagesize = 5;
                string orderJson = await response.Content.ReadAsStringAsync();
                var orderList = JsonConvert.DeserializeObject<List<Cart>>(orderJson).ToPagedList(page, pagesize);
                return View("Views/Admin/Order/Index.cshtml", orderList);
            }
            return View("Error");
        }

        [Route("OrderItem/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(domain);
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "LoginAuth");
            }
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string orderItemJson = await Client.GetStringAsync(apiOrderItem + "/" + id);
            List<CartItem> orderItem = JsonConvert.DeserializeObject<List<CartItem>>(orderItemJson);
            return View("Views/Admin/Order/Detail.cshtml", orderItem);
        }
        [Route("Order/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Cart c, int id)
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
                formdata.Add(new StringContent(c.OrderId.ToString()), "OrderId");
                formdata.Add(new StringContent(c.UserName), "UserName");
                formdata.Add(new StringContent(c.UserId.ToString()), "UserId");
                formdata.Add(new StringContent(c.Email), "Email");
                formdata.Add(new StringContent(c.OrderDate.ToString()), "OrderDate");
                formdata.Add(new StringContent(c.ShippingAddress), "ShippingAddress");
                formdata.Add(new StringContent(c.Status), "Status");
                formdata.Add(new StringContent(c.PaymentStatus), "PaymentStatus");
                formdata.Add(new StringContent(c.Phone), "Phone");
                formdata.Add(new StringContent(c.TotalAmount.ToString()), "TotalAmount");


                var response = await Client.PutAsync(apiOrder + "/" + c.OrderId, formdata);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Cập nhật thành công";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(errorMessage);
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật: " + errorMessage);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
