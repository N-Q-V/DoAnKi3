using WebApplication1.Models.Admin.DataModels;

namespace WebApplication1.Models.Admin.ViewModels
{
    public class OrderViewModel
    {
        public int UserId { get; set; } 
        public string UserName { get; set; }
        public double TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string ShippingAddress { get; set; } 
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public List<CartItemViewModel>? CartItems { get; set; } 
    }
}
