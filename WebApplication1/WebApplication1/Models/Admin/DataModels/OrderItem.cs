namespace WebApplication1.Models.Admin.DataModels
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string Color { get; set; }
    }
}
