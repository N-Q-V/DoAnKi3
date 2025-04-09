using CallApi.Models.Authen;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace CallApi.Controllers.Admin
{
    [Route("Admin")]
    [AdminRole]
    public class HomeController : Controller
    {
        HttpClient Client;
        string domain = "https://localhost:44374/";
        [Route("Home")]
        public IActionResult Index()
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
            var userId = HttpContext.Session.GetString("UserId");
            var userName = HttpContext.Session.GetString("UserName");
            ViewBag.userId = userId;
            ViewBag.userName = userName;
            return View("Views/Admin/Home/Index.cshtml");
        }
    }
}
