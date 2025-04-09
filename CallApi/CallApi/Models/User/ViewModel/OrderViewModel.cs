namespace CallApi.Models.User.ViewModel
{
    public class OrderViewModel
    {
        public int UserId { get; set; }
        public double TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string ShippingAddress { get; set; }
        public List<CartItem>? CartItems { get; set; }
    }
}
