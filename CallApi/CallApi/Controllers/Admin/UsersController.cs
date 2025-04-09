using CallApi.Models.Admin;
using CallApi.Models.Authen;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CallApi.Controllers.Admin
{
    [Route("Admin")]
    public class UsersController : Controller
    {
        HttpClient Client;
        string domain = "https://localhost:44374/";
        string apiUser = "/api/account";
        [Route("User")]
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
            string userJson = await Client.GetStringAsync(apiUser);
            List<Users> users = JsonConvert.DeserializeObject<List<Users>>(userJson);
            return View("Views/Admin/Users/Index.cshtml", users);
        }
        [Route("User/Delete")]
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
            var response = await Client.DeleteAsync(apiUser + "/" + id);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Xóa thành công";
            }
            return RedirectToAction("Index");
        }
    }
}
