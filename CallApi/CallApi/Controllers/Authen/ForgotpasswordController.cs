using CallApi.Models.Authen;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using CallApi.Models.Admin.ViewModel;
using Newtonsoft.Json;
using NuGet.Common;
using Microsoft.Win32;

namespace CallApi.Controllers.Authen
{
    public class ForgotpasswordController : Controller
    {
        HttpClient Client;
        string domain = "https://localhost:44374";
        string apiForgotPass = "api/account";
        public IActionResult Forgotpassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(domain);

            if (ModelState.IsValid)
            {
                try
                {
                    // Attempt to get user information by email
                    string userJson = await Client.GetStringAsync(apiForgotPass + "/email/" + model.Email);
                    Users user = JsonConvert.DeserializeObject<Users>(userJson);

                    if (user != null)
                    {
                        // Generate reset token
                        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                        user.PasswordResetToken = token;
                        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);

                        var formdata = new MultipartFormDataContent();
                        formdata.Add(new StringContent(token), "PasswordResetToken");
                        formdata.Add(new StringContent(user.PasswordResetTokenExpiry.ToString()), "PasswordResetTokenExpiry");

                        // Send update request
                        var response = await Client.PutAsync(apiForgotPass + "/email/" + model.Email, formdata);
                        if (response.IsSuccessStatusCode)
                        {
                            var resetLink = Url.Action("ResetPassword", "Forgotpassword", new { token = token, email = model.Email }, protocol: HttpContext.Request.Scheme);
                            SendPasswordResetEmail(model.Email, resetLink);
                            return RedirectToAction("ForgotPasswordConfirmation", "Forgotpassword");
                        }
                    }
                }
                catch (HttpRequestException)
                {
                    ModelState.AddModelError(string.Empty, "Email không hợp lệ hoặc không thể tìm thấy.");
                }
                catch (JsonException)
                {
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi xử lý yêu cầu của bạn.");
                }
            }

            // If we got this far, something failed; redisplay form with error messages
            return View("Views/Forgotpassword/ForgotPassword.cshtml", model);
        }

        private void SendPasswordResetEmail(string email, string resetLink)
        {

            var fromAddress = new MailAddress("vietmoc1702@gmail.com", "Nguyễn Quốc Việt");
            var toAddress = new MailAddress(email);
            const string fromPassword = "Vietmoc1995@@";
            const string subject = "Khôi Phục Mật Khẩu";
            string body = $"Để khôi phục mật khẩu của bạn, vui lòng nhấp vào liên kết sau: {resetLink}";

            var smtp = new SmtpClient
            {
                Host = "smtp.outlook.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null)
            {
                return BadRequest("Token không hợp lệ.");
            }

            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View("Views/Forgotpassword/ResetPassword.cshtml", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(domain);

            if (ModelState.IsValid)
            {
                // Kiểm tra người dùng từ API
                string userJson = await Client.GetStringAsync(apiForgotPass + "/email/" + model.Email);
                Users user = JsonConvert.DeserializeObject<Users>(userJson);

                if (user != null && user.PasswordResetToken == model.Token && user.PasswordResetTokenExpiry > DateTime.UtcNow)
                {
                    user.PasswordResetTokenExpiry = DateTime.Now.AddMinutes(30);
                    var formdata = new MultipartFormDataContent();
                    formdata.Add(new StringContent(model.Password), "PasswordHash");
                    formdata.Add(new StringContent(Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))), "PasswordResetToken");
                    formdata.Add(new StringContent(user.PasswordResetTokenExpiry.Value.ToString("o")), "PasswordResetTokenExpiry");
                    var response = await Client.PutAsync(apiForgotPass + "/user/" + model.Email, formdata);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Đặt lại mật khẩu thành công!";
                        return RedirectToAction("Login", "LoginAuth");
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        ViewBag.ErrorMessage = string.IsNullOrWhiteSpace(errorMessage) ?
                                               "Đã xảy ra lỗi" :
                                               errorMessage;
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Token không hợp lệ hoặc đã hết hạn.");
                }
            }

            // Nếu ModelState không hợp lệ hoặc có lỗi, vẫn ở lại trang ResetPassword
            return View(model);
        }

    }
}
