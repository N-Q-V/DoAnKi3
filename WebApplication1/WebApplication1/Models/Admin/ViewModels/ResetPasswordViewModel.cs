using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Admin.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string PasswordHash { get; set; }
        public string PasswordResetToken { get; set; }
        public DateTime PasswordResetTokenExpiry { get; set; }
    }
}
