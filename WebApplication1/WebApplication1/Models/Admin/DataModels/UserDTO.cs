namespace WebApplication1.Models.Admin.DataModels
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Phonenumber { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Adress { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
    }
}
