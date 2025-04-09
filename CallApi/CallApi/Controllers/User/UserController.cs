using CallApi.Models.Admin;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CallApi.Controllers.User
{
    public class UserController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
