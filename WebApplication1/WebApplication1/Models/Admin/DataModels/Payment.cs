namespace WebApplication1.Models.Admin.DataModels
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public double Amount { get; set; }
    }
}
