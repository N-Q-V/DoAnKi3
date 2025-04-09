namespace WebApplication1.Models.Admin.ViewModels
{
    public class ForgotpasswordViewModel
    {
        public string PasswordResetToken { get; set; }
        public DateTime PasswordResetTokenExpiry { get; set; }
    }
}
