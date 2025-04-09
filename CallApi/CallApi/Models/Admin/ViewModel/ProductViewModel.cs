using Microsoft.AspNetCore.Mvc.Rendering;

namespace CallApi.Models.Admin.ViewModel
{
    public class ProductViewModel
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public bool ProductType { get; set; }
        public bool Status { get; set; }
        public string Display { get; set; }
        public string FrontCamera { get; set; }
        public string RearCamera { get; set; }
        public string Cpu { get; set; }
        public string System { get; set; }
        public string ConnectType { get; set; }
        public string Memory { get; set; }
        public DateTime CreatAt { get; set; }
    }
}
