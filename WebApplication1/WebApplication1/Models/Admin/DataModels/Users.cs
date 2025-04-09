using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Admin.DataModels
{
    [Table("users")]
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phonenumber { get; set; }
        public string PasswordHash { get; set; }
        public string Adress {  get; set; }
        public int Role { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
