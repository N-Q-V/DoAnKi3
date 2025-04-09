using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApplication1.Models.Admin.BusinesModels;
using WebApplication1.Models.Admin.DataModels;
using WebApplication1.Models.Admin.ViewModels;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        MobileDbContext _context;
        IConfiguration _config;
        public AccountController(MobileDbContext context, IConfiguration config)
        {
            this._context = context;
            this._config = config;
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserModel model)
        {
            var passwordMd5 = GenerateMD5(model.PasswordHash);
            var user = _context.users.FirstOrDefault(x => x.Username == model.Username && x.PasswordHash == passwordMd5);

            if (user == null)
            {
                return NotFound(new { message = "Đăng nhập sai" });
            }

            string role = user.Role == 1 ? "Admin" : "User";
            var key = _config["Jwt:Key"];
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signingCredential = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Name, model.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.MobilePhone, user.Phonenumber),
                new Claim(ClaimTypes.Country, user.Adress)
            };

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                expires: DateTime.Now.AddHours(1),
                signingCredentials: signingCredential,
                claims: claims
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            return Ok(new { Token = tokenString });
        }

        //hàm mã hóa 1 chuỗi sử dụng MD5
        private string GenerateMD5(string input)
        {
            // Step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            // Step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id,[FromForm] UpdateUserViewModel model)
        {
            var data = await _context.users.FirstOrDefaultAsync(x => x.UserId == id);
            if (data == null)
            {
                return NotFound("User không tồn tại.");
            }
            data.FullName = model.FullName;;
            data.Phonenumber = model.Phonenumber;
            data.Adress = model.Adress;
            _context.users.Update(data);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> Register([FromBody] UserDTO model)
        {
            if (model == null)
            {
                return BadRequest("Model không hợp lệ.");
            }

            var passwordMd5 = GenerateMD5(model.PasswordHash);

            // Lấy người dùng trong DB
            var existingUser = await _context.users
                .FirstOrDefaultAsync(x => x.Username == model.Username || x.Email == model.Email || x.Phonenumber == model.Phonenumber);

            if (existingUser != null)
            {
                if (existingUser.Username == model.Username)
                {
                    return BadRequest("Tên người dùng đã tồn tại.");
                }
                if (existingUser.Email == model.Email)
                {
                    return BadRequest("Email đã được đăng kí.");
                }
                if (existingUser.Phonenumber == model.Phonenumber)
                {
                    return BadRequest("Số điện thoại đã được đăng kí.");
                }
            }

            var newUser = new Users
            {
                UserId = model.UserId,
                Username = model.Username,
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = passwordMd5,
                Phonenumber = model.Phonenumber,
                Adress = model.Adress, 
                Role = 0
            };

            _context.users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đăng ký thành công!" });
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.users.ToListAsync();
            return Ok(users);
        }
        [HttpGet("{UserId}")]
        public async Task<IActionResult> GetUserById(int UserId)
        {
            var data = await _context.users.FirstOrDefaultAsync(x => x.UserId == UserId);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var data = await _context.users.FirstOrDefaultAsync(x => x.Email == email);

            if (data == null)
            {
                return NotFound(); 
            }

            return Ok(data); 
        }

        [HttpPut("email/{email}")]
        public async Task<IActionResult> ForgotPass(string email, [FromForm] ForgotpasswordViewModel c)
        {

            var data = await _context.users.FirstOrDefaultAsync(x => x.Email == email);
            if (data == null)
            {
                return NotFound("User không tồn tại.");
            }
            // Cập nhật dữ liệu của danh mục

            data.PasswordResetToken = c.PasswordResetToken;
            data.PasswordResetTokenExpiry = c.PasswordResetTokenExpiry;
            _context.users.Update(data);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPut("user/{email}")]
        public async Task<IActionResult> ResetPassword(string email, [FromForm] ResetPasswordViewModel c)
        {
            var data = await _context.users.FirstOrDefaultAsync(x => x.Email == email);
            if (data == null)
            {
                return NotFound("User không tồn tại.");
            }
            var passwordMd5 = GenerateMD5(c.PasswordHash);
            data.PasswordHash = passwordMd5;
            data.PasswordResetToken = c.PasswordResetToken;
            data.PasswordResetTokenExpiry = c.PasswordResetTokenExpiry;
            _context.users.Update(data);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_context.users == null)
            {
                return NotFound();
            }
            var user = await _context.users.FindAsync(id);
            
            if (user == null)
            {
                return NotFound();
            }
            _context.users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
