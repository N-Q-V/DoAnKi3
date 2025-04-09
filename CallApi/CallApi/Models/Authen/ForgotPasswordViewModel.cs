using System.ComponentModel.DataAnnotations;

namespace CallApi.Models.Authen
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
