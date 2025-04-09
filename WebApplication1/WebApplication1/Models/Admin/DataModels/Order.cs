namespace WebApplication1.Models.Admin.DataModels
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public Users? User { get; set; }
        public string UserName { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public double TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string ShippingAddress { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
