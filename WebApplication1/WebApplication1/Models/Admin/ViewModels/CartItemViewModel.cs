namespace WebApplication1.Models.Admin.ViewModels
{
    public class CartItemViewModel
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; } 
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string Color { get; set; }
    }
}
