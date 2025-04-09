using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CallApi.Models.Authen
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        
        public string Username { get; set; }
        
        public string Email { get; set; }
        
        public string Phonenumber { get; set; }
        
        public string PasswordHash { get; set; }
        
        public string Adress { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
