using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CallApi.Models.Authen
{
    public class Users
    {
        public int UserId { get; set; }
        [DisplayName("Tên người dùng")]
        [Required(ErrorMessage = "Tên người dùng không được để trống.")]
        [StringLength(200, ErrorMessage = "Tên người dùng không được vượt quá 200 ký tự.")]
        public string Username { get; set; }

        [DisplayName("Họ và tên")]
        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        [StringLength(200, ErrorMessage = "Họ và tên không được vượt quá 200 ký tự.")]
        public string FullName { get; set; }
        [DisplayName("Email người dùng")]
        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Hãy điền đúng kiểu email")]
        public string Email { get; set; }
        [DisplayName("Số điện thoại người dùng")]
        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [Phone(ErrorMessage = "Hãy điền đúng kiểu số điện thoại")]
        public string Phonenumber { get; set; }
        [DisplayName("Mật khẩu người dùng")]
        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        public string PasswordHash { get; set; }
        [DisplayName("Địa chỉ người dùng")]
        [Required(ErrorMessage = "Địa chỉ không được để trống.")]
        public string Adress {  get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
    }
}
