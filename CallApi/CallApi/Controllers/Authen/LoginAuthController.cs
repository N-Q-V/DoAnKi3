using Microsoft.AspNetCore.Mvc;
using CallApi.Models.Authen;
using Microsoft.AspNetCore.Authorization;
using NuGet.Common;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using System.Data;
namespace CallApi.Controllers.Authen
{
    public class LoginAuthController : Controller
    {
        HttpClient Client;
        string domain = "https://localhost:44374";
        string login = "/api/account/login";
        string apiProduct = "/api/product";
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(domain);

            var loginData = new { Username = model.Username, PasswordHash = model.PasswordHash };

            var loginResponse = await Client.PostAsJsonAsync(login, loginData);
            try
            {
                var token = await loginResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                HttpContext.Session.SetString("JWTToken", token["token"]);
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token["token"]);
                var jwtToken = token["token"];
                // Giải mã token để lấy UserId
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(jwtToken);
                var userIdClaim = jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
                var usernameClaim = jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
                var roleClaim = jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value;

                // Lưu UserId vào session
                if (userIdClaim != null)
                {
                    HttpContext.Session.SetString("UserId", userIdClaim.Value);
                    HttpContext.Session.SetString("UserName", usernameClaim.Value);
                    HttpContext.Session.SetString("UserRole", roleClaim);
                }
                if (roleClaim == "Admin")
                {
                    return RedirectToAction("Index", "Product");
                }
                else if (roleClaim == "User")
                {
                    return RedirectToAction("Index", "User");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng!";
                return View("Login");
            }
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("JWTToken");
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Login");
        }
    }
}
